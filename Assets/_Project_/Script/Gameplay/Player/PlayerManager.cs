using UnityEngine;
using System;
using NaughtyAttributes;
using Tayx.Graphy.Utils.NumString;


namespace FXnRXn
{
    public class PlayerManager : MonoBehaviour
    {
        #region Singleton
		public static PlayerManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance == null) Instance = this;
			OnAwake();
		}

		#endregion
		
		#region Animation Hashes
		private static readonly int CurrentSpeedHash = Animator.StringToHash("CurrentSpeed");
		#endregion

        #region Properties
        [Header("Movement Settings")]
        [HorizontalLine(color: EColor.Green)]
        [SerializeField] private float moveSpeed = 5f;
        
        [Header("Gravity Settings")]
        [HorizontalLine(color: EColor.Green)]
        [SerializeField] private bool wantGravity = false;
        [ReadOnly] [SerializeField] private float gravity = -20f;
        [ReadOnly] [SerializeField] private float jumpHeight = 2f;
        
        [Header("Player Stat Settings")]
        public float currentHP { get; set; }
        public float maxHP { get; set; } = 100f;
        public int currentLevel { get; set; } = 1;
        public int currentExp { get; set; }
        public int nextExp { get; set; } = 10;

        [Header("Player Vfx Settings")]
        [HorizontalLine(color: EColor.Green)]
        [SerializeField] private ParticleSystem runVfx;
        
        // Private Fields
        private Animator _animator;
        private CharacterController _characterController;

        private Vector3 _playerVelocity;
	        

        #endregion

        #region Unity Callbacks

        private void OnAwake()
        {
	        if (_animator == null) _animator = GetComponentInChildren<Animator>();
	        if (_characterController == null) _characterController = GetComponent<CharacterController>();
	        currentHP = maxHP;
        }

        private void Update()
        {
	        if (JoyStickController.Instance != null)
	        {
		        Move(JoyStickController.Instance.GetMoveAxis());
	        }
        }

        #endregion

        #region Custom Method

        public void Move(Vector3 direction) //-- Update called from PlayWindow script
        {
	        Vector3 moveDir = new Vector3(direction.x, 0f, direction.z);
	        if (moveDir.magnitude >= 1f) moveDir.Normalize();
	        
	        HandleGravity();
	        if (wantGravity)
	        {
		        // Separate horizontal and vertical movement
		        Vector3 horizontalMovement = moveDir * moveSpeed * Time.deltaTime;
		        Vector3 verticalMovement = new Vector3(0, _playerVelocity.y, 0) * Time.deltaTime;
		        _characterController.Move(horizontalMovement + verticalMovement);
	        }
	        else
	        {
		        _characterController.Move(moveDir * moveSpeed * Time.deltaTime);
	        }
	        
	        
	        //-- Animate the player (Idle/Run)
	        _animator.SetFloat(CurrentSpeedHash, moveDir.magnitude);
	        
	        //-- Handle run VFX based on movement speed
	        HandleRunVfx(moveDir.magnitude);
        }

        private void HandleRunVfx(float speed)
        {
	        if(runVfx == null) return;
	        // Consider "running" if speed is above a small threshold
	        const float runThreshold = 0.1f;

	        if (speed > runThreshold)
	        {
		        if (!runVfx.isPlaying)
		        {
			        runVfx.Play();
		        }
	        }
	        else
	        {
		        if (runVfx.isPlaying)
		        {
			        runVfx.Stop();
		        }
	        }
        }

        private void HandleGravity()
        {
	        // Check if the character is grounded
	        if (_characterController.isGrounded && _playerVelocity.y < 0)
	        {
		        // Reset vertical velocity to a small negative value to keep the character on the ground
		        _playerVelocity.y = -2f; 
	        }
	        _playerVelocity.y += gravity * Time.deltaTime;
        }
        
        public void RotatePlayer(float angle)
        {
	        if (angle != 0f)
	        {
		        transform.localEulerAngles = new Vector3(0f, -(angle - 90f), 0f);
	        }
        }

        public void Jump()
        {
	        if ( _characterController.isGrounded) //Input.GetKeyDown(KeyCode.Space) &&
	        {
		        // Calculate the velocity needed to achieve a specific jump height
		        _playerVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
	        }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------

        #region Helper
		public Animator GetAnimator() => _animator;
		public int GetCurrentLevel => currentLevel;
		public int GetCurrentExp => WorldData.GetPlayerExp();
		public int GetNextExp => nextExp;

		#endregion

    }
}

