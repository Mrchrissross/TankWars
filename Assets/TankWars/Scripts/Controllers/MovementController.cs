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
    
    public class MovementController : MonoBehaviour
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

            [SerializeField] private float forwardSpeed = 8.0f;
        
            /// <summary>
            /// Speed when moving forward.
            /// </summary>
            
            public float ForwardSpeed
            {
                get => forwardSpeed;
                set => forwardSpeed = Mathf.Max(0.0f, value);
            }
            
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
            [SerializeField] private float turnSpeed = 6.0f;

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
            public Vector3 velocity
            {
                get => rigidbody.velocity;
                set => rigidbody.velocity = value;
            }
            
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
            [SerializeField] public float deceleration = 20.0f;
            
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
            
            MovementDirection = new Vector2(GetHorizontalInput(),GetVerticalInput());
            _accelerate = Input.GetButton(accelerateInput);
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


        private void Update()
        {
            HandleInput();
        }

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

        #endregion

        
        
        
        
        #region MonoBehaviour

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
