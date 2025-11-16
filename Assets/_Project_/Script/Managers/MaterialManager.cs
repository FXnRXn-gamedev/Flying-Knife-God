using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;


namespace FXnRXn
{
    public class MaterialManager : MonoBehaviour
    {
        #region Singleton
		public static MaterialManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Blue)]
        [SerializeField] private List<EnemyMaterialConfig> enemyMaterialList;

        #endregion

        #region Unity Callbacks

        #endregion

        #region Custom Method

        public Material GetEnemyMaterialList(EEnemyType type)
        {
	        Material mat = null;
	        if (enemyMaterialList == null) return null;

	        foreach (var materialConfig in enemyMaterialList)
	        {
		        if (materialConfig.type == type) mat = materialConfig.material;
	        }
	        return mat;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        

        #endregion

    }

    [Serializable]
    public class EnemyMaterialConfig
    {
	    public EEnemyType type;
	    public Material material;
    }
}

