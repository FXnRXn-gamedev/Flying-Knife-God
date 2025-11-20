using UnityEngine;
using System;
using NaughtyAttributes;
using UnityEngine.UI;


namespace FXnRXn
{
    public class SkillUI : MonoBehaviour
    {
        #region Singleton
		public static SkillUI Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Green)] 
        [field: SerializeField] private Button skill1Button;


        

        #endregion

        #region Unity Callbacks

        public void InitData()
        {
	        
        }

        private void Start()
        {
	        if (skill1Button != null)
	        {
		        skill1Button.onClick.RemoveAllListeners();
		        skill1Button.onClick.AddListener(()=> UIManager.Instance.OnSkill1ButtonClick?.Invoke());
	        }
        }

        #endregion

        #region Cutom Method

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

