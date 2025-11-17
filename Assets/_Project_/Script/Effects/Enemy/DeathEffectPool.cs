using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using FXnRXn.ObjectPool;
using NaughtyAttributes;


namespace FXnRXn
{
    public class DeathEffectPool : MonoBehaviour
    {
        #region Singleton
		public static DeathEffectPool Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties
        
        [Header("Death Effect Pool Settings")]  [HorizontalLine(color: EColor.Blue)]
        [SerializeField] private int initialSize = 10;
        [SerializeField] private int maxSize = 30;
        
        private GameObject deathEffectPrefab;

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
	        if(Resources.Load<GameObject>("Effect/EnemyDeathEffect") == null ) return;
	        if (deathEffectPrefab == null) deathEffectPrefab = Resources.Load<GameObject>("Effect/EnemyDeathEffect");
        }

        private async UniTask InitializePool()
        {
	        if (PoolManager.Instance == null || deathEffectPrefab == null) return;
	        
	        var prefabTransform = deathEffectPrefab.GetComponent<Transform>();
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

        public void SpawnDeathEffect(Vector3 centerPosition, int amount)
        {
	        if (PoolManager.Instance == null || deathEffectPrefab == null) return;
	        
	        string poolKey = deathEffectPrefab.name;
	        if (!PoolManager.Instance.HasPool(poolKey)) return;
	        
	        for (int i = 0; i < amount; i++)
	        {
		        var deathEffectTransform = PoolManager.Instance.Get<Transform>(poolKey);
		        if (deathEffectTransform == null) continue;

		        deathEffectTransform.position = centerPosition;
		        
	        }
        }
        
        public async UniTaskVoid ReturnAfterDelay(Transform deathEffect)
        {
	        var deathEffectTransform = PoolManager.Instance.Get<Transform>(deathEffect.ToString());
	        await UniTask.Yield();

	        if (deathEffectTransform == null || PoolManager.Instance == null) return;

	        PoolManager.Instance.Return(deathEffectTransform);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
}

