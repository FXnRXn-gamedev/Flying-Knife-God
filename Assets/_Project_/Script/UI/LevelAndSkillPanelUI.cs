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
	        int tempPlayerExp;
	        if(PlayerManager.Instance) UpdateLevelSlider(PlayerManager.Instance.currentExp, PlayerManager.Instance.nextExp, PlayerManager.Instance.currentLevel);

	        while (PlayerManager.Instance.currentExp >= PlayerManager.Instance.nextExp)
	        {
		        tempPlayerExp = PlayerManager.Instance.currentExp - PlayerManager.Instance.nextExp;
		        
		        if(GameManager.Instance) GameManager.Instance.IncreaseLevel();
		       // PlayerManager.Instance.currentExp = tempPlayerExp;
		        WorldData.GetPlayerDataSO().playerExp = tempPlayerExp;
		        if(PlayerManager.Instance != null) PlayerManager.Instance.InitData();
		        if(PlayerManager.Instance) UpdateLevelSlider(PlayerManager.Instance.currentExp, PlayerManager.Instance.nextExp, PlayerManager.Instance.currentLevel);

		        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
	        }
	        
	        await UniTask.Yield();
	       
        }
   
        public void UpdateLevelSlider(int _currentExp, int _nextExp, float _currentLevel)
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

        public void SeekPlayerLevelData(int lvl)
        {
	        if (WorldData.playerStatSOList.Count <= 0)
	        {
		        InventoryManager.Instance?.InitPlayerData();
	        }

	        PlayerStatSO tempPlayerStat = new PlayerStatSO();

	        for (int i = 0; i < WorldData.playerStatSOList.Count; i++)
	        {
		        if (WorldData.playerStatSOList[i].lvl == lvl)
		        {
			        tempPlayerStat = WorldData.playerStatSOList[i];
			        break;
		        }
	        }

	        if (PlayerManager.Instance.currentLevel > WorldData.playerStatSOList.Count)
	        {
		        tempPlayerStat.hp = ((PlayerManager.Instance.currentLevel * 10) + 90);
		        tempPlayerStat.nextExp = ((PlayerManager.Instance.currentLevel * 10) + 20);
		        tempPlayerStat.moveSpeed = 20f;
	        }

	        PlayerManager.Instance.currentHP = tempPlayerStat.hp;
	        PlayerManager.Instance.maxHP = tempPlayerStat.hp;
	        PlayerManager.Instance.nextExp = tempPlayerStat.nextExp;
	        
	        UIMove.Instance.UpdateHPUISlider(PlayerManager.Instance.currentHP, PlayerManager.Instance.maxHP);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

