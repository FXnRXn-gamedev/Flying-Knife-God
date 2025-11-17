using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FXnRXn.ObjectPool;
using NaughtyAttributes;
using Random = UnityEngine.Random;


namespace FXnRXn
{
    public class EnemySystemManager : MonoBehaviour
    {
        #region Singleton
		public static EnemySystemManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Green)] 
        [SerializeField] private LevelType currentLevelType;
        [SerializeField] private List<GameObject> enemyPosList;
        [SerializeField] private List<GameObject> enemyPrefabList;
        [SerializeField] private Transform enemyPart;
        [SerializeField] private List<GameObject> enemySurviveList;
        [SerializeField] private GameObject enemyController;
        
        [Header("Pool Integration")] [HorizontalLine(color: EColor.Blue)]
        [SerializeField] private bool useObjectPool = true;

        [SerializeField] private bool hasEnemyLifetime = false;
        [ShowIf("hasEnemyLifetime")][SerializeField] private float enemyLifetime = 30f; // Auto-return enemies after this time

        [SerializeField] private bool wantMaxActiveEnemies = false;
        [ShowIf("wantMaxActiveEnemies")][SerializeField] private int maxActiveEnemies = 20;

        private List<GameObject> currentEnemyPrefabList;
        public bool isRoundStarted { get; set; } = false;
        private bool isPoolReady = false;

        public Action CreateEnemyAction;
        #endregion

        #region Unity Callbacks

        private void Start()
        {
	        CreateEnemyAction += CreateEnemy;
	        // Wait for pools to be ready
	        if (EnemyPoolManager.Instance != null)
	        {
		        if (EnemyPoolManager.Instance.IsInitialized)
		        {
			        isPoolReady = true;
		        }
		        else
		        {
			        EnemyPoolManager.Instance.OnPoolsReady += OnPoolsReady;
		        }
	        }
        }
        
        private void OnDestroy()
        {
	        if (EnemyPoolManager.Instance != null)
	        {
		        EnemyPoolManager.Instance.OnPoolsReady -= OnPoolsReady;
	        }
        }

        #endregion

        #region Custom Method

        public void CreateEnemy()
        {
	        switch (currentLevelType)
	        {
		        case LevelType.Farm:
			        currentEnemyPrefabList = enemyPrefabList;
			        break;
		        case LevelType.TheBeach:
			        break;
		        case LevelType.TheWildWest:
			        break;
	        }
	        
			GenerateEnemyRound();
        }

        private async UniTask GenerateEnemyRound()
        {
	        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
	        while (isRoundStarted)
	        {
		        if (CheckIsCreateEnemy())
		        {
			        await UniTask.Delay(TimeSpan.FromSeconds(GetRandomSpawnEnemyTimerInterval()));
			        // Create enemy using pool system if available and enabled
			        if (useObjectPool && isPoolReady && EnemyPoolManager.Instance != null)
			        {
				        await CreateEnemyFromPool();
			        }
			        else
			        {
				        // Fallback to traditional instantiation
				        CreateEnemyTraditional();
			        }
		        }
		        await UniTask.Yield();
	        }
        }
        
        private async UniTask CreateEnemyFromPool()
        {
	        try
	        {
		        // Get enemy from pool
		        GameObject enemyInstantiatePrefab = EnemyPoolManager.Instance.GetRandomEnemy(currentLevelType);
		        if (enemyInstantiatePrefab == null)
		        {
			        Debug.LogWarning("Failed to get enemy from pool, using traditional method");
			        CreateEnemyTraditional();
			        return;
		        }

		        // Get controller from pool
		        GameObject part = EnemyPoolManager.Instance.GetEnemyController();
		        if (part == null)
		        {
			        Debug.LogWarning("Failed to get controller from pool, using traditional method");
			        EnemyPoolManager.Instance.ReturnEnemy(enemyInstantiatePrefab);
			        CreateEnemyTraditional();
			        return;
		        }

		        // Setup enemy hierarchy
		        enemyInstantiatePrefab.transform.SetParent(part.transform);

		        // Set position
		        Vector3 enemyPos = GetEnemySpawnPosition();
		        part.transform.SetParent(enemyPart.transform);
		        part.transform.position = enemyPos;
		        
		        part.GetComponent<EnemyController>()?.InitData();

		        // Add to survive list
		        enemySurviveList.Add(part);

		        // Auto-return enemy after lifetime
		        if (hasEnemyLifetime) AutoReturnEnemyAfterLifetime(part, enemyInstantiatePrefab).Forget();
		        
	        }
	        catch (Exception e)
	        {
		        Debug.LogError($"Error creating enemy from pool: {e.Message}");
		        CreateEnemyTraditional(); // Fallback
	        }
        }
        
        private void CreateEnemyTraditional()
        {
	        if (currentEnemyPrefabList != null && enemyController != null)
	        {
		        GameObject enemyInstantiatePrefab =
			        Instantiate(currentEnemyPrefabList[Random.Range(0, currentEnemyPrefabList.Count)]);
            
		        GameObject part = Instantiate(enemyController);
            
		        enemyInstantiatePrefab.transform.SetParent(part.transform);

		        Vector3 enemyPos = GetEnemySpawnPosition();
		        part.transform.SetParent(enemyPart.transform);
		        part.transform.position = enemyPos;
            
		        enemySurviveList.Add(part);
	        }
        }
        
        private async UniTask AutoReturnEnemyAfterLifetime(GameObject controller, GameObject enemy)
        {
	        await UniTask.Delay(TimeSpan.FromSeconds(enemyLifetime));
            
	        // Check if objects are still valid and active
	        if (controller != null && enemy != null && enemySurviveList.Contains(controller))
	        {
		        ReturnEnemyToPool(controller, enemy);
	        }
        }
        
        public void ReturnEnemyToPool(GameObject controller, GameObject enemy)
        {
	        if (!useObjectPool || !isPoolReady || EnemyPoolManager.Instance == null)
		        return;

	        // Remove from survive list
	        if (enemySurviveList.Contains(controller))
	        {
		        enemySurviveList.Remove(controller);
	        }

	        // Return to pools
	        EnemyPoolManager.Instance.ReturnEnemy(enemy);
	        EnemyPoolManager.Instance.ReturnController(controller);
        }

        private bool CheckIsCreateEnemy()
        {
	        RemoveNullEnemiesFromSurviveList(enemySurviveList);
	        // Add pool-aware logic
	        if (useObjectPool && isPoolReady && wantMaxActiveEnemies)
	        {
		        return enemySurviveList.Count < maxActiveEnemies;
	        }
	        return true;
        }
        
        private void OnPoolsReady()
        {
	        isPoolReady = true;
        }

        public void RemoveNullEnemiesFromSurviveList(List<GameObject> list)
        {
	        if (list.Count > 0 )
	        {
		        for (int i = 0; i < list.Count; i++)
		        {
			        if (list[i] == null)
			        {
				        list.RemoveAt(i);
				        i -= 1;
			        }
		        }
	        }
        }

        public Vector3 GetEnemySpawnPosition()
        {
	        return enemyPosList[Random.Range(0, enemyPosList.Count)].transform.position;
        }

        public float GetRandomSpawnEnemyTimerInterval()
        {
	        return 0.2f;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper
        [Button("Force Return All Enemies")]
        private void ForceReturnAllEnemies()
        {
	        if (!Application.isPlaying || !useObjectPool || !isPoolReady) return;
            
	        for (int i = enemySurviveList.Count - 1; i >= 0; i--)
	        {
		        var controller = enemySurviveList[i];
		        if (controller != null && controller.transform.childCount > 0)
		        {
			        var enemy = controller.transform.GetChild(0).gameObject;
			        ReturnEnemyToPool(controller, enemy);
		        }
	        }
        }
        
        [Button("Show Pool Stats")]
        private void ShowEnemyPoolStats()
        {
	        if (!Application.isPlaying || EnemyPoolManager.Instance == null) return;
            
	        var stats = EnemyPoolManager.Instance.GetAllEnemyPoolStats();
	        Debug.Log("=== Current Enemy Pool Stats ===");
	        foreach (var stat in stats)
	        {
		        Debug.Log($"{stat.PoolName}: {stat.AvailableCount}/{stat.TotalCount}");
	        }
        }
        #endregion

    }
}

