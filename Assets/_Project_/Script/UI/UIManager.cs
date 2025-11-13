using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;


namespace FXnRXn
{
    public class UIManager : MonoBehaviour
    {
        #region Singleton
		public static UIManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties
        
        [Header("Settings")] 
        [HorizontalLine(color: EColor.Green)]
        public Transform UIPart;
        public Dictionary<UIType, GameObject> UIObject = new Dictionary<UIType, GameObject>();

        

        #endregion

        #region Unity Callbacks

        private void Start()
        {
	        InitData();
        }
        

        #endregion

        #region Custom Method
        
        public void InitData()
        {
	        ShowUIPanel(UIType.GamePanel);
	        if (GameCamera.Instance != null)
	        {
		        GameCamera.Instance.SetCameraType(ECameraType.GameUICamera);
	        }
        }

        public GameObject ShowUIPanel(UIType _uiType)
		{
	        if (UIObject.ContainsKey(_uiType))
	        {
		        GameObject panel = UIObject[_uiType];
		        panel.GetComponent<CanvasGroup>().alpha = 1f;
		        panel.GetComponent<CanvasGroup>().interactable = true;
		        panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
		        return panel;
	        }

	        GameObject CreatePanel = Instantiate(Resources.Load<GameObject>("UI/Panel/" + GetUIPanelName(_uiType)));
	        CreatePanel.transform.SetParent(UIPart);
	        CreatePanel.transform.localPosition = Vector3.zero;
	        RectTransform rectTransform = CreatePanel.GetComponent<RectTransform>();
	        
	        rectTransform.offsetMin = Vector2.zero;
	        rectTransform.offsetMax = Vector2.zero;

	        if (CreatePanel.GetComponent<UIPanel>() != null)
	        {
		        CreatePanel.GetComponent<UIPanel>().SetUI();
	        }
	        
	        UIObject.Add(_uiType, CreatePanel);
	        return CreatePanel;
		}
        
		public void HideUIPanel(UIType _uiType)
		{
			if (UIObject.ContainsKey(_uiType))
			{
				GameObject panel = UIObject[_uiType];
				panel.GetComponent<CanvasGroup>().alpha = 0f;
				panel.GetComponent<CanvasGroup>().interactable = false;
				panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
			}
		}

		public string GetUIPanelName(UIType type)
		{
			string panelName = String.Empty;
			switch (type)
			{
				case UIType.GamePanel:
					panelName = "Game-Panel";
					break;
				case UIType.JoystickRoot:
					panelName = "Joystick-Root";
					break;
			}
			return panelName;
		}
		
		
		//---------------

		public void PlayGameBtnClick()
		{
			HideUIPanel(UIType.GamePanel);
			ShowUIPanel(UIType.JoystickRoot);
			if (GameCamera.Instance != null) GameCamera.Instance.SetCameraType(ECameraType.GameCamera);
		}

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }

    public enum UIType
    {
	    GamePanel,
	    JoystickRoot
    }
}

