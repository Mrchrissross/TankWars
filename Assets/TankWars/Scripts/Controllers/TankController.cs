using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TankWars.Managers;
using TankWars.Utility;

namespace TankWars.Controllers
{
    /// <summary>
    /// Controls the movement, style and shooting of the player tank.
    /// </summary>
    
    [RequireComponent(typeof(Rigidbody2D))]
    public class TankController : MonoBehaviour
    {

        #region Classes

        [Serializable]
        public class Weapon
        {
            
            #region Fields

            // The name of the weapon.
            public string name;

            // The type of bullet fired.
            public BulletController.BulletType type;
            
            // Plays when the tanks shoots.
            public ParticleSystem muzzleFlash;

            #endregion
            
            
            #region Properties

            /// <summary>
            /// Time between shots fired in seconds.
            /// </summary>
                
            public Vector2 shotTimer
            {
                get => _shotTimer;
                set
                {
                    _shotTimer.x = Mathf.Max(0.0f, value.x);
                    _shotTimer.y = Mathf.Max(-0.001f, value.y);
                }
            }
            [SerializeField] private Vector2 _shotTimer = new Vector2(0.8f, 0.0f);
            
            /// <summary>
            /// How fast the bullet travels.
            /// </summary>
                
            public float bulletSpeed
            {
                get => _bulletSpeed;
                set => _bulletSpeed = Mathf.Max(20.0f, value);
            }
            [SerializeField] private float _bulletSpeed = 40.0f;
            
            /// <summary>
            /// How much damage the bullet does.
            /// </summary>
                
            public float bulletDamage
            {
                get => _bulletDamage;
                set => _bulletDamage = Mathf.Max(0.0f, value);
            }
            [SerializeField] private float _bulletDamage = 20.0f;
            
            #endregion


            #region Functions

            public Weapon()
            {
                name = "New Weapon";
                type = BulletController.BulletType.Shell;
            }
            
            public Weapon(Weapon copy)
            {
                name = copy.name + " copy";
                type = copy.type;
                muzzleFlash = copy.muzzleFlash;
                shotTimer = copy.shotTimer;
                bulletSpeed = copy.bulletSpeed;
                bulletDamage = copy.bulletDamage;
            }

            #endregion
            
        }
        
        #endregion
        
        
        
        
        
        #region Properties

            #region Cached Components
            // The storage of components for easy access.
            
            private Rigidbody2D _rigidbody;
            
            /// <summary>
            /// Cached 'Rigidbody' component.
            /// </summary>

            private new Rigidbody2D rigidbody
            {
                get
                {
                    if (_rigidbody) return _rigidbody;
                    
                    _rigidbody = GetComponent<Rigidbody2D>();
                    ExtensionsLibrary.CheckComponent(_rigidbody, "Rigidbody Component", name);
                    return _rigidbody;
                }
            }
            
            #endregion
        
            
            
            
            #region Speed Settings
            // The movement settings control all aspects relating to the speed of the tank.

        
            /// <summary>
            /// Speed when moving forward.
            /// </summary>
            
            public float ForwardSpeed
            {
                get => forwardSpeed;
                set => forwardSpeed = Mathf.Max(0.0f, value);
            }
            [SerializeField] private float forwardSpeed = 10.0f;
            
            /// <summary>
            /// Speed when moving backwards.
            /// </summary>
            
            public float BackwardSpeed
            {
                get => backwardSpeed;
                set => backwardSpeed = Mathf.Max(0.0f, value);
            }
            [SerializeField] private float backwardSpeed = 6.0f;

            /// <summary>
            /// Speed when moving sideways.
            /// </summary>
            
            public float TurnSpeed
            {
                get => turnSpeed;
                set => turnSpeed = Mathf.Max(0.0f, value);
            }
            [SerializeField] private float turnSpeed = 5.0f;

            /// <summary>
            /// Speed multiplier while sprinting.
            /// </summary>
            
            public float SpeedMultiplier
            {
                get => speedMultiplier;
                set => speedMultiplier = Mathf.Max(value, 1.0f);
            }
            [SerializeField] private float speedMultiplier = 2.0f;

            /// <summary>
            /// Current movement speed of the character (in m/s).
            /// </summary>
            
            public float Speed { get; set; }
            
            
            
            // ------------------------------- Hidden Properties------------------------------- 

            /// <summary>
            /// This is the characters velocity combined with any platform that the character may be standing on.
            /// </summary>
            
            // ReSharper disable once InconsistentNaming
            public Vector2 velocity
            {
                get => rigidbody.velocity;
                set => rigidbody.velocity = value;
            }

            public float currentRotation
            {
                get => _currentRotation;
                set
                {
                    _currentRotation = value;
                    rigidbody.MoveRotation(_currentRotation);
                }
            }
            private float _currentRotation = 0;

            
            /// <summary>
            /// This is the current move direction magnitude. Used ensure the speed matches that of a joystick. 
            /// </summary>
            
            public float CurrentSpeed
            {
                get => _currentSpeed;
                set => _currentSpeed = Mathf.Max(0.0f, value);
            }
            private float _currentSpeed;
            
            /// <summary>
            /// This is the current directional input from the user.
            /// </summary>
            
            public Vector2 MovementDirection
            {
                get => _movementDirection;
                set => _movementDirection = Vector3.ClampMagnitude(value, 1.0f);
            }
            private Vector2 _movementDirection;
            
            /// <summary>
            /// Halts the players movement input for one frame.
            /// </summary>
            
            public bool HaltMovement
            {
                get => _haltMovement;
                set
                {
                    _haltMovement = value;
                    if (_haltMovement) MovementDirection = Vector2.zero;
                }
            }
            private bool _haltMovement;

            #endregion

            
            
        
            #region Acceleration Settings
            // The acceleration settings control how quickly the character accelerates or decelerates.
            
            [SerializeField] public float acceleration = 50.0f;
            
            /// <summary>
            /// The rate at which the tank accelerates.
            /// </summary>
                
            public float Acceleration
            {
                get => acceleration;
                set => acceleration = Mathf.Max(0.0f, value);
            }

            /// <summary>
            /// The rate at which the tank decelerates and comes to a halt.
            /// </summary>
                
            public float Deceleration
            {
                get => deceleration;
                set => deceleration = Mathf.Max(0.0f, value);
            }
            [SerializeField] public float deceleration = 10.0f;
            
            #endregion
            
            
        
            
            #region Max Speed Settings
            // The movement settings control all aspects relating to the speed of the character.

            /// <summary>
            /// The maximum speed at which the tank can move along the X axis.
            /// This includes all external physics that may be at work (eg. being pushed, recoil, explosion, etc).
            /// </summary>
            
            public float MaxHorizontalSpeed
            {
                get => maxHorizontalSpeed;
                set => maxHorizontalSpeed = Mathf.Max(0.0f, value);
            }
            [SerializeField] private float maxHorizontalSpeed = 100.0f;
            
            /// <summary>
            /// The maximum speed at which the tank will go upward.
            /// This includes all external physics that may be at work (eg. being pushed, recoil, explosion, etc).
            /// </summary>
            
            public float MaxUpwardSpeed
            {
                get => maxUpwardSpeed;
                set => maxUpwardSpeed = Mathf.Max(0.0f, value);
            }
            [SerializeField] private float maxUpwardSpeed = 100.0f;
            
            /// <summary>
            /// The maximum speed at which the tank will go downward.
            /// This includes all external physics that may be at work (eg. being pushed, recoil, explosion, etc).
            /// </summary>
            
            public float MaxDownwardSpeed
            {
                get => maxDownwardSpeed;
                set => maxDownwardSpeed = Mathf.Max(0.0f, value);
            }
            [SerializeField] private float maxDownwardSpeed = 100.0f;

            #endregion
            
            
            
            
            #region Friction Settings
        
            /// <summary>
            /// This is how much grip the character has while grounded.
            /// </summary>
            
            public Vector2 Friction
            {
                get => friction;
                set
                {
                    friction.x = Mathf.Max(0.0f, value.x);
                    friction.y = Mathf.Max(0.0f, value.y);
                }
            }
            [SerializeField] private Vector2 friction = new Vector2(10, 2.0f);

            #endregion
            
        #endregion

        
        
        
        
        #region Fields

        #region Input settings

        // Keyboard Input (names that are within the input manager).
        public string keyboardHorizontalInput = "Horizontal";
        public string keyboardVerticalInput = "Vertical";
        
        // Inverts the input on select axis.
        public bool keyboardInvertHorizontal;
        public bool keyboardInvertVertical;

        // Joystick Input (names that are within the input manager).
        public string joystickHorizontalInput = "Joystick Horizontal";
        public string joystickVerticalInput = "Joystick Vertical";
	    
        // Inverts the input on select axis.
        public bool joystickInvertHorizontal;
        public bool joystickInvertVertical;

        // Button to apply the speed multiplier.
        private bool _accelerate;
        public string accelerateInput = "Accelerate";
        
        // Button to shoot from the main (middle) cannon.
        private bool _mainShoot;
        public string mainCannonFire = "Main Shoot";
        
        // Button to shoot from the left cannon.
        private bool _leftShoot;
        public string leftCannonFire = "Left Shoot";
        
        // Button to shoot from the left cannon.
        private bool _rightShoot;
        public string rightCannonFire = "Right Shoot";

        #endregion
        
        
        // This is the dead zone threshold of the joystick, if the joystick input is below this threshold, no input is applied.
        public float deadZoneThreshold = 0.2f;
        
        // The movement settings control all aspects relating to the speed of the character.
        public bool limitMaximumSpeed = true;

        // Pauses all tank components.
        public bool pause;
        
        // The characters cached layer to use when ignore raycasts is applied.
        private int _cachedLayer;
        // Stored ignore raycast layer.
        private int _ignoreLayer;

        public Transform hull;
        public Transform cannonRotor;
        public List<Transform> firePoints = new List<Transform>();
        public List<Weapon> weapons = new List<Weapon>();

        // [Header("Health: "), Range(0.0f, 100.0f)] public int playerHealth = 100;
        
        #endregion

        
        
        

        #region Functions

        /// <summary>
        /// Handles user input.
        /// </summary>

        private void HandleInput()
        {
            if (pause || HaltMovement) return;
            
            // Acquire Inputs.
            MovementDirection = new Vector2(GetHorizontalInput(),GetVerticalInput());
            _accelerate = Input.GetButton(accelerateInput);
            
            // Apply constraints to the acquired input.
            var x = Mathf.Abs(MovementDirection.x);
            var y = Mathf.Abs(MovementDirection.y);
            
            if (x > y) MovementDirection = MovementDirection.WithY(0);
            else if (y > x) MovementDirection = MovementDirection.WithX(0);
            else if (x > 0.69f && y > 0.69f) MovementDirection = MovementDirection.WithY(0) * 1.4285f;  
        }
        
        /// <summary>
        /// Handles the users horizontal input.
        /// </summary>
		
        private float GetHorizontalInput()
        {
            // Acquire the keyboards horizontal input.
            var horizontal = Input.GetAxisRaw(keyboardHorizontalInput);
            
            // If currently moving with the keyboard, return the input.
            if (Mathf.Abs(horizontal) > 0) 
                return keyboardInvertHorizontal ? -horizontal : horizontal;
	        
            // Acquire joystick horizontal input.
            horizontal = Input.GetAxisRaw(joystickHorizontalInput);
            
            // If not moving the joystick or the joystick is extremely lightly pushed return zero.
            if (Mathf.Abs(horizontal) < deadZoneThreshold) return 0;
            
            // Else, return the joystick input.
            return joystickInvertHorizontal ? -horizontal : horizontal;
        }

        /// <summary>
        /// Handles the users vertical input.
        /// </summary>
		
        private float GetVerticalInput()
        {
            // Acquire the keyboards vertical input.
            var vertical = Input.GetAxisRaw(keyboardVerticalInput);
	        
            // If currently moving with the keyboard, return the input.
            if (Mathf.Abs(vertical) > 0) 
                return keyboardInvertVertical ? -vertical : vertical;
	        
            // Acquire joystick vertical input.
            vertical = Input.GetAxisRaw(joystickVerticalInput);

            // If not moving the joystick or the joystick is extremely lightly pushed return zero.
            if (Mathf.Abs(vertical) < deadZoneThreshold) return 0;
            
            // Else, return the joystick input.
            return joystickInvertVertical ? -vertical : vertical;
        }
        
        /// <summary>
        /// Perform character movement.
        /// </summary>

        private void ProcessInput()
        {
            // Performs character movement.
            UpdateVelocity();
            
            // Jump logic
            // PerformJumpLogic();
        }



        #region Movement Logic

        /// <summary>
        /// Generates a velocity by multiplying the movement direction with the input speed.
        /// </summary>
        
        private Vector2 InputToVelocity(out float targetSpeed)
        {
            // Acquire the length of the movement direction.
            CurrentSpeed = MovementDirection == Vector2.zero ? 0.0f : MovementDirection.magnitude;
            
            // Default the target speed to zero.
            targetSpeed = 0;
            
            // If moving along the x-axis, default to turn speed.
            if(Mathf.Abs(MovementDirection.x) > 0) targetSpeed = turnSpeed * CurrentSpeed;
            
            // Multiply a speed multiplier when accelerating (only applicable while moving forward or backward).
            if (_accelerate) CurrentSpeed *= speedMultiplier;
            
            // If moving backwards, set the target speed to backward.
            if (MovementDirection.y < 0.0f) targetSpeed = backwardSpeed * CurrentSpeed;

            // The forward speed should be the quickest so it overwrites the target speed last.
            else if (MovementDirection.y > 0.0f) targetSpeed = forwardSpeed * CurrentSpeed;
            
            // If necessary, override the target speed.
            OverrideInput(ref targetSpeed);
            
            // Update the speed displayed in the inspector.
            Speed = targetSpeed;
            
            // Multiply the direction with speed to acquire the new velocity.
            var velocity = MovementDirection * targetSpeed;
            
            // Transform the newly acquired local velocity's direction from local to world.
            velocity = transform.TransformDirection(velocity);
            
            // Remove the y axis from the equation as this will be controlled by gravity or through jumping.
            return velocity;
        }

        /// <summary>
        /// Overrides the users input, to produce a new speed.
        /// </summary>
        /// <param name="targetSpeed">The current users input speed.</param>
        
        private void OverrideInput(ref float targetSpeed)
        {
            if (targetSpeed.IsZero()) return;

            // Override speed.
        }
        
        
        
        /// <summary>
        /// Updates the characters velocity by applying movement, gravity, and limits.
        /// </summary>

        private void UpdateVelocity()
        {
            var currentVelocity = velocity;
            var deltaTime = Time.deltaTime;

            // Calculate the target velocity.
            var targetVelocity = InputToVelocity(out var targetSpeed);
            
            // Apply movement to the tank.
            ApplyMovement(ref currentVelocity, targetVelocity, targetSpeed, deltaTime);
            
            // Apply limits to the velocity.
            LimitVelocity(ref currentVelocity);
            
            // Finally, apply the new velocity to the rigidbody. 
            if(!currentVelocity.IsNaN()) velocity = currentVelocity;
        }

        /// <summary>
        /// Perform an accelerated friction based movement.
        /// </summary>
        /// <param name="currentVelocity">The current velocity of the character.</param>
        /// <param name="targetVelocity">The target velocity of the character.</param>
        /// <param name="targetSpeed">This is the speed to aim for.</param>
        /// <param name="deltaTime">The completion time in seconds since the last frame.</param>

        private void ApplyMovement(ref Vector2 currentVelocity, Vector2 targetVelocity, float targetSpeed, float deltaTime)
        {
            // Apply torque to the tanks rigidbody for rotation.        
            var torque = -MovementDirection.x * Mathf.Clamp01(1f - Friction.x * deltaTime);
            currentRotation += torque * targetSpeed;
            
            // if turning, remove all extra velocity.
            if(Mathf.Abs(MovementDirection.x) > 0) targetVelocity = Vector2.zero; 
            
            // If in the previous fixed frame the character was grounded, assign the whole velocity.
            var newVelocity = currentVelocity;
            var targetDirection = targetVelocity / targetSpeed;
            
            // Calculate the appropriate acceleration and combine the direction.
            var targetAcceleration = targetDirection * (Acceleration * deltaTime);
            
            // Character is decelerating.
            if (targetAcceleration.IsZero() || newVelocity.IsExceeding(targetSpeed))
            {
                // Find appropriate friction and apply braking friction clamped between zero and one.
                newVelocity *= Mathf.Clamp01(1f - Friction.y * deltaTime);

                // Retrieve the appropriate deceleration value and apply it.
                newVelocity = Vector3.MoveTowards(newVelocity, targetVelocity, Deceleration * deltaTime);
            }
            
            // Character is accelerating.
            else
            {
                // Find appropriate friction and apply it.
                newVelocity -= (newVelocity - targetDirection * newVelocity.magnitude) * Mathf.Min(Friction.x * deltaTime, 1.0f);

                // Apply acceleration while also clamping its length to the target speed.
                newVelocity = Vector3.ClampMagnitude(newVelocity + targetAcceleration, targetSpeed);
            }
            
            // Update the reference to the character's velocity.
            currentVelocity += newVelocity - currentVelocity;
        }
        
        /// <summary>
        /// Applies limits the horizontal and vertical velocity of the tank to ensure it does not propel
        /// too fast in one direction.
        /// </summary>

        private void LimitVelocity(ref Vector2 currentVelocity)
        {
            // If not limiting speed, return.
            if (!limitMaximumSpeed) return;
            
            // Split the velocity between horizontal and vertical.
            var horizontalVelocity = currentVelocity.x; 
            var verticalVelocity = currentVelocity.y;

            // Apply left and right limits.
            if (horizontalVelocity > MaxHorizontalSpeed) 
                currentVelocity += Vector2.zero.WithX(MaxHorizontalSpeed - horizontalVelocity);
            else if (horizontalVelocity < -MaxHorizontalSpeed)
                currentVelocity += Vector2.zero.WithX(-MaxHorizontalSpeed - horizontalVelocity);
            
            // Apply upward and downward limits.
            if (verticalVelocity > MaxUpwardSpeed) 
                currentVelocity += Vector2.zero.WithY(MaxUpwardSpeed - verticalVelocity);
            else if (verticalVelocity < -MaxDownwardSpeed)
                currentVelocity += Vector2.zero.WithY(-MaxDownwardSpeed - verticalVelocity);
        }

        #endregion

        // /// <summary>
        // /// Ensures the main camera stays on top of the player tank.
        // /// </summary>
        // void UpdateCamera()
        // {
        //     Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
        // }
        //
        // /// <summary>
        // /// Rotates the players cannon to the direction of the mouse.
        // /// </summary>
        // void RotateCannon()
        // {
        //     Vector3 mousePos = Input.mousePosition;                                                                        
        //     Vector3 screenPos = Camera.main.WorldToScreenPoint(cannonRotor.position);                              
        //     Vector3 offset = new Vector3(mousePos.x - screenPos.x, mousePos.y - screenPos.y);   
        //
        //     float angle = Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg;                      
        //     cannonRotor.rotation = Quaternion.AngleAxis(angle, Vector3.back);                        
        // }
        //
        // /// <summary>
        // /// Shoots when the player clicks the left mouse button.
        // /// </summary>
        // void Shoot()
        // {
        //     if(shotTimer.y > 0f) shotTimer.y -= Time.deltaTime;
        //
        //     if (!Input.GetMouseButtonDown(0) || !(shotTimer.y < Mathf.Epsilon)) return;
        //     
        //     var bullet = AssetManager.Instance.SpawnObject("Shoot", firePoint.position, firePoint.rotation, true);                 
        //     var newBullet = bullet.transform.GetComponent<BulletController>();
        //     newBullet.BulletSpeed = bulletSpeed;                                                                                    
        //     newBullet.BulletDamage = bulletDamage;                                                                                  
        //     muzzleFlash.Play();                                                                                                     
        //     Recoil();                                                                                                               
        //     shotTimer.y = shotTimer.x;
        // }
        //
        // /// <summary>
        // /// Recoils the tank in the opposite direction to the shot.
        // /// </summary>
        // void Recoil()
        // {
        //     if((cannonRotor.localEulerAngles.z > 0.0f && cannonRotor.localEulerAngles.z < 43.0f) || (cannonRotor.localEulerAngles.z > 318.0f && cannonRotor.localEulerAngles.z < 360.0f))
        //         rb.AddForce(transform.up * -extra.y, ForceMode2D.Impulse);
        //
        //     else if (cannonRotor.localEulerAngles.z > 150.0f && cannonRotor.localEulerAngles.z < 210.0f)
        //         rb.AddForce(transform.up * extra.y, ForceMode2D.Impulse);
        // }
        //
        // // /// <summary>
        // // /// Performs a health check on the tank.
        // // /// </summary>
        // // void CheckHealth()
        // // {
        // //     if (playerHealth <= 0f)                                                                                     
        // //     {
        // //         Destroy(gameObject);                                                                                    
        // //         AssetManager.Instance.SpawnObject("TankExplosion", transform.position, transform.rotation);                             
        // //         GameObject destroyedTank = AssetManager.Instance.SpawnObject("DestroyedPlayerTank", transform.position, transform.rotation); 
        // //         destroyedTank.name = "DeadPlayer";
        // //     }
        // // }
        //
        // /// <summary>
        // /// Moves the player tank in the desired direction.
        // /// </summary>
        // void Move()
        // {
        //     rb.velocity = transform.up * Vector2.Dot(rb.velocity, transform.up);    
        //
        //     if (Input.GetButton("Accelerate"))                                      
        //         rb.AddForce(transform.up * (slow ? extra.z : vertical.x));                                  
        //
        //     if (Input.GetButton("Reverse"))                                         
        //         rb.AddForce(transform.up * (slow ? extra.z : -vertical.y));                          
        //
        //     rb.AddTorque(Input.GetAxis("Left/Right") * (slow ? (-extra.x / 2) : -extra.x));                
        // }

        public void CheckDependancies()
        {
            if (hull != null && cannonRotor != null) return;
            
            Debug.LogError("Tank Controller: Please build a tank using the Tank Builder component on an empty game object.");
            DestroyImmediate(this);
        }

        #endregion

        
        
        
        
        #region MonoBehaviour

        private void Update()
        {
            // Handles the user input
            HandleInput();
        }
        
        private void FixedUpdate()
        {
            // Processes the users input by moving the character
            ProcessInput();
        }
        
        
        // private void Update()
        // {
        //     UpdateCamera();
        //     RotateCannon();
        //     Shoot();
        //     CheckHealth();
        // }
        //
        // private void FixedUpdate()
        // {
        //     Move();
        // }
        //
        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     switch (other.gameObject.tag)
        //     {
        //         case "Mine":
        //             {
        //                 playerHealth = 0;
        //                 AssetManager.Instance.SpawnObject("MineExplosion", other.transform.position, other.transform.rotation);
        //                 Destroy(other.gameObject);
        //                 break;
        //             }
        //         case "DestroyedTank":
        //             {
        //                 slow = true;
        //                 break;
        //             }
        //     }
        // }
        //
        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     switch (other.gameObject.tag)
        //     {
        //         case "DestroyedTank":
        //             {
        //                 slow = false;
        //                 break;
        //             }
        //     }
        // }

        #endregion
    }
}
