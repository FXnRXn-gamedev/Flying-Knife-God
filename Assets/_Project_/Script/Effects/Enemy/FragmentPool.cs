using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using FXnRXn.ObjectPool;
using NaughtyAttributes;
using Random = UnityEngine.Random;


namespace FXnRXn
{
    public class FragmentPool : MonoBehaviour
    {
        #region Singleton
		public static FragmentPool Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties
        
        [Header("Settings")] [HorizontalLine(color: EColor.Green)]
        [SerializeField] private int sunPoolInitialSize = 10;
        [SerializeField] private int sunPoolMaxSize = 50;
        [SerializeField] private bool sunPoolAllowGrowth = true;

		public string poolKey { get; private set; }
        private GameObject sunPrefab;
        #endregion

        #region Unity Callbacks

        private void Start()
        {
	        
        }

        #endregion

        #region Custom Method

        public async UniTask InitData()
        {
	        int value = Random.Range(1, 3);

	        if (value == 1)
	        {
		        sunPrefab = Resources.Load<GameObject>("EnemyPrefab/SLICE_FinalSphere");
	        }
	        else
	        {
		        sunPrefab = Resources.Load<GameObject>("EnemyPrefab/SLICE_SolidRound");
	        }

	        if (sunPrefab == null)
	        {
		        Debug.LogError("FragmentController: sunPrefab could not be loaded from Resources.");
		        return;
	        }

	        // Ensure PoolManager instance exists
	        if (PoolManager.Instance == null)
	        {
		        Debug.LogError("FragmentController: PoolManager.Instance is null. Make sure a PoolManager is in the scene.");
		        return;
	        }

	        poolKey = sunPrefab.name;

	        // Create pool only if it doesn't already exist
	        if (!PoolManager.Instance.HasPool(poolKey))
	        {
		        // Create a pool for the sun prefab (GameObject)
		        await PoolManager.Instance.CreatePool(
			        sunPrefab.transform,
			        sunPoolInitialSize,
			        sunPoolMaxSize,
			        sunPoolAllowGrowth,
			        prewarm: true
		        );
	        }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
}

