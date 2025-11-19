using UnityEngine;
using System;
using System.Collections.Generic;
using Defective.JSON;
using NaughtyAttributes;


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

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
}

