using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using TankWars.Utility;
using UnityEngine;

namespace TankWars.Controllers
{
	
	/// <summary>
	/// This class controls all aspects of the camera, such as movement, rotation and offsetting when desired.
	/// </summary>
	
	public class CameraController : MonoBehaviour 
	{
		
	    #region Properties
		
	    #region Cached Components
	    // The storage of components for easy access.
            
            
	    /// <summary>
	    /// Cached 'Camera' component.
	    /// </summary>

	    public Camera Camera
	    {
		    get
		    {
			    if (_camera != null) return _camera;

			    _camera = GetComponentInChildren<Camera>();
			    ExtensionsLibrary.CheckComponent(_camera, "Camera Component", name);
			    return _camera;
		    }
	    }
	    private Camera _camera;
            
	    #endregion	    
	    
	    
	    
	    #region Targeting
	    
	    /// <summary>
	    /// This is the current target of the camera.
	    /// </summary>

	    public Transform Target
	    {
		    get
		    {
			    if(cameraTarget == null) Debug.LogError("Camera Controller: No camera target has been set.");
			    
			    return cameraTarget;
		    }
		    set => cameraTarget = value;
	    }
	    public Transform cameraTarget;
        
        /// <summary>
        /// The character's rigidbody rotation.
        /// </summary>

        private Quaternion Rotation
        {
	        get => transform.rotation;
	        set => transform.rotation = value;
        }

        /// <summary>
        /// The holders transform (the game object that holds the camera).
        /// </summary>

        public Transform CameraTransform
        {
	        get
	        {
		        // Check if exists.
		        if (_cameraTransform) return _cameraTransform;
		        
		        // If not, attempt to find camera within children.
		        _cameraTransform = Camera.transform;

		        // Check if exists.
		        if (_cameraTransform) return _cameraTransform;
		        
		        Debug.LogError("Camera Controller: A camera can not be found among the children of this gameObject.");
		        return null;
	        }
        }
        private Transform _cameraTransform;
        
        /// <summary>
        /// The keyboard key to unlock the mouse cursor.
        /// </summary>
        
        public KeyCode UnlockCursorKey
        {
	        get => unlockCursorKey;
	        set => unlockCursorKey = value;
        }
        [SerializeField] private KeyCode unlockCursorKey = KeyCode.Escape;
		
        #endregion
        
		

		#region Movement

		/// <summary>
		/// The method used to follow the target.
		/// </summary>
		
		public enum FollowMethod
		{
			Lerp,
			SmoothDamp, 
		}
		public FollowMethod followMethod = FollowMethod.SmoothDamp;

		/// <summary>
		/// Get the current screen offset in use.
		/// </summary>

		private Vector2 ScreenOffset => farLook ? FarLookOffset : transform.TransformDirection(PositionOffset).xy();
		
		/// <summary>
		/// Offsets the cameras position relative to the target.
		/// </summary>
		
		public Vector2 PositionOffset
		{
			get => positionOffset;
			set
			{
				positionOffset.x = Mathf.Clamp(value.x, -10.0f, 10.0f);
				positionOffset.y = Mathf.Clamp(value.y, -10.0f, 10.0f);
			}
		}
		[SerializeField] private Vector2 positionOffset = Vector2.zero;

		
		/// <summary>
		/// Offsets the screen according to where the mouse is.
		/// </summary>
		
		public Vector2 FarLookOffset
		{
			get => farLookOffset;
			set
			{
				farLookOffset.x = Mathf.Clamp(value.x, -15, 15);
				farLookOffset.y = Mathf.Clamp(value.y, -15, 15);
			}
		}
		[SerializeField] private Vector2 farLookOffset = Vector2.zero;
		
		/// <summary>
		/// The distance the camera is from the tank.
		/// </summary>
		
		public float CameraDistance
		{
			get => cameraDistance;
			set
			{
				cameraDistance = value;
				Camera.orthographicSize = cameraDistance;
			}
		}
		[SerializeField] private float cameraDistance = 30.0f;


		#endregion

		#endregion



		#region Fields

		#region Unity Inspector

		[HideInInspector] public bool[] hideSection = new bool[2];

		#endregion
		
		
		
		#region Input

		// Mouse axes (names that are within the input manager).
		public string joystickHorizontalInput = "Right Joystick Horizontal";
		public string joystickVerticalInput = "Right Joystick Vertical";

		// Inverts the input on select axis.
		public bool joystickInvertHorizontal;
		public bool joystickInvertVertical;

		// This is the dead zone threshold of the joystick, if the joystick input is below this threshold,
		// no input is applied.
		public float deadZoneThreshold = 0.2f;
		
		// Changes the screen offset from positional to mouse.
		public bool farLook;
		public string farLookInput = "Far Look";
		public float farLookSpeed = 10.0f;
		public float farLookDistance = 15.0f;
		
		#endregion

		
		
		// The tanks transform, used to reset the camera to look at tank.
		public TankController tankController;

		// When enabled the character will rotate will the camera.
		public bool rotateWithTarget;

		// Smoothly moves the characters rotation toward the target rotation. 
		public bool smoothRotation = true;
		
		// Applies a drag to the camera, making it take longer to reach the target rotation.
		public float smoothRotationFactor = 2.5f;
		
		// Displays whether the mouse cursor is locked to the screen or not.
		public bool lockCursor;
		
		// The velocity at which the camera is following the target.
		private Vector2 _velocity;
		
		// If enabled, the cameras movement toward the target will be dampened.
		public bool smoothMovement = true;
		
		// The speed at which the camera will be linearly interpolated toward the camera.
		public float lerpMovementSpeed = 25f;

		// The amount time that the camera will lag behind the target.
		public float smoothMovementTime = 0.25f;

		// This is the current position of the camera.
		private Vector2 _currentPosition;
		
		// The previous mouse position.
		private Vector2 _mousePosition;
		
		#endregion

		
		
		#region Functions

		/// <summary>
		/// Handles user input.
		/// </summary>

		private void HandleInput()
		{
			if(tankController != null && (tankController.pause || tankController.HaltMovement)) return;
			
			// Acquire the far look input.
			if (Input.GetButtonDown(farLookInput))
			{
				// Get the mouse input for later use.
				_mousePosition = Input.mousePosition;
				
				// Enable far look.
				farLook = true;
			}
			
			// On far look disable,
			if (Input.GetButtonUp(farLookInput))
			{
				// Reset the look offset.
				FarLookOffset = Vector2.zero;
				
				// Disable far look.
				farLook = false;
			}

			// If far look is not enabled, we don't need to go any further.
			if (!farLook) return;
			
			// Acquire mouse or joystick input for use of far look.
			var lookOffset = GetFarLookInput() * farLookDistance;
			FarLookOffset = Vector2.Lerp(FarLookOffset, lookOffset, Time.deltaTime * farLookSpeed);
		}

		/// <summary>
		/// Gets the far look input.
		/// </summary>
		
		private Vector2 GetFarLookInput()
		{
			// Check for any mouse input through using the earlier input.
			var mousePosition = Input.mousePosition.xy() - _mousePosition;

			// If no input has been found, the joystick will be used instead.
			var useMouse = mousePosition != Vector2.zero;

			// Else, find the world point, direction and normalize it so it is between 1 & 0.
			if(useMouse)
			{
				mousePosition = 
					(Camera.ScreenToWorldPoint(
						(_mousePosition + mousePosition)).xy() - Target.position.xy()).normalized;

				return mousePosition;
			}

			// Acquire joystick horizontal input.
			var horizontal = Input.GetAxisRaw(joystickHorizontalInput);
			var vertical = Input.GetAxisRaw(joystickVerticalInput);
            
			// If not moving the joystick or the joystick is extremely lightly pushed return zero.
			if (Mathf.Abs(horizontal) < deadZoneThreshold) horizontal = 0;
			if (Mathf.Abs(vertical) < deadZoneThreshold) vertical = 0;
            
			// Else, return the joystick input.
			horizontal = joystickInvertHorizontal ? -horizontal : horizontal;
			vertical = joystickInvertVertical ? -vertical : vertical;
			
			// Return the final joystick position.
			return new Vector2(horizontal, vertical);
		}
		
		/// <summary>
		/// Updates the cameras rotation.
		/// </summary>
		
		private void HandleRotation()
		{
			var targetRotation = rotateWithTarget ? Target.rotation : Quaternion.identity;
			
			// If smoothing is enabled, smoothly move the characters rotation toward the target rotation.
			if (smoothRotation)
			{
				// Calculate smooth time and update the cameras rotation.
				var smoothTime = smoothRotationFactor * Time.deltaTime;
				Rotation = Quaternion.Slerp(Rotation, targetRotation, smoothTime);
			}
            
			// If smoothing is not enabled, simply rotate the character accordingly.
			else Rotation = targetRotation;
		}

		/// <summary>
		/// Sets the cursor lock (eg. hides the cursor)
		/// </summary>
		/// <param name="value">True: Hides cursor.
		///                     False: Shows cursor.</param>

		public void SetCursorLock(bool value)
		{
			lockCursor = value;
			Cursor.lockState = !value ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = !value;
		}


		/// <summary>
		/// Moves the camera toward the target at select rate.
		/// </summary>
		
		private void FollowTarget()
		{
			// If no target is set, return.
			if (Target == null) return;
			
			// Apply an offset to the target position and ensure the rotation is the same as the target.
			var targetPosition = Target.position.xy() + ScreenOffset;

			// Smoothly follow the target either through lerp or through smooth dampening.
			if (smoothMovement)
			{
				_currentPosition = followMethod == FollowMethod.Lerp
					? Vector2.Lerp(_currentPosition, targetPosition, Time.deltaTime * lerpMovementSpeed)
					: Vector2.SmoothDamp(_currentPosition, targetPosition, ref _velocity, smoothMovementTime);
			}
			
			// If smoothing is disabled, simply replace the current position.
			else _currentPosition = targetPosition;

			// Apply the calculated position to the transform.
			transform.position = _currentPosition;
		}

		/// <summary>
		/// Resets the target as tank.
		/// </summary>
		
		public void ResetTarget() => Target = tankController.transform;
		
		///<summary>
		/// Completely resets the camera.
		/// </summary>
		
		public void Reset()
		{
			if (Target == null) return;
			
			_currentPosition = Target.position.xy() + PositionOffset;
			if(rotateWithTarget) transform.rotation = Target.rotation;
		}

		#endregion


		
		#region Monobehaviour

		// Ensure the camera is pointing the right direction.			
		private void Start() => Reset();

		// Handle Input.
		private void Update() => HandleInput();

		// Handle the cameras rotation when rotating with the tank.
		private void FixedUpdate() => HandleRotation();
		
		// Update the camera position so it follows the player.
		private void LateFixedUpdate() => FollowTarget();
        
		private void OnEnable()
		{
			StartCoroutine(nameof(RunLateFixedUpdate));
			Reset();
		}

		private void OnDisable()
		{
			StopCoroutine(nameof(RunLateFixedUpdate));
		}
		
		/// <summary>
		/// Perform the Unity LateFixedUpdate Pattern to produce a late fixed update.
		/// More information: visit https://gist.github.com/capnslipp/acfec93b5763901d6893.
		/// </summary>
		
		private IEnumerator RunLateFixedUpdate()
		{
			while (true)
			{
				yield return new WaitForFixedUpdate();
				LateFixedUpdate();
			}
		}

		#endregion
		
	}
}
