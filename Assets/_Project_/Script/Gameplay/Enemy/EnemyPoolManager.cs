using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using FXnRXn.ObjectPool;


namespace FXnRXn.ObjectPool
{
	/// <summary>
	/// Specialized pool manager for enemy objects
	/// Integrates with EnemySystemManager and provides enemy-specific pooling features
	/// </summary>
    public class EnemyPoolManager : MonoBehaviour
    {
        #region Singleton
		public static EnemyPoolManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		#endregion

        #region Properties
        [Header("Enemy Pool Configuration")] [HorizontalLine(color: EColor.Red)]
        [SerializeField] private List<EnemyPoolConfig> enemyPoolConfigs = new List<EnemyPoolConfig>();
        
        [Header("Controller Pool Configuration")] [HorizontalLine(color: EColor.Orange)]
        [SerializeField] private GameObject enemyControllerPrefab;
        [SerializeField] private int controllerPoolSize = 20;
        [SerializeField] private int maxControllerPoolSize = 50;
        
        [Header("Pool Settings")] [HorizontalLine(color: EColor.Yellow)]
        [SerializeField] private bool prewarmOnStart = true;
        [SerializeField] private bool enablePoolStats = true;
        [SerializeField] private float poolCleanupInterval = 30f;
        
        private Dictionary<LevelType, List<string>> levelEnemyPools = new Dictionary<LevelType, List<string>>();
        private ObjectPool<Transform> controllerPool;
        
        public bool IsInitialized { get; private set; }
        public event Action OnPoolsReady;

        #endregion

        #region Unity Callbacks
        private void Start()
        {
	        if (prewarmOnStart) InitializeEnemyPools().Forget();
        }

        private void OnDestroy()
        {
	        StopAllCoroutines();
        }

        #endregion

        #region Custom Method
        
        //-- Initialize all enemy pools based on configuration
        public async UniTask InitializeEnemyPools()
        {
	        if (PoolManager.Instance == null)
	        {
		        Debug.LogError("PoolManager instance not found!");
		        return;
	        }

	        try
	        {
		        // Initialize controller pool first
		        await InitializeControllerPool();
                
		        // Initialize enemy pools
		        foreach (var config in enemyPoolConfigs)
		        {
			        await CreateEnemyPool(config);
		        }

		        // Setup level-specific pool mappings
		        SetupLevelPoolMappings();
                
		        IsInitialized = true;
		        OnPoolsReady?.Invoke();
                
		        // Start periodic cleanup
		        StartPeriodicCleanup();
                
		        Debug.Log($"Enemy pools initialized: {enemyPoolConfigs.Count} enemy types, 1 controller pool");
	        }
	        catch (Exception e)
	        {
		        Debug.LogError($"Failed to initialize enemy pools: {e.Message}");
	        }
        }
        
        //-- Get a random enemy from the current level's pool
        public GameObject GetRandomEnemy(LevelType levelType)
        {
	        if (!IsInitialized)
	        {
		        Debug.LogWarning("Enemy pools not initialized yet!");
		        return null;
	        }

	        if (!levelEnemyPools.TryGetValue(levelType, out List<string> availablePools))
	        {
		        Debug.LogWarning($"No enemy pools found for level: {levelType}");
		        return null;
	        }

	        if (availablePools.Count == 0)
	        {
		        Debug.LogWarning($"Empty enemy pool list for level: {levelType}");
		        return null;
	        }

	        string randomPoolKey = availablePools[UnityEngine.Random.Range(0, availablePools.Count)];
	        return PoolManager.Instance.Get<Transform>(randomPoolKey)?.gameObject;
        }
        
        
        //-- Get enemy controller from pool
        public GameObject GetEnemyController()
        {
	        if (controllerPool == null)
	        {
		        Debug.LogWarning("Controller pool not initialized!");
		        return null;
	        }

	        Transform controller = controllerPool.Get();
	        return controller?.gameObject;
        }
        
        //-- Return enemy to pool
        public void ReturnEnemy(GameObject enemy)
        {
	        if (enemy == null) return;
            
	        // Reset enemy state before returning
	        ResetEnemyState(enemy);
	        PoolManager.Instance.Return(enemy);
        }
        
        //-- Return controller to pool
        public void ReturnController(GameObject controller)
        {
	        if (controller == null || controllerPool == null) return;
            
	        // Reset controller state
	        ResetControllerState(controller);
	        controllerPool.Return(controller.transform);
        }
        
        //-- Get pool statistics for debugging
        
        public List<PoolStats> GetAllEnemyPoolStats()
        {
	        var stats = new List<PoolStats>();
            
	        foreach (var config in enemyPoolConfigs)
	        {
		        var poolStats = PoolManager.Instance.GetPoolStats(config.enemyPrefab.name);
		        if (poolStats != null)
			        stats.Add(poolStats);
	        }

	        // Add controller pool stats
	        if (controllerPool != null)
	        {
		        stats.Add(new PoolStats
		        {
			        PoolName = "EnemyController",
			        AvailableCount = controllerPool.AvailableCount,
			        TotalCount = controllerPool.TotalCount,
			        IsPrewarmed = controllerPool.IsPrewarmed
		        });
	        }

	        return stats;
        }
        
        //-- Expand specific enemy pool
        public async UniTask ExpandEnemyPool(string enemyName, int additionalCount)
        {
	        var pool = PoolManager.Instance.GetPool<Transform>(enemyName);
	        if (pool != null)
	        {
		        await pool.ExpandPool(additionalCount);
		        Debug.Log($"Expanded {enemyName} pool by {additionalCount} objects");
	        }
        }

        #endregion
        
        #region Private Methods
        private async UniTask InitializeControllerPool()
        {
            if (enemyControllerPrefab == null)
            {
                Debug.LogError("Enemy controller prefab is not assigned!");
                return;
            }

            controllerPool = await PoolManager.Instance.CreatePool(
                enemyControllerPrefab.transform,
                controllerPoolSize,
                maxControllerPoolSize,
                true,
                true
            );

            Debug.Log($"Controller pool initialized: {controllerPoolSize} objects");
        }

        private async UniTask CreateEnemyPool(EnemyPoolConfig config)
        {
            if (config.enemyPrefab == null)
            {
                Debug.LogWarning($"Enemy prefab is null in config for {config.levelType}");
                return;
            }

            await PoolManager.Instance.CreatePool(
                config.enemyPrefab.transform,
                config.initialPoolSize,
                config.maxPoolSize,
                config.allowPoolGrowth,
                true
            );

            Debug.Log($"Enemy pool created: {config.enemyPrefab.name} for {config.levelType}");
        }

        private void SetupLevelPoolMappings()
        {
            levelEnemyPools.Clear();

            foreach (var config in enemyPoolConfigs)
            {
                if (!levelEnemyPools.ContainsKey(config.levelType))
                {
                    levelEnemyPools[config.levelType] = new List<string>();
                }

                levelEnemyPools[config.levelType].Add(config.enemyPrefab.name);
            }
        }

        private void ResetEnemyState(GameObject enemy)
        {
            // Reset common enemy properties
            if (enemy.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (enemy.TryGetComponent<Rigidbody2D>(out var rb2d))
            {
                rb2d.linearVelocity = Vector2.zero;
                rb2d.angularVelocity = 0f;
            }

            // Reset any enemy-specific components
            var enemyComponents = enemy.GetComponents<MonoBehaviour>();
            foreach (var component in enemyComponents)
            {
                if (component is IPoolable poolableComponent)
                {
                    poolableComponent.OnReturnToPool();
                }
            }
        }

        private void ResetControllerState(GameObject controller)
        {
            // Clear all children (enemies)
            for (int i = controller.transform.childCount - 1; i >= 0; i--)
            {
                var child = controller.transform.GetChild(i);
                ReturnEnemy(child.gameObject);
            }

            // Reset controller properties
            controller.transform.position = Vector3.zero;
            controller.transform.rotation = Quaternion.identity;

            var controllerComponents = controller.GetComponents<MonoBehaviour>();
            foreach (var component in controllerComponents)
            {
                if (component is IPoolable poolableComponent)
                {
                    poolableComponent.OnReturnToPool();
                }
            }
        }

        private void StartPeriodicCleanup()
        {
            if (poolCleanupInterval > 0)
            {
                InvokeRepeating(nameof(PeriodicCleanup), poolCleanupInterval, poolCleanupInterval);
            }
        }

        private void PeriodicCleanup()
        {
            // Implement any periodic cleanup logic here
            // For example: remove excess pooled objects, garbage collection, etc.
            if (enablePoolStats)
            {
                Debug.Log("=== Enemy Pool Periodic Cleanup ===");
                var stats = GetAllEnemyPoolStats();
                foreach (var stat in stats)
                {
                    Debug.Log($"{stat.PoolName}: {stat.AvailableCount}/{stat.TotalCount}");
                }
            }
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
        
        #region Debug Methods
        [Button("Initialize Pools")]
        private void InitializePoolsButton()
        {
	        if (!Application.isPlaying) return;
	        InitializeEnemyPools().Forget();
        }

        [Button("Show Pool Statistics")]
        private void ShowPoolStatistics()
        {
	        if (!Application.isPlaying || !IsInitialized) return;

	        var stats = GetAllEnemyPoolStats();
	        Debug.Log("=== Enemy Pool Statistics ===");
	        foreach (var stat in stats)
	        {
		        Debug.Log($"{stat.PoolName}: Available({stat.AvailableCount}) / Total({stat.TotalCount}) - Prewarmed: {stat.IsPrewarmed}");
	        }
        }
        #endregion
    
    }
	//-- Configuration class for enemy pool setup
	[Serializable]
	public class EnemyPoolConfig
	{
		[Header("Pool Configuration")]
		public LevelType levelType;
		public GameObject enemyPrefab;
		[Range(5, 50)] public int initialPoolSize = 10;
		[Range(10, 100)] public int maxPoolSize = 30;
		public bool allowPoolGrowth = true;

		[Header("Pool Behavior")]
		[Tooltip("Automatically return enemy to pool when destroyed")]
		public bool autoReturnOnDestroy = true;
		[Tooltip("Reset enemy properties when returning to pool")]
		public bool resetOnReturn = true;
	}
	
	//-- Interface for objects that need special handling when returned to pool
	public interface IPoolable
	{
		void OnReturnToPool();
		void OnGetFromPool();
	}
}

