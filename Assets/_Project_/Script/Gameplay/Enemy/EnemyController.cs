using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FXnRXn.ObjectPool;
using NaughtyAttributes;
using UnityEngine.AI;
using Random = UnityEngine.Random;


namespace FXnRXn
{
    public class EnemyController : MonoBehaviour
    {
        #region Singleton
        public static EnemyController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        #endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Blue)] 
        [SerializeField] private EEnemyState        currentEnemyState;
        [SerializeField] private bool               isAIMoving;
        

        [Header("Stats")] [HorizontalLine(color: EColor.Green)] 
        [ReadOnly][SerializeField] private int                enemyHP;
        [ReadOnly][SerializeField] private int                maxEnemyHP;
        public int                enemyATK { get; set; }
        [ReadOnly][SerializeField] private float              attackDistance = 3f;
        [ReadOnly][SerializeField] private float              enemyATKTime;
        [SerializeField] private float                        currentATKTime;
        [ReadOnly][SerializeField] private int                expValue = 1;
        [ReadOnly][SerializeField] private int                expNum = 1;

        private NavMeshAgent                        _enemyNavAgent;
        private Animator                            _enemyAnim;
        private EnemyModelManager                   _enemyModelManager;
        private MeshRenderer[]                      _renderers;
        private FragmentController                  _fragmentControllerSc;
        private Dictionary<int, List<GameObject>>   _hurtModelList = new Dictionary<int, List<GameObject>>();


        #endregion

        #region Unity Callbacks

        public void InitData()
        {
            if (_enemyNavAgent == null) _enemyNavAgent = GetComponent<NavMeshAgent>();
            if (_enemyModelManager == null) _enemyModelManager = GetComponentInChildren<EnemyModelManager>();
            
            isAIMoving = true;
            EnemyData();
        }

        private void Update()
        {
            
            if (isAIMoving)
            {
                switch (currentEnemyState)
                {
                    case EEnemyState.Move:
                        if (_enemyNavAgent && PlayerManager.Instance != null)
                        {
                            _enemyNavAgent.SetDestination(PlayerManager.Instance.transform.position);
                        }
                        break;
                    case EEnemyState.Attack:
                        break;
                    case EEnemyState.Death:
                        
                        break;
                }
            }

            if(PlayerManager.Instance != null)
            {
                if(currentEnemyState == EEnemyState.Death) return;
                
                float distance = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position);
                if (distance > attackDistance)
                {
                    if (currentEnemyState != EEnemyState.Move)
                    {
                        ChangeState(EEnemyState.Move);
                    }
                }
                else if (distance < attackDistance)
                {
                    if (currentEnemyState != EEnemyState.Attack)
                    {
                        ChangeState(EEnemyState.Attack);
                    }
                }
            }
        }

        #endregion

        #region Custom Method

        public void DecreaseHP(int value)
        {
            enemyHP -= value;
            
            // Effect
            HurtEffect();
            if (enemyHP > 0)
            {
                CreateFragmentAndEffect();
            }
            

            if (enemyHP <= 0)
            {
                enemyHP = 0;
                ChangeState(EEnemyState.Death);
                EnemyDeathAndEffect();
                if (isAIMoving)
                {
                    EnemyDie();
                }
            }
        }

        private void EnemyDie()
        {
            isAIMoving = false;
            if(GetComponent<CapsuleCollider>()) GetComponent<CapsuleCollider>().enabled = false;
            if (_enemyModelManager == null) _enemyModelManager = GetComponentInChildren<EnemyModelManager>();

            CreateExpPrefab();
        }

        private void CreateExpPrefab()
        {
            if (ExpPool.Instance == null) return;

            ExpPool.Instance.SpawnExp(transform.position, expNum, expNum);
        }

        private async UniTask EnemyDeathAndEffect()
        {
            await UniTask.Yield();
            Transform effectTransform = null;
            
            // Get death effect from pool instead of instantiating it
            if (PoolManager.Instance != null && PoolManager.Instance.HasPool("EnemyDeathEffect"))
            {
                effectTransform = PoolManager.Instance.Get<Transform>("EnemyDeathEffect");
            }
            
            if (effectTransform != null)
            {
                var deathEffect = effectTransform.gameObject;
                deathEffect.transform.SetParent(transform);
                deathEffect.transform.localPosition = Vector3.zero;

                var ps = deathEffect.GetComponent<ParticleSystem>();
                if (ps != null) ps.Play();
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(2f));

            // Return effect to its pool
            if (effectTransform != null && PoolManager.Instance != null)
            {
                effectTransform.SetParent(DeathEffectPool.Instance != null 
                    ? DeathEffectPool.Instance.transform 
                    : null);
                PoolManager.Instance.Return(effectTransform);
            }
            
            // Return enemy + controller via EnemySystemManager so enemySurviveList and pools stay in sync
            if (EnemySystemManager.Instance != null)
            {
                GameObject controller = gameObject;
                GameObject enemy = null;

                if (controller.transform.childCount > 0)
                {
                    enemy = controller.transform.GetChild(0).gameObject;
                }

                if (enemy != null)
                {
                    EnemySystemManager.Instance.ReturnEnemyToPool(controller, enemy);
                }
                else
                {
                    // Fallback if child is missing but PoolManager exists
                    if (PoolManager.Instance != null)
                    {
                        PoolManager.Instance.Return(controller.transform);
                    }
                    else
                    {
                        Destroy(controller);
                    }
                }
            }
            else
            {
                // Return enemy to its pool instead of destroying it
                if (PoolManager.Instance != null)
                {
                    PoolManager.Instance.Return(transform);
                }
                else
                {
                    // Fallback if PoolManager is missing (safety)
                    Destroy(gameObject);
                }
            }

        }

        private void ChangeState(EEnemyState state)
        {
            GetAnimatorComponent();
            if (state == EEnemyState.Move)
            {
                _enemyNavAgent.isStopped = false;
            }
            else if (state == EEnemyState.Attack)
            {
                _enemyNavAgent.isStopped = true;
            }
            else if(state == EEnemyState.Death)
            {
                _enemyNavAgent.isStopped = true;
            }
            currentEnemyState = state;
        }

        private void GetAnimatorComponent()
        {
            if (_enemyAnim == null)
            {
                if (_enemyModelManager != null)
                {
                    _enemyAnim = _enemyModelManager._anim;
                }
                else
                {
                    _enemyAnim = GetComponentInChildren<Animator>();
                }
            }
        }


        public async UniTask EnemyData()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
            
            if (_enemyModelManager == null) _enemyModelManager = GetComponentInChildren<EnemyModelManager>();
            if(_enemyModelManager == null) return;

            _enemyModelManager.enemyLevel = WorldData.WorldLevel;

            if (_enemyModelManager.enemyLevel == 0)
            {
                enemyHP = 2;
                maxEnemyHP = 2;
                enemyATK = 1;
                attackDistance = 3;
                enemyATKTime = 1;
                expValue = 1;
                expNum = 1;
            }
            else
            {
                if(WorldData.enemyStatSOList.Count <= 0) InventoryManager.Instance?.InitEnemyData();
            
                EnemyStatSO enemyStat			= new EnemyStatSO();
                for (int i = 0; i < WorldData.enemyStatSOList.Count; i++)
                {
                    if (WorldData.enemyStatSOList[i].lvl == _enemyModelManager.enemyLevel)
                    {
                        enemyStat = WorldData.enemyStatSOList[i];
                        enemyHP = enemyStat.HP;
                        maxEnemyHP = enemyStat.HP;
                        enemyATK = enemyStat.AttackValue;
                        attackDistance = enemyStat.AttackRange;
                        enemyATKTime = enemyStat.AttackTime;
                        expValue = enemyStat.ExpValue;
                        expNum = enemyStat.ExpNum;
                        break;
                    }
                }
            }

            CheckEnemyHurtModelList();
        }

        private void CheckEnemyHurtModelList()
        {
            _hurtModelList.Clear();

            if (enemyHP == _enemyModelManager.ModelListGetter().Count)
            {
                for (int i = 0; i < enemyHP; i++)
                {
                    List<GameObject> modelList = new List<GameObject>();
                    modelList.Add(_enemyModelManager.ModelListGetter()[i]);
                    if (!_hurtModelList.ContainsKey(i))
                    {
                        _hurtModelList.Add(i, modelList);
                    }
                    else
                    {
                        _hurtModelList[i] = modelList;
                    }
                    
                }
            }
            else if (enemyHP < _enemyModelManager.ModelListGetter().Count)
            {
                int modelIndex = 0;
                while (modelIndex < _enemyModelManager.ModelListGetter().Count)
                {
                    for (int i = 0; i < enemyHP; i++)
                    {
                        if (modelIndex >= _enemyModelManager.ModelListGetter().Count) break;
                        List<GameObject> model = new List<GameObject>();
                        model.Add(_enemyModelManager.ModelListGetter()[modelIndex]);
                        if (!_hurtModelList.ContainsKey(i))
                        {
                            _hurtModelList.Add(i, model);
                        }
                        else
                        {
                            List<GameObject> tempModel = _hurtModelList[i];
                            tempModel.Add(_enemyModelManager.ModelListGetter()[modelIndex]);
                            _hurtModelList[i] = tempModel;
                        }

                        modelIndex++;
                    }
                }
            }
            else if (enemyHP > _enemyModelManager.ModelListGetter().Count)
            {
                for (int i = 0; i < enemyHP; i++)
                {
                    List<GameObject> model = new List<GameObject>();
                    if (!_hurtModelList.ContainsKey(i))
                    {
                        _hurtModelList.Add(i, model);
                    }
                    else
                    {
                        List<GameObject> tempModel = _hurtModelList[i];
                        tempModel.Add(_enemyModelManager.ModelListGetter()[i]);
                        _hurtModelList[i] = tempModel;
                    }
                }

                for (int i = _enemyModelManager.ModelListGetter().Count - 2; i >= 0; i--)
                {
                    List<GameObject> model = new List<GameObject>();
                    model.Add(_enemyModelManager.ModelListGetter()[i]);
                    if (!_hurtModelList.ContainsKey(i))
                    {
                        _hurtModelList.Add(i, model);
                    }
                    else
                    {
                        List<GameObject> tempModel = _hurtModelList[i];
                        tempModel.Add(_enemyModelManager.ModelListGetter()[i]);
                        _hurtModelList[i] = tempModel;
                    }
                }
                
                
                List<GameObject> models = new List<GameObject>();
                models.Add(_enemyModelManager.ModelListGetter()[_enemyModelManager.ModelListGetter().Count - 1]);
                if (!_hurtModelList.ContainsKey(enemyHP - 1))
                {
                    _hurtModelList.Add(enemyHP - 1, models);
                }
                else
                {
                    List<GameObject> tempModel = _hurtModelList[enemyHP - 1];
                    tempModel.Add(_enemyModelManager.ModelListGetter()[_enemyModelManager.ModelListGetter().Count - 1]);
                    _hurtModelList[enemyHP - 1] = tempModel;
                }
            }
            
            
            
        }

        private void SliceModel()
        {
            int value = maxEnemyHP - enemyHP;
            if(value == 0) return;

            for (int i = 0; i < value; i++)
            {
                if (_hurtModelList.ContainsKey(i))
                {
                    List<GameObject> modelList = _hurtModelList[i];
                    if (modelList.Count > 0)
                    {
                        for (int j = 0; j < modelList.Count; j++)
                        {
                            modelList[j].gameObject.SetActive(false);
                        }
                    }
                }
            }
        }


        private void CreateFragmentAndEffect()
        {
            if (_fragmentControllerSc == null)
            {
                _fragmentControllerSc = GetComponent<FragmentController>();
                _fragmentControllerSc.InitData();
            }

            GameObject fragment  = _fragmentControllerSc.CreateItem();
            if (_enemyModelManager != null && fragment != null)
            {
                fragment.GetComponent<MeshRenderer>().material =
                    MaterialManager.Instance?.GetEnemyMaterialList(_enemyModelManager.GetEnemyType);
            }
            //

            SliceModel();
        }

        private async UniTask HurtEffect()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));

            _renderers = GetComponentsInChildren<MeshRenderer>();

            if (_renderers.Length > 0)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if(_renderers[i] == null) continue;
                    for (int j = 0; j < _renderers[i].materials.Length; j++)
                    {
                        _renderers[i].materials[j].color = Color.red;
                    }
                }
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            
            if (_renderers.Length > 0)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if(_renderers[i] == null) continue;
                    for (int j = 0; j < _renderers[i].materials.Length; j++)
                    {
                        _renderers[i].materials[j].color = Color.white;
                    }
                }
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }

    public enum EEnemyState
    {
        Move,
        Attack,
        Death
    }
}

