using System;
using TankWars.Managers;
using TankWars.Utility;
using UnityEngine;

namespace TankWars.Controllers
{
    /// <summary>
    /// Performs bullet movement, collision detection and damage.
    /// </summary>
    
    public class BulletController : MonoBehaviour
    {
        
        /// <summary>
        ///  The type of bullet that the tank will fire.
        /// </summary>
        
        public enum BulletType
        {
            Bullet,
            Shell,
            Missile
        }
        
        #region Properties

        /// <summary>
        /// The storage of components for easy access.
        /// </summary>
            
        [Header("Cached Components")]

        #region Cached Components
            
        private Rigidbody2D _rigidbody2D;
        
        /// <summary>
        /// Cached 'Rigidbody' component.
        /// </summary>
        
        private new Rigidbody2D rigidbody2D
        {
            get
            {
                if (_rigidbody2D) return _rigidbody2D;
                
                _rigidbody2D = GetComponent<Rigidbody2D>();
                ExtensionsLibrary.CheckComponent(_rigidbody2D, "Rigidbody Component", name);
                return _rigidbody2D;
            }
        }
            
        #endregion


        
        /// <summary>
        /// Contains all the bullets information.
        /// </summary>
            
        [Header("Bullet Information")]
        
        #region Bullet Information

        private float _bulletDamage = 20.0f;
        
        /// <summary>
        /// How much damage the bullet does.
        /// </summary>
        
        public float BulletDamage
        {
            get => _bulletDamage;
            set => _bulletDamage = Mathf.Max(0.0f, value);
        }
        
        /// <summary>
        /// How fast the bullet travels.
        /// </summary>
        
        public float BulletSpeed
        {
            get => _bulletSpeed;
            set => _bulletSpeed = Mathf.Max(20.0f, value);
        }
        private float _bulletSpeed = 40.0f;
        
        #endregion
        
        #endregion

        
        
        #region Fields

        // The type of bullet fired.
        public BulletType type;
        
        // Layers that the bullet can collide with.
        public LayerMask collisionLayer;

        #endregion
        
        
        
        #region Functions

        private void Move()
        {
            Vector2 velocity = transform.up * (BulletSpeed * Time.fixedDeltaTime);
            rigidbody2D.MovePosition(rigidbody2D.position + velocity);
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
                    AssetManager.Instance.SpawnObject("MineExplosion", impactTarget.position, impactTarget.rotation);
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
            Destroy(gameObject);
            AssetManager.Instance.SpawnObject("BulletExplosion", transform.position, transform.rotation, true);
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
            BulletDamage = _bulletDamage;
            BulletSpeed = _bulletSpeed;
        }

        #endregion
        
    }
}


