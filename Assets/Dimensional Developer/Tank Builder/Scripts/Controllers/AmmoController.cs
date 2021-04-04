using UnityEngine;
using DimensionalDeveloper.TankBuilder.Managers;
using DimensionalDeveloper.TankBuilder.Utility;

namespace DimensionalDeveloper.TankBuilder.Controllers
{
    /// <summary>
    /// Performs bullet movement, collision detection and damage.
    /// </summary>
    
    public class AmmoController : MonoBehaviour
    {
        
        #region Properties

        // The storage of components for easy access.
        #region Cached Components
            
        /// <summary>
        /// Cached 'Rigidbody' component.
        /// </summary>
        
        private new Rigidbody2D Rigidbody2D
        {
            get
            {
                if (_rigidbody2D) return _rigidbody2D;
                
                _rigidbody2D = GetComponent<Rigidbody2D>();
                ExtensionsLibrary.CheckComponent(_rigidbody2D, "Rigidbody Component", name);
                return _rigidbody2D;
            }
        }
        private Rigidbody2D _rigidbody2D;

        /// <summary>
        /// Cached 'AudioManager' component.
        /// </summary>
        
        private new AudioManager AudioManager
        {
            get
            {
                if (_audioManager) return _audioManager;

                _audioManager = AudioManager.Instance;
                
                if(_audioManager == null) Debug.LogError("Ammo Controller: An audio manager was not found in the scene.");

                return _audioManager;
            }
        }
        private AudioManager _audioManager;
        
        /// <summary>
        /// Cached 'AssetManager' component.
        /// </summary>
        
        private new AssetManager AssetManager
        {
            get
            {
                if (_assetManager) return _assetManager;

                _assetManager = AssetManager.Instance;
                
                if(_assetManager == null) Debug.LogError("Ammo Controller: An asset manager was not found in the scene.");

                return _assetManager;
            }
        }
        private AssetManager _assetManager;
            
        #endregion


        
            
        #region Bullet Information
        // Contains all the bullets information.
        
        /// <summary>
        /// How much damage the bullet does.
        /// </summary>
        
        public float Damage
        {
            get => _damage;
            set => _damage = Mathf.Max(0.0f, value);
        }
        private float _damage = 20.0f;
        
        /// <summary>
        /// How fast the bullet travels.
        /// </summary>
        
        public float Speed
        {
            get => _speed;
            set => _speed = Mathf.Max(20.0f, value);
        }
        private float _speed = 40.0f;
        
        #endregion
        
        #endregion

        
        
        #region Fields

        #region Bullet Information
        
        // (Ensure below matches the settings found in the Audio Manager)
        public string explosionSound;        // Sound effect when the ammo explodes.

        // (Ensure below matches the settings found in the Asset Manager)
        public string explosion;             // The particle system to play when the ammo explodes.
        
        // Layers that the ammo can collide with.
        public LayerMask collisionLayer;
        
        #endregion

        #endregion
        
        
        
        #region Functions

        /// <summary>
        /// Copies an existing weapon.
        /// </summary>

        public void InitialiseAmmo(Weapon copy)
        {
            Speed = copy.Speed;
            Damage = copy.Damage;
            explosionSound = copy.ExplosionSound;
            explosion = copy.Explosion;
            collisionLayer = copy.CollisionLayer;
            
            Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        private void Move()
        {
            Vector2 velocity = transform.up * (Speed * Time.fixedDeltaTime);
            Rigidbody2D.MovePosition(Rigidbody2D.position + velocity);
        } 

        private void PerformCollisionCheck()
        {
            var hitInfo = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, collisionLayer);

            if (hitInfo.collider == null) return;
            var impactTarget = hitInfo.collider.transform;
            
            Explode();
            
            // Add triggers here.
            switch(impactTarget.tag)
            {
                case "Mine":
                {
                    AssetManager.SpawnObject("Mine Explosion", impactTarget.position, impactTarget.rotation);
                    AudioManager.PlaySound("Mine Explosion");
                    Destroy(impactTarget.gameObject);
                    break;
                }
            }
        }
        
        /// <summary>
        /// Triggers an explosion by destroying the game object and spawning an explosion animation.
        /// </summary>
        private void Explode()
        {
            gameObject.SetActive(false);
            AssetManager.SpawnObject(explosion, transform.position, Quaternion.Euler(transform.rotation.eulerAngles.WithZ(0)));
            AudioManager.PlaySound(explosionSound);
        }
        
        #endregion


        
        #region MonoBehaviour
        
        private void FixedUpdate()
        {
            Move();
            PerformCollisionCheck();
        }

        private void OnValidate()
        {
            Damage = _damage;
            Speed = _speed;
        }

        #endregion
        
    }
}


