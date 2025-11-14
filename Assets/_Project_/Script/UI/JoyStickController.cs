using UnityEngine;
using UnityEngine.UI;
using System;
using NaughtyAttributes;
using UnityEngine.EventSystems;


namespace FXnRXn
{
    public class JoyStickController : InputClickHandler
    {
        #region Singleton
        public static JoyStickController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        #endregion

        #region Properties

        [Header("Joystick Settings")] 
        [HorizontalLine(color: EColor.Green)] 
        [SerializeField] private float          pointDis = 140f;
        [SerializeField] private Image          imgTouch;
        [SerializeField] private Image          imgDirBG;
        [SerializeField] private Image          imgDirPoint;
        [SerializeField] private Transform      ArrowRoot;
        
        private Vector2 startPos = Vector2.zero;
        private Vector2 defaultPos = Vector2.zero;
        private Vector3 moveDir = Vector3.zero;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            if(ArrowRoot) SetActive(ArrowRoot, false);
            if(imgDirBG) defaultPos = imgDirBG.transform.position;
            RegisterMoveEvents();
        }

        #endregion

        #region Custom Method

        private void RegisterMoveEvents()
        {
            SetActive(ArrowRoot, false);
            OnClickDown(imgTouch.gameObject, (PointerEventData evtData, object[] args) =>
            {
                startPos = evtData.position;
                if (GetComponent<CanvasGroup>() != null) GetComponent<CanvasGroup>().alpha = 1f;
                if (imgDirBG) imgDirBG.transform.position = evtData.position;
            });
            
            OnClickUp(imgTouch.gameObject, (PointerEventData evtData, object[] args) =>
            {
                if (GetComponent<CanvasGroup>() != null) GetComponent<CanvasGroup>().alpha = 0f;
                if (imgDirBG) imgDirBG.transform.position = defaultPos;
                SetActive(ArrowRoot, false);
                if (imgDirPoint) imgDirPoint.transform.localPosition = Vector2.zero;
                
                // Reset movement direction when releasing joystick
                moveDir = Vector3.zero;
            });
            
            OnDrag(imgTouch.gameObject, (PointerEventData evtData, object[] args) =>
            {
                Vector2 joystickDir = evtData.position - startPos;
                float length = joystickDir.magnitude;
                
                // Handle joystick point position
                if (length > pointDis)
                {
                    Vector2 clampDir = Vector2.ClampMagnitude(joystickDir, 200);
                    if (imgDirPoint) imgDirPoint.transform.position = startPos + clampDir;
                }
                else
                {
                    if (imgDirPoint) imgDirPoint.transform.position = evtData.position;
                }

                if (joystickDir != Vector2.zero)
                {
                    SetActive(ArrowRoot);
                    float angle = Vector2.SignedAngle(new Vector2(1,0), joystickDir);
                    ArrowRoot.localEulerAngles = new Vector3(0, 0, angle + 180);
                    // if (playerObjTrans) playerObjTrans.transform.localEulerAngles = new Vector3(0f, -(angle - 90f), 0f);
                    // -- Rotate Player
                    if (PlayerManager.Instance) PlayerManager.Instance.RotatePlayer(angle); 
                    
                    
                    //-- Convert 2D joystick input to 3D world movement
                    //-- Joystick X -> World X, Joystick Y -> World Z
                    Vector2 normalizedJoystickDir = joystickDir.normalized;
                    moveDir = new Vector3(normalizedJoystickDir.x, 0f, normalizedJoystickDir.y);
                }
                else
                {
                    moveDir = Vector3.zero;
                }
                
            });
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        public Vector3 GetMoveAxis() => moveDir;

        #endregion

    }
}

