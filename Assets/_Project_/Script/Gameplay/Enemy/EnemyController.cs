using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine.AI;


namespace FXnRXn
{
    public class EnemyController : MonoBehaviour
    {
        #region Singleton
        public static EnemyController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            OnAwake();
        }

        #endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Blue)] 
        [SerializeField] private EEnemyState        currentEnemyState;
        [SerializeField] private bool               isAIMoving;


        [Header("Stats")] [HorizontalLine(color: EColor.Green)] 
        [SerializeField] private int                enemyHP;
        [SerializeField] private int                maxEnemyHP;
        [SerializeField] private int                enemyATK;
        [SerializeField] private float              attackDistance = 3f;
        [SerializeField] private float              enemyATKTime;
        [SerializeField] private float              currentATKTime;
        [SerializeField] private int                expValue = 1;
        [SerializeField] private int                expNum = 1;

        private NavMeshAgent                        _enemyNavAgent;
        private Animator                            _enemyAnim;
        private EnemyModelManager                   _enemyModelManager;
        private MeshRenderer[]                      _renderers;
        private FragmentController                  _fragmentControllerSc;


        #endregion

        #region Unity Callbacks

        private void OnAwake()
        {
            
        }

        private void Start()
        {
            if (_enemyNavAgent == null) _enemyNavAgent = GetComponent<NavMeshAgent>();
            if (_enemyModelManager == null) _enemyModelManager = GetComponentInChildren<EnemyModelManager>();
            
            enemyHP = maxEnemyHP;
            isAIMoving = true;
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
                isAIMoving = false;
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

