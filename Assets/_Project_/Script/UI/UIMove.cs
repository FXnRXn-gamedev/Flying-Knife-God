using UnityEngine;
using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine.UI;


namespace FXnRXn
{
    public class UIMove : MonoBehaviour
    {
        #region Singleton
		public static UIMove Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties

        [Header("Settings")] [HorizontalLine(color: EColor.Green)] 
        [SerializeField] private Vector3 offset;
        [SerializeField] private Transform UIObject;
        [SerializeField] private Image HPBackground;
        [SerializeField] private TMP_Text HPValueText;
		
        private Transform target;
        private Camera mainCamera;
        #endregion

        #region Unity Callbacks

        public void InitData()
        {
	        if (mainCamera == null) mainCamera = Camera.main;
	        if (target == null) target = PlayerManager.Instance.transform;

	        if (GetComponent<CanvasGroup>() != null)
	        {
		        GetComponent<CanvasGroup>().interactable = false;
		        GetComponent<CanvasGroup>().blocksRaycasts = false;
	        }
	        if (PlayerManager.Instance != null)
	        {
		        UpdateHPUISlider(PlayerManager.Instance.currentHP, PlayerManager.Instance.maxHP);
	        }
        }

        private void Update()
        {
	        if (target != null)
	        {
		        Vector3 screenPoint = mainCamera.WorldToScreenPoint(target.position);
		        UIObject.transform.position = screenPoint + offset;
	        }
	        else
	        {
		        target = PlayerManager.Instance.transform;
	        }

	        if (PlayerManager.Instance != null)
	        {
		        UpdateHPUISlider(PlayerManager.Instance.currentHP, PlayerManager.Instance.maxHP);
	        }
        }

        #endregion

        #region Custom Method
        

        public void UpdateHPUISlider(float current, float max)
        {
	        if (current <= max)
	        {
		        if(HPBackground != null) HPBackground.fillAmount = current / max;
		        
	        }
	        else
	        {
		        HPBackground.fillAmount = 1;
	        }
	        
	        UpdateHPValueText(current, max);
        }

        private void UpdateHPValueText(float current, float max)
        {
	       if(HPValueText != null) HPValueText.text = "<size=30>" + current.ToString("F0") + "/" + "<size=35>" + max.ToString("F0");
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion

    }
}

