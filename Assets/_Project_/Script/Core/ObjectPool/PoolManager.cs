using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;


namespace FXnRXn.ObjectPool
{
    public class PoolManager : MonoBehaviour
    {
        #region Singleton
		public static PoolManager Instance { get; private set; }

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

        [Header("Pool Settings")] [HorizontalLine(color: EColor.Blue)] 
        
        [SerializeField] private Transform poolContainer;
        [SerializeField] private bool prewarmOnStart = true;
        [SerializeField] private bool showDebugInfo = true;

        private Dictionary<string, IObjectPool> pools = new Dictionary<string, IObjectPool>();
        private Dictionary<GameObject, string> objectToPoolMap = new Dictionary<GameObject, string>();
        
        public bool IsInitialized { get; private set; }
        public int TotalPoolsCount => pools.Count;
        
        // Events
        public event Action OnPoolsInitialized;
        public event Action<string> OnPoolCreated; 

        #endregion

        #region Unity Callbacks

        private void Start()
        {
	        if (poolContainer == null)
	        {
		        GameObject container = new GameObject("Pool Container");
		        container.transform.SetParent(transform);
		        poolContainer = container.transform;
	        }

	        if (prewarmOnStart) InitializePools().Forget();
        }

        private void OnDestroy()
        {
	        ClearAllPools();
        }

        #endregion

        #region Custom Method
        
        //-- Create a new pool for the specified prefab
        public async UniTask<ObjectPool<T>> CreatePool<T>(T prefab, int initialSize = 10, int maxSize = 100,
	        bool allowGrowth = true, bool prewarm = true) where T : Component
        {
	        string poolKey = prefab.name;

	        if (pools.ContainsKey(poolKey))
	        {
		        Debug.LogWarning($"Pool already exists for {poolKey}");
		        return pools[poolKey] as ObjectPool<T>;
	        }

	        var pool = new ObjectPool<T>();
	        
	        // Create pool parent
	        GameObject poolParent = new GameObject($"Pool_{poolKey}");
	        poolParent.transform.SetParent(poolContainer);
	        
	        await pool.Initialize(prefab, initialSize, maxSize, allowGrowth, poolParent.transform);
	        
	        pools[poolKey] = pool;
	        OnPoolCreated?.Invoke(poolKey);
            
	        if (showDebugInfo) Debug.Log($"Pool created: {poolKey} (Initial: {initialSize}, Max: {maxSize})");
            
	        return pool;
	        
        }
        
        //-- Get a pool by prefab name
        public ObjectPool<T> GetPool<T>(string poolKey) where T : Component
        {
	        if (pools.TryGetValue(poolKey, out IObjectPool pool))
	        {
		        return pool as ObjectPool<T>;
	        }
            
	        Debug.LogError($"Pool not found: {poolKey}");
	        return null;
        }
        
        //-- Get an object from the specified pool
        public T Get<T>(string poolKey) where T : Component
        {
	        var pool = GetPool<T>(poolKey);
	        if (pool != null)
	        {
		        T obj = pool.Get();
		        if (obj != null)
		        {
			        objectToPoolMap[obj.gameObject] = poolKey;
		        }
		        return obj;
	        }
            
	        return null;
        }
        
        //-- Return an object to its pool
        public void Return<T>(T obj) where T : Component
        {
	        if (obj == null) return;
            
	        if (objectToPoolMap.TryGetValue(obj.gameObject, out string poolKey))
	        {
		        var pool = GetPool<T>(poolKey);
		        pool?.Return(obj);
		        objectToPoolMap.Remove(obj.gameObject);
	        }
	        else
	        {
		        Debug.LogWarning($"Object {obj.name} not found in pool mapping");
	        }
        }
        
        //-- Return object to pool using GameObject
        public void Return(GameObject obj)
        {
	        if (obj == null) return;
            
	        if (objectToPoolMap.TryGetValue(obj, out string poolKey))
	        {
		        var component = obj.GetComponent<Component>();
		        if (component != null)
		        {
			        var poolType = pools[poolKey].GetType();
			        var returnMethod = poolType.GetMethod("Return");
			        returnMethod?.Invoke(pools[poolKey], new object[] { component });
			        objectToPoolMap.Remove(obj);
		        }
	        }
        }
        
        //-- Check if a pool exists
        public bool HasPool(string poolKey)
        {
	        return pools.ContainsKey(poolKey);
        }
        
        //-- Get pool statistics
        public PoolStats GetPoolStats(string poolKey)
        {
	        if (pools.TryGetValue(poolKey, out IObjectPool pool))
	        {
		        return new PoolStats
		        {
			        PoolName = poolKey,
			        AvailableCount = pool.AvailableCount,
			        TotalCount = pool.TotalCount,
			        IsPrewarmed = pool.IsPrewarmed
		        };
	        }
            
	        return null;
        }
        
        //-- Clear specific pool
        public void ClearPool(string poolKey)
        {
	        if (pools.TryGetValue(poolKey, out IObjectPool pool))
	        {
		        pool.Clear();
		        pools.Remove(poolKey);
                
		        // Clean up object mapping
		        var keysToRemove = new List<GameObject>();
		        foreach (var kvp in objectToPoolMap)
		        {
			        if (kvp.Value == poolKey)
				        keysToRemove.Add(kvp.Key);
		        }
                
		        foreach (var key in keysToRemove)
		        {
			        objectToPoolMap.Remove(key);
		        }
                
		        if (showDebugInfo)
			        Debug.Log($"Pool cleared: {poolKey}");
	        }
        }
        
        //-- Clear all pools
        public void ClearAllPools()
        {
	        foreach (var pool in pools.Values)
	        {
		        pool?.Clear();
	        }
            
	        pools.Clear();
	        objectToPoolMap.Clear();
	        IsInitialized = false;
            
	        if (showDebugInfo)
		        Debug.Log("All pools cleared");
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
        
        #region Private Methods
        private async UniTask InitializePools()
        {
	        // This will be called by EnemyPoolManager
	        await UniTask.Yield();
	        IsInitialized = true;
	        OnPoolsInitialized?.Invoke();
            
	        if (showDebugInfo)
		        Debug.Log("PoolManager initialized");
        }
        #endregion

        #region Debug
        [Button("Show Pool Stats")]
        private void ShowPoolStats()
        {
	        if(!Application.isPlaying) return;
	        
	        Debug.Log("=== Pool Statistics ===");
	        foreach (var kvp in pools)
	        {
		       var stats = GetPoolStats(kvp.Key);
		       Debug.Log($"{stats.PoolName}: {stats.AvailableCount}/{stats.TotalCount} (Prewarmed: {stats.IsPrewarmed})");
	        }
        }
        
        [Button("Clear All Pools")]
        private void ClearAllPoolsButton()
        {
	        if (!Application.isPlaying) return;
			ClearAllPools();
        }

        #endregion

    }
    
    //-- Interface for pool management
    public interface IObjectPool
    {
	    int AvailableCount { get; }
	    int TotalCount { get; }
	    bool IsPrewarmed { get; }
	    void Clear();
    }
    
    //-- Pool statistics data
    [Serializable]
    public class PoolStats
    {
	    public string PoolName;
	    public int AvailableCount;
	    public int TotalCount;
	    public bool IsPrewarmed;
    }
}

