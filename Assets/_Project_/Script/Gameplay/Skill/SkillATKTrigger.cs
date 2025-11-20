using UnityEngine;
using System;
using NaughtyAttributes;


namespace FXnRXn
{
    public class SkillATKTrigger : MonoBehaviour
    {
        #region Singleton
		public static SkillATKTrigger Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Blue)] 
        [field: SerializeField] private int ATKDamage = 1;

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
			        if(EC) EC.DecreaseHP(ATKDamage);
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

