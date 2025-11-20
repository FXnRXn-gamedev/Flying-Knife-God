using UnityEngine;
using System;
using System.Collections.Generic;
using Defective.JSON;
using NaughtyAttributes;
using Newtonsoft.Json;


namespace FXnRXn
{
    public class InventoryManager : MonoBehaviour
    {
        #region Singleton
		public static InventoryManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Blue)] 
        
        [Expandable] [SerializeField] private List<EnemyStatSO> _enemyStatSO;
        [Expandable] [SerializeField] private List<PlayerStatSO> _playerStatSO;
        [SerializeField] private List<SkillAttributeEntity> skillList = new List<SkillAttributeEntity> ();

        #endregion

        #region Method

        public void InitEnemyData()
        {
	        ParseEnemyDataJsonList();
        }

        public void InitPlayerData()
        {
	        ParsePlayerDataJsonList();
        }

        public void InitSkillData()
        {
	        ParseSkillDataJsonList();
        }

        #endregion

        #region Player Data

        private void ParsePlayerDataJsonList()
        {
	        if(Resources.Load<TextAsset>("JsonData/PlayerData") == null) return;
	        
	        _playerStatSO.Clear();
	        string itemJson = "";
	        TextAsset itemText = Resources.Load<TextAsset>("JsonData/PlayerData");
	        itemJson = itemText.text;
	        
	        JSONObject j = new JSONObject(itemJson);
	        foreach (var jsonObject in j.list)
	        {
		        PlayerStatSO stat			= new PlayerStatSO();
		        stat.lvl					= jsonObject["lv"].intValue;
		        stat.hp						= jsonObject["HP"].floatValue;
		        stat.nextExp				= jsonObject["NextExp"].intValue;
		        stat.moveSpeed				= jsonObject["MoveSpeed"].floatValue;
		        
		        _playerStatSO.Add(stat);
	        }
	        
	        WorldData.playerStatSOList.Clear();
	        _playerStatSO.ForEach(dataSO =>
	        {
		        WorldData.playerStatSOList.Add(dataSO);
	        });
        }

        #endregion

        #region Enemy Data

        private void ParseEnemyDataJsonList()
        {
	        if(Resources.Load<TextAsset>("JsonData/EnemyData") == null) return;
	        
	        _enemyStatSO.Clear();
	        string itemJson = "";
	        TextAsset itemText = Resources.Load<TextAsset>("JsonData/EnemyData");
	        itemJson = itemText.text;

	        JSONObject j = new JSONObject(itemJson);
	        foreach (var jsonObject in j.list)
	        {
		        EnemyStatSO stat			= new EnemyStatSO();
		        stat.lvl					= jsonObject["lv"].intValue;
		        stat.HP						= jsonObject["HP"].intValue;
		        stat.AttackValue			= jsonObject["AttackValue"].intValue;
		        stat.AttackRange			= jsonObject["AttackRange"].intValue;
		        stat.AttackTime				= jsonObject["AttackTime"].intValue;
		        stat.ExpValue				= jsonObject["ExpValue"].intValue;
		        stat.ExpNum					= jsonObject["ExpNum"].intValue;
		        
		        _enemyStatSO.Add(stat);
	        }
	        
	        WorldData.enemyStatSOList.Clear();
	        _enemyStatSO.ForEach(dataSO =>
	        {
		        WorldData.enemyStatSOList.Add(dataSO);
	        });
        }

        

        #endregion

        #region Skill Data

        private void ParseSkillDataJsonList()
        {
	        if(Resources.Load<TextAsset>("JsonData/SkillData/SkillAttribute") == null) return;
	        
	        string itemJson = "";
	        TextAsset itemText = Resources.Load<TextAsset>("JsonData/SkillData/SkillAttribute");
	        itemJson = itemText.text;

	        List<SkillAttributeData> skillData = JsonConvert.DeserializeObject<List<SkillAttributeData>>(itemJson);
	        
	        foreach (var data in skillData)
	        {
		        SkillAttributeEntity skillEntity = new SkillAttributeEntity();
		        skillEntity.SkillID = data.SkillID;
		        skillEntity.SkillName = data.SkillName;
		        foreach (var d in data.Data)
		        {
			        SkillData _data = new SkillData();
			        _data.Level = d.Level;
			        _data.Num1 = d.Num1;
			        _data.Num2 = d.Num2;
			        _data.Num3 = d.Num3;
			        _data.Num4 = d.Num4;
			        skillEntity.Data.Add(_data);
		        }
		        skillList.Add(skillEntity);
		        
	        }

	       WorldData.skillAttributeData = skillList;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
	

    [Serializable]
    public class SkillAttributeData
    {
	    public int SkillID;
	    public string SkillName;
	    public List<SkillData> Data;
    }
	
    [Serializable]
    public class SkillData
    {
	    public int Level;
	    public int Num1;
	    public int Num2;
	    public int Num3;
	    public int Num4;
    }
}

