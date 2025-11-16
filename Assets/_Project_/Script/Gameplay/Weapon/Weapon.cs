using UnityEngine;
using System;
using NaughtyAttributes;


namespace FXnRXn
{
    public class Weapon : MonoBehaviour
    {
        #region Singleton
		public static Weapon Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        // [Header("Settings")]
        // [HorizontalLine(color: EColor.Green)]
        public int weaponATKDamage { get; set; } = 1;



        #endregion

        #region Unity Callbacks



        #endregion

        #region Custom Method

        private void OnTriggerEnter(Collider other)
        {
	        if (other.CompareTag("Enemy"))
	        {
		        if (other.GetComponent<EnemyController>() != null)
		        {
			        EnemyController EC = other.GetComponent<EnemyController>();
			        EC.DecreaseHP(weaponATKDamage);
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

