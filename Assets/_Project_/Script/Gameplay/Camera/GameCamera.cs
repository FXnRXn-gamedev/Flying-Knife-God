using UnityEngine;
using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;


namespace FXnRXn
{
    public class GameCamera : MonoBehaviour
    {
        #region Singleton
		public static GameCamera Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
			if(mainCamera == null) mainCamera =GetComponentInChildren<Camera>();
			if(gameCamera == null) gameCamera = transform.GetChild(0);
		}

		#endregion

        #region Properties
        [Header("Refference")]
        [HorizontalLine(color: EColor.Green)]
        [ReadOnly] [SerializeField] private GameObject						mainCharacter;
        [ReadOnly] [SerializeField] private Camera							mainCamera;
        
        [Header("Camera Settings")]
        [HorizontalLine(color: EColor.Green)]
        [ReadOnly] [SerializeField] private Transform						playerTarget;
        [ReadOnly] [SerializeField] private Transform						lockOnTarget;

        [Space(10)] 
        public bool												isFollowPlayer;
        [SerializeField] private bool							invertCamera;
        [SerializeField] private float							mouseSensitivity			= 5f;
        
        [ReadOnly][SerializeField] private Vector2							cameraTiltBounds			= new Vector2(-10f, 45f);
        [ReadOnly][SerializeField] private float							positionalCameraLag			= 1f;
        [ReadOnly][SerializeField] private float							rotationalCameraLag			= 1f;

        public Action<bool>										MoveEndAction;
        private float											cameraDistance;
        private float											cameraHeightOffset;
        private float											cameraHorizontalOffset;
        private float											cameraTiltOffset;
        private float											cameraInversion;
        private float											lastAngleX;
        private float											lastAngleY;
        private Vector3											lastPosition;
        private float											newAngleX;
        private float											newAngleY;
        private Vector3											newPosition;
        private float											rotationX;
        private float											rotationY;
        private Transform										gameCamera;
        
        
        
        private const int										LAG_DELTA_TIME_ADJUSTMENT = 20;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
	        cameraInversion = invertCamera ? 1 : -1;
	        playerTarget = PlayerManager.Instance.transform;
	        lockOnTarget = PlayerManager.Instance.transform;
	        mainCharacter = PlayerManager.Instance.gameObject;
			
	        transform.position = playerTarget.position;
	        transform.rotation = playerTarget.rotation;
	        lastPosition = transform.position;
			
	        gameCamera.localPosition = new Vector3(cameraHorizontalOffset, cameraHeightOffset, cameraDistance * -1);
	        gameCamera.localEulerAngles = new Vector3(cameraTiltOffset, 0f, 0f);
	        
        }
        
        private void LateUpdate()
		{
			if (isFollowPlayer)
			{
				HandleCameraFollowPlayer();
			}
			
		}

        #endregion

        #region Custom Method

        public void SetCameraType(ECameraType camraType)
        {
	        switch (camraType)
	        {
		        case ECameraType.GameCamera:
			        isFollowPlayer = true;
			        TransitionCameraToTarget(42f, 35f, 1.8f, 36f, 1f, ECameraType.GameCamera, (e) =>
			        {
				        if (GameManager.Instance != null) GameManager.Instance.GameStarted();
			        });
			        
			        break;
		        case ECameraType.GameUICamera:
			        isFollowPlayer = false;
			        cameraDistance = 20f;
			        cameraHeightOffset = 12f;
			        cameraHorizontalOffset = 0f;
			        cameraTiltOffset = 20f;
			       TransitionCameraToTarget(20f, 12f, 0f, 20f, 1f, ECameraType.GameUICamera, (e) =>
			        {
				        
			        });
			        break;
	        }
        }

        private void HandleCameraFollowPlayer()
        {
	        if(playerTarget ==null) return;
			
	        float positionalFollowSpeed = 1 / (positionalCameraLag / LAG_DELTA_TIME_ADJUSTMENT);
	        float rotationalFollowSpeed = 1 / (rotationalCameraLag / LAG_DELTA_TIME_ADJUSTMENT);
			
	        newAngleX += rotationX;
	        newAngleX = Mathf.Clamp(newAngleX, cameraTiltBounds.x, cameraTiltBounds.y);
	        newAngleX = Mathf.Lerp(lastAngleX, newAngleX, rotationalFollowSpeed * Time.deltaTime);

	        newAngleY += rotationY;
	        newAngleY = Mathf.Lerp(lastAngleY, newAngleY, rotationalFollowSpeed * Time.deltaTime);

	        newPosition = playerTarget.position;
	        newPosition = Vector3.Lerp(lastPosition, newPosition, positionalFollowSpeed * Time.deltaTime);

	        transform.position = newPosition;
	        transform.eulerAngles = new Vector3(newAngleX, newAngleY, 0);
			
	        // Calculate target camera local position
	        Vector3 targetLocalPosition = new Vector3(cameraHorizontalOffset, cameraHeightOffset, cameraDistance * -1);
	        // Calculate target camera local Euler angles
	        Vector3 targetLocalEulerAngles = new Vector3(cameraTiltOffset, 0f, 0f);

	        gameCamera.localPosition = new Vector3(cameraHorizontalOffset, cameraHeightOffset, cameraDistance * -1);
	        gameCamera.localEulerAngles = new Vector3(cameraTiltOffset, 0f, 0f);

	        lastPosition = newPosition;
	        lastAngleX = newAngleX;
	        lastAngleY = newAngleY;
        }
        
        private async UniTask TransitionCameraToTarget(float targetDistance, float targetHeightOffset, float targetHorizontalOffset, float targetTiltOffset, float duration, ECameraType type, Action<bool> callback)
        {
	        // Store starting values
	        float startDistance = cameraDistance;
	        float startHeightOffset = cameraHeightOffset;
	        float startHorizontalOffset = cameraHorizontalOffset;
	        float startTiltOffset = cameraTiltOffset;

	        float elapsedTime = 0f;

	        while (elapsedTime < duration)
	        {
		        elapsedTime += Time.deltaTime;
		        float t = elapsedTime / duration;
                
		        // Use smoothstep for smoother interpolation
		        t = t * t * (3f - 2f * t);

		        // Interpolate between start and target values
		        cameraDistance = Mathf.Lerp(startDistance, targetDistance, t);
		        cameraHeightOffset = Mathf.Lerp(startHeightOffset, targetHeightOffset, t);
		        cameraHorizontalOffset = Mathf.Lerp(startHorizontalOffset, targetHorizontalOffset, t);
		        cameraTiltOffset = Mathf.Lerp(startTiltOffset, targetTiltOffset, t);

		        await UniTask.Yield();
	        }

	        // Ensure final values are exactly the target values
	        cameraDistance = targetDistance;
	        cameraHeightOffset = targetHeightOffset;
	        cameraHorizontalOffset = targetHorizontalOffset;
	        cameraTiltOffset = targetTiltOffset;

	        switch (type)
	        {
		        case ECameraType.GameCamera:
			        callback?.Invoke(true);
			        break;
		        case ECameraType.GameUICamera:
			        callback?.Invoke(false);
			        break;
		        
	        }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }

    public enum ECameraType
    {
	    GameCamera,
	    GameUICamera
    }
}

