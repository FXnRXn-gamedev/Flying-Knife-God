using UnityEngine;
using System;
using NaughtyAttributes;


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
        [HorizontalLine(color: EColor.Green)]
        [ReadOnly] [SerializeField] private float currentHP;
        [SerializeField] private float maxHP = 100f;
        [ReadOnly] [SerializeField] private float currentLevel;
        [ReadOnly] [SerializeField] private float currentExp;
        [ReadOnly] [SerializeField] private float nextExp;

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

	        _animator.SetFloat(CurrentSpeedHash, moveDir.magnitude);
	        // -- Animate the player (Idle/Run)
	        // if (moveDir != Vector3.zero)
	        // {
		       //  _animator.SetFloat(CurrentSpeedHash, moveDir.magnitude);
	        // }
	        // else
	        // {
		       //  _animator.SetFloat(CurrentSpeedHash, 0f);
	        // }
	        
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
        #endregion
        
    }
}

