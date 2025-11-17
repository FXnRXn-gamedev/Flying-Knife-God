using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using FXnRXn.ObjectPool;
using NaughtyAttributes;


namespace FXnRXn
{
    public class ExpPool : MonoBehaviour
    {
        #region Singleton
		public static ExpPool Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        
        
        [Header("Exp Effect Pool Settings")]  [HorizontalLine(color: EColor.Blue)]
        [SerializeField] private int initialSize = 10;
        [SerializeField] private int maxSize = 30;
        
        private GameObject expEffectPrefab;

        #endregion

        #region Unity Callbacks
        
        private void Start()
        {
	        InitData();
	        InitializePool().Forget();
        }

        #endregion

        #region Custom Method
        
        private void InitData()
        {
	        if(Resources.Load<GameObject>("Other/ExpPrefab") == null ) return;
	        if (expEffectPrefab == null) expEffectPrefab = Resources.Load<GameObject>("Other/ExpPrefab");
        }

        private async UniTask InitializePool()
        {
	        if (PoolManager.Instance == null || expEffectPrefab == null) return;
	        
	        var prefabTransform = expEffectPrefab.GetComponent<Transform>();
	        if (prefabTransform == null) return;
	        
	        // Pool key will be deathEffectPrefab.name (e.g. "EnemyDeathEffect")
	        await PoolManager.Instance.CreatePool(
		        prefabTransform,
		        initialSize,
		        maxSize,
		        allowGrowth: true,
		        prewarm: true
	        );
	        
        }
        
        // Spawn EXP orbs from pool and schedule auto-return
        public void SpawnExp(Vector3 centerPosition, int amount, int expValue)
        {
	        if (PoolManager.Instance == null || expEffectPrefab == null) return;

	        string poolKey = expEffectPrefab.name;
	        if (!PoolManager.Instance.HasPool(poolKey)) return;

	        for (int i = 0; i < amount; i++)
	        {
		        var expTransform = PoolManager.Instance.Get<Transform>(poolKey);
		        if (expTransform == null) continue;

		        expTransform.position = centerPosition +
		                                new Vector3(
			                                UnityEngine.Random.Range(0f, .5f),
			                                0.0f,
			                                UnityEngine.Random.Range(0f, .5f)
		                                );

		        if (expTransform.GetComponent<ExpPrefab>() != null)
		        {
			        expTransform.GetComponent<ExpPrefab>().InitData();
			        expTransform.GetComponent<ExpPrefab>().expNum = expValue;
		        }

		        //ReturnAfterDelay(expTransform).Forget();
	        }
        }

        // public async UniTaskVoid ReturnAfter(Transform exp)
        // {
	       //  var expTransform = PoolManager.Instance.Get<Transform>(exp.ToString());
	       //  await UniTask.Yield();
        //
	       //  if (expTransform == null || PoolManager.Instance == null) return;
        //
	       //  PoolManager.Instance.Return(expTransform);
        // }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
}

