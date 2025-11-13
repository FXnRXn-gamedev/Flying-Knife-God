using UnityEngine;
using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine.UI;


namespace FXnRXn
{
    public class GamePanelUI : UIPanel
    {
        #region Singleton
		public static GamePanelUI Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Green)] 
        [SerializeField] private TMP_Text currentGameLevelName;
        [SerializeField] private Button gamePlayBtn;
        [SerializeField] private Image  gamePlayBtnImg;
        #endregion

        #region Unity Callbacks
        

        #endregion

        #region Custom Method

        public override void SetUI()
        {
	        base.SetUI();
	        BindingButton();
        }

        public void SetLevelName(string levelName)
        {
	        if (currentGameLevelName != null)
	        {
		        currentGameLevelName.text = levelName.ToString();
	        }
        }

        public void BindingButton()
        {
	        if (gamePlayBtn != null)
	        {
		        gamePlayBtn.onClick.RemoveAllListeners();
		        gamePlayBtn.onClick.AddListener(PlayGame);
	        }
        }

        public void PlayGame()
        {
	        if(UIManager.Instance != null) UIManager.Instance.PlayGameBtnClick();
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

