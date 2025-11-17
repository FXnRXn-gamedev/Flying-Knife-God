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
        
        [Expandable] [SerializeField] private List<EnemyDataSO> _enemyDataSO;

        #endregion

        #region Method

        public void InitData()
        {
	        ParseEnemyDataJsonList();
        }

        #endregion

        #region Enemy Data

        private void ParseEnemyDataJsonList()
        {
	        if(Resources.Load<TextAsset>("JsonData/EnemyData") == null) return;
	        
	        _enemyDataSO.Clear();
	        string itemJson = "";
	        TextAsset itemText = Resources.Load<TextAsset>("JsonData/EnemyData");
	        itemJson = itemText.text;

	        JSONObject j = new JSONObject(itemJson);
	        foreach (var jsonObject in j.list)
	        {
		        EnemyDataSO data			= new EnemyDataSO();
		        data.lvl					= jsonObject["lv"].intValue;
		        data.HP						= jsonObject["HP"].intValue;
		        data.AttackValue			= jsonObject["AttackValue"].intValue;
		        data.AttackRange			= jsonObject["AttackRange"].intValue;
		        data.AttackTime				= jsonObject["AttackTime"].intValue;
		        data.ExpValue				= jsonObject["ExpValue"].intValue;
		        data.ExpNum					= jsonObject["ExpNum"].intValue;
		        
		        _enemyDataSO.Add(data);
	        }
	        
	        WorldData.enemyDataSOList.Clear();
	        _enemyDataSO.ForEach(dataSO =>
	        {
		        WorldData.enemyDataSOList.Add(dataSO);
	        });
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
}

