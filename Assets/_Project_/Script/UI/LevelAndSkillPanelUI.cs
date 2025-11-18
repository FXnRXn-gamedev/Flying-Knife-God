using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Tayx.Graphy.Utils.NumString;
using TMPro;
using UnityEngine.UI;


namespace FXnRXn
{
    public class LevelAndSkillPanelUI : MonoBehaviour
    {
        #region Singleton
		public static LevelAndSkillPanelUI Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Blue)] 
        [SerializeField] private Image					characterLevelProgressPanelFill;
        [SerializeField] private TMP_Text				characterLevelProgressPanelLevelText;
        [SerializeField] private Slider					characterStateProgressPanelLevelProgress;
        [SerializeField] private TMP_Text				characterStateProgressPanelDescText;



        #endregion

        #region Unity Callbacks

        #endregion

        #region Custom Method

        public void InitData()
        {
	        if (GetComponent<CanvasGroup>() != null)
	        {
		        GetComponent<CanvasGroup>().interactable = false;
		        GetComponent<CanvasGroup>().blocksRaycasts = false;
	        }

	        UpdateLevelAndSkill();
        }

        public void UpdateLevelAndSkill()
        {
	        UpdateExpValue();
        }

        

        private async UniTaskVoid UpdateExpValue()
        {
	        await UniTask.Yield();
	       if(PlayerManager.Instance) UpdateLevelSlider(PlayerManager.Instance.GetCurrentExp, PlayerManager.Instance.GetNextExp, PlayerManager.Instance.GetCurrentLevel);
        }
   
        private void UpdateLevelSlider(int _currentExp, int _nextExp, float _currentLevel)
        {
	        float value;
	        if (_currentExp <= _nextExp)
	        {
		        value = _currentExp.ToFloat() / _nextExp.ToFloat();
	        }
	        else
	        {
		        value = 1;
	        }
	        
	        if(characterLevelProgressPanelFill) characterLevelProgressPanelFill.fillAmount = value;
	        if(characterLevelProgressPanelLevelText) characterLevelProgressPanelLevelText.text = $"{(int)_currentLevel}";
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

