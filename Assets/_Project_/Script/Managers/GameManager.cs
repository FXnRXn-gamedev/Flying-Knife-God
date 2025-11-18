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
        
        public EGameState GameState { get; set; }
        public Action<EGameState> GameStateAction;
        #endregion

        #region Unity Callbacks

        private void Start()
        {
	        Application.targetFrameRate = targetFPS;
	        SetGameState(EGameState.GameUIStart);
	        InventoryManager.Instance?.InitData();
	        if(PlayerManager.Instance) AddPlayerExp(PlayerManager.Instance.GetCurrentExp);
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
	        }
        }

        public void SetGameState(EGameState state)
        {
	        GameStateAction?.Invoke(state);
	        GameState = state;
        }

        public void AddPlayerExp(int value)
        {
	        WorldData.AddPlayerExp(value);
	        if (LevelAndSkillPanelUI.Instance != null)
	        {
		        LevelAndSkillPanelUI.Instance.UpdateLevelAndSkill();
	        }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

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

