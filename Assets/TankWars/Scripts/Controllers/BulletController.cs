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
        
        #region Properties & Fields

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
        private Rigidbody2D _rigidbody2D;

        /// <summary>
        /// How much damage the bullet does.
        /// </summary>
        
        public float BulletDamage
        {
            get => _bulletDamage;
            set => _bulletDamage = Mathf.Max(0.0f, value);
        }
        [Tooltip("How much damage the bullet does.")] private float _bulletDamage = 20.0f;
        
        /// <summary>
        /// How fast the bullet travels.
        /// </summary>
        
        public float BulletSpeed
        {
            get => _bulletSpeed;
            set => _bulletSpeed = Mathf.Clamp(value, 20.0f, 100.0f);
        }
        [Tooltip("How fast the bullet travels.")] private float _bulletSpeed = 40f;
        
        [Tooltip("Layers that the bullet can collide with.")] public LayerMask collisionLayer;
        
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


