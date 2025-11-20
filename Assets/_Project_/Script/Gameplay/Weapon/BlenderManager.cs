using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;


namespace FXnRXn
{
    public class BlenderManager : MonoBehaviour
    {
        #region Singleton
		public static BlenderManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties
        [Header("Settings")] [HorizontalLine(color: EColor.Orange)] 
        [field: SerializeField] private float speed = 10f;
        [field: SerializeField] private float gravity = 10f;
        [field: SerializeField] private Vector3 offPos = Vector3.zero;
        
        
        private int createCount = 0;
        private float lifeTime;
        private float createTimer = 2;
        
        
        public bool isStartToCreate { get; set; } = false;
        private GameObject _blenderPrefab;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
	       if(UIManager.Instance) UIManager.Instance.OnSkill1ButtonClick += TriggerBlenderSkill;
        }

        private void TriggerBlenderSkill()
        {
	        if (GameManager.Instance.currentGameState == EGameState.GameStart)
	        {
		        GetSkillData(1, WorldData.WorldLevel);
		        InitBlenderSkillData();
			       
	        }
        }

        private void InitBlenderSkillData()
        {
	        if (Resources.Load<GameObject>("Weapon/Blender") != null)
		        _blenderPrefab = Resources.Load<GameObject>("Weapon/Blender");

	        if (!isStartToCreate)
	        {
		        CreateBlender();
	        }
        }
        
        
        #endregion

        #region Custom Method

        private SkillAttributeEntity data;
        private SkillData skData;
        public void GetSkillData(int SkillID, int Level)
        {
	        if (InventoryManager.Instance != null) InventoryManager.Instance.InitSkillData();
	        
	        
	        for (int i = 0; i < WorldData.skillAttributeData.Count; i++)
	        {
		        if (WorldData.skillAttributeData[i].SkillID == SkillID)
		        {
			        data = WorldData.skillAttributeData[i];
			        break;
		        }
	        }
	        
	        for (int i = 0; i < data.Data.Count; i++)
	        {
		        if (data.Data[i].Level == Level)
		        {
			        skData = data.Data[i];
			        break;
		        }
	        }
	        
	        createCount = skData.Num1;
	        createTimer = skData.Num2;

	        
        }

        private async UniTaskVoid CreateBlender()
        {
	        await UniTask.Yield();
	        while (!isStartToCreate)
	        {
		        isStartToCreate = true;

		        for (int i = 0; i < createCount; i++)
		        {
			        ConstructBlener();
			        await UniTask.Delay(TimeSpan.FromSeconds(createTimer));
		        }

		        
		        await UniTask.Yield();
	        }
        }

        private void ConstructBlener()
        {
	        if(_blenderPrefab == null) return;
	        if(SkillPool.Instance == null) return;
	        Vector3 pos = PlayerManager.Instance.transform.position + (offPos + new Vector3(UnityEngine.Random.Range(-3f, 3f), 0f, UnityEngine.Random.Range(-3f, 3f)));
	        
	        SkillPool.Instance.SpawnBlenderSkill(pos, 1, skData);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

