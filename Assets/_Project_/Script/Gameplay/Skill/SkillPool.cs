using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using FXnRXn.ObjectPool;
using NaughtyAttributes;


namespace FXnRXn
{
    public class SkillPool : MonoBehaviour
    {
        #region Singleton
		public static SkillPool Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties
        [Header("Skill Pool Settings")]  [HorizontalLine(color: EColor.Blue)]
        [SerializeField] private int initialSize = 10;
        [SerializeField] private int maxSize = 30;


        private readonly string BLENDER_SKILL_KEY = "Weapon/Blender";
        private GameObject blenderSkillPrefab;

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
	        if(Resources.Load<GameObject>(BLENDER_SKILL_KEY) == null ) return;
	        if (blenderSkillPrefab == null) blenderSkillPrefab = Resources.Load<GameObject>(BLENDER_SKILL_KEY);
        }
        
        private async UniTask InitializePool()
        {
	        if (PoolManager.Instance == null || blenderSkillPrefab == null) return;
	        
	        var prefabTransform = blenderSkillPrefab.GetComponent<Transform>();
	        if (prefabTransform == null) return;
	        
	        
	        await PoolManager.Instance.CreatePool(
		        prefabTransform,
		        initialSize,
		        maxSize,
		        allowGrowth: true,
		        prewarm: true
	        );
	        
        }
        
        // Spawn EXP orbs from pool and schedule auto-return
        public void SpawnBlenderSkill(Vector3 centerPosition, int amount, SkillData skData)
        {
	        if (PoolManager.Instance == null || blenderSkillPrefab == null || skData == null) return;

	        string poolKey = blenderSkillPrefab.name;
	        if (!PoolManager.Instance.HasPool(poolKey)) return;

	        for (int i = 0; i < amount; i++)
	        {
		        var expTransform = PoolManager.Instance.Get<Transform>(poolKey);
		        if (expTransform == null) continue;

		        expTransform.position = centerPosition;

		        if (expTransform.GetComponent<BlenderWeapon>() != null)
		        {
			        expTransform.GetComponent<BlenderWeapon>().Initialize(skData);
		        }
	        }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

