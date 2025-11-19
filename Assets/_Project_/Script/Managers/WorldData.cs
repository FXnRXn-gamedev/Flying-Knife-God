using UnityEngine;
using System;
using System.Collections.Generic;


namespace FXnRXn
{
    public static class WorldData
    {
        public static List<PlayerStatSO> playerStatSOList = new List<PlayerStatSO>();
        public static List<EnemyStatSO> enemyStatSOList = new List<EnemyStatSO>();
        
        private static int _worldLevel = 1;

        public static int WorldLevel
        {
            get => _worldLevel;
            set => _worldLevel = value;
        }

        public static int ToInt(Enum e)
        {
            return Convert.ToInt32(e);
        }

        #region Player Data

        public static void AddPlayerExp(int value)
        {
            if (GetPlayerDataSO() == null) return;

            GetPlayerDataSO().playerExp += value;
        }
        
        public static int GetPlayerExp()
        {
            if (GetPlayerDataSO() == null) return 0;

            return GetPlayerDataSO().playerExp;
        }

        public static PlayerDataSO GetPlayerDataSO()
        {
            return Resources.Load<PlayerDataSO>("Data/player-data");
        }

        #endregion

        


        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper
        

        #endregion
    
    }
}

