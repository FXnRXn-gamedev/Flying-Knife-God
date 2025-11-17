using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using FXnRXn.ObjectPool;
using NaughtyAttributes;


namespace FXnRXn
{
    public class ExpPrefab : MonoBehaviour
    {
        #region Singleton
		public static ExpPrefab Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}

		#endregion

        #region Properties
        [Header("Settings")] [HorizontalLine(color: EColor.Green)] 
        [SerializeField] private bool isRotating = true;
        [ShowIf("isRotating")][SerializeField] private float RotateYSpeed;
        public int expNum { get; set; }
        [SerializeField] private float moveArcDistance = 4f;
        [SerializeField] private float moveArcDuration = 2f;

        private bool isFollowArc = false;
        private Transform _playerTrans;
        private Transform _playerHeadTrans;

        #endregion

        #region Unity Callbacks

        public void InitData()
        {
	        if(PlayerManager.Instance != null) _playerTrans = PlayerManager.Instance.transform;
	        if(PlayerManager.Instance.GetAnimator() != null) _playerHeadTrans = PlayerManager.Instance.GetAnimator().GetBoneTransform(HumanBodyBones.Head);
	        isFollowArc = false;
        }

        private void Update()
        {
	        if (isRotating) transform.Rotate(Vector3.up, RotateYSpeed * Time.deltaTime);

	        if (Vector3.Distance(transform.position, PlayerManager.Instance.transform.position) <= moveArcDistance && !isFollowArc)
	        {
		        MoveInArc();
	        }
        }
        
        #endregion

        #region Custom Method
        
        private async UniTaskVoid MoveInArc()
        {
	        isFollowArc = true;
	        Vector3 startPosition = transform.position;
	        float elapsedTime = 0f;

	        while (elapsedTime < moveArcDuration)
	        {
		        if (_playerHeadTrans == null) return;
		        
		        elapsedTime += Time.deltaTime;
		        float t = Mathf.Clamp01(elapsedTime / moveArcDuration);
		        float arcHeight = 5f;
		        // Linear interpolation for the horizontal position
		        Vector3 linearPos = Vector3.Lerp(startPosition, _playerHeadTrans.position, t);

		        // Calculate vertical offset using a sine wave for a smooth arc
		        float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;

		        // Apply the arc height to the vertical position
		        transform.position = new Vector3(linearPos.x, linearPos.y + arc, linearPos.z);
		        
		        await UniTask.Yield();
	        }
	        
	        if (_playerHeadTrans != null)
	        {
		        if (GameManager.Instance != null)
		        {
			        GameManager.Instance.AddPlayerExp(expNum);
		        }
		        transform.position = _playerHeadTrans.position;
		        if (ExpPool.Instance != null)
		        {
			        PoolManager.Instance.Return(transform);
		        }
	        }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper

        #endregion
    
    }
}

