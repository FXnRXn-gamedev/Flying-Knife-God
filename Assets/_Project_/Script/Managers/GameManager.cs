using UnityEngine;
using System;
using NaughtyAttributes;
using UnityEngine.UI;


namespace FXnRXn
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
		public static GameManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties
        
        [Header("Application Settings")]
        [HorizontalLine(color: EColor.Green)]
        [ReadOnly] [SerializeField] private int targetFPS = 120;
        
        public EGameState currentGameState { get; set; }
        public Action<EGameState> OnGameStateChanged;
        
        #endregion

        #region Unity Callbacks

        private void Start()
        {
	        Application.targetFrameRate = targetFPS;
	        SetGameState(EGameState.GameUIStart);
	        InventoryManager.Instance?.InitEnemyData();
	        InventoryManager.Instance?.InitPlayerData();
	        if(PlayerManager.Instance != null) PlayerManager.Instance.InitData();
	        if(PlayerManager.Instance) AddPlayerExp(PlayerManager.Instance.currentExp);
        }
        
        

        #endregion

        #region Custom Method

        public void GameStarted()
        {
	        if(EnemySystemManager.Instance != null) EnemySystemManager.Instance.isRoundStarted = true;
	        if(EnemySystemManager.Instance != null) EnemySystemManager.Instance.CreateEnemyAction?.Invoke();
	        SetGameState(EGameState.GameStart);
	        if (UIManager.Instance != null)
	        {
		        GameObject levelAndSkillPanel = UIManager.Instance.ShowUIPanel(UIType.LevelAndSkillPanel);
		        levelAndSkillPanel.GetComponent<LevelAndSkillPanelUI>().InitData();
		        
		        GameObject hpPanel = UIManager.Instance.ShowUIPanel(UIType.HPSliderPanel);
		        hpPanel.GetComponent<UIMove>().InitData();
		        
		        GameObject skillPanel = UIManager.Instance.ShowUIPanel(UIType.SkillPanel);
		        skillPanel.GetComponent<SkillUI>().InitData();
	        }
	        
        }

        public void GameOver()
        {
	        SetGameState(EGameState.GameOver);
        }

        public void SetGameState(EGameState state)
        {
	        OnGameStateChanged?.Invoke(state);
	        currentGameState = state;
        }
        
        public void AddPlayerExp(int value)
        {
	        WorldData.AddPlayerExp(value);
	        if (PlayerManager.Instance != null) PlayerManager.Instance.currentExp = WorldData.GetPlayerExp();
	        if (LevelAndSkillPanelUI.Instance != null)
	        {
		        LevelAndSkillPanelUI.Instance.UpdateLevelAndSkill();
	        }
        }
		
        public void IncreaseLevel()
        {
	        WorldData.WorldLevel += 1;
	       if(PlayerManager.Instance != null) PlayerManager.Instance.InitData();
	       if (BlenderManager.Instance != null) BlenderManager.Instance.GetSkillData(1, WorldData.WorldLevel);
        }
        

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        [Button]
        private void DebugAddExp()
        {
	        AddPlayerExp(100);
        }
        
        [Button]
        private void DebugAddHP()
        {
	        PlayerManager.Instance.AddHP(10);
        }
        
        [Button]
        private void DebugResetHP()
        {
	        PlayerManager.Instance.ResetHP(20);
        }

        #endregion
    
    }

    public enum LevelType
    {
	    Farm,
	    TheBeach,
	    TheWildWest
    }

    public enum EGameState
    {
	    GameUIStart,
	    GameStart,
	    GameOver
    }

    public enum EEnemyType
    {
	    Bean,
	    Blueberry,
	    Carrot,
	    ChickenLeg
    }
}

