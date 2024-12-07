using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DimensionalDeveloper.TankBuilder.Utility;
using DimensionalDeveloper.TankBuilder.Managers;

namespace DimensionalDeveloper.TankBuilder.Controllers
{
    /// <summary>
    /// The weapon controller, controls all aspects of the tanks weapons. Once a weapon scriptable object has been added,
    /// it will manage all data within, use it and send the rest to the individual ammo controllers.
    /// </summary>
    
    public class WeaponController : MonoBehaviour
    {

        #region Classes

        [Serializable]
        public class LinkedWeapon
        {
            
            
            #region Properties

            /// <summary>
            /// Returns true is a the weapon is available to shoot.
            /// </summary>
            
            public virtual bool CanShoot => !OnCooldown && InputReceived;
            
            /// <summary>
            /// Returns true if there has been user input to use this weapon. 
            /// </summary>
            
            public virtual bool InputReceived => Input.GetButtonDown(input) || Math.Abs(Input.GetAxisRaw(input)) > 0.0f;
            
            /// <summary>
            /// Returns true if the weapon is currently on cooldown.
            /// </summary>
            
            public virtual bool OnCooldown => !(cooldown.y < Mathf.Epsilon);

            #endregion

            
            
            #region Fields

            public Transform firePoint;
            
            public string input = "";
            public Weapon weapon;
            public bool expandWeapon = false;

            public Vector2 cooldown;

            #endregion



            #region Methods

            /// <summary>
            /// Shoots the weapon - playing a sound, spawning a projectile and muzzle flash.
            /// </summary>
            
            public virtual void SpawnProjectile()
            {
                UpdateCooldown();
                
                if (!CanShoot) return;

                // Acquire fire point.
                var firePointPosition = firePoint.position;
                var firePointRotation = firePoint.rotation;
                
                // Spawn the fired ammo, at the fire point, location and rotation.
                var ammo = AssetManager.SpawnObject(weapon.Asset, firePointPosition, firePointRotation);
                AudioManager.PlaySound(weapon.ShotSound);
                
                // Perform the muzzle flash.
                AssetManager.SpawnObject(weapon.MuzzleFlash, firePointPosition, firePointRotation);
                
                // Ensure a rigidbody exists on the ammo.
                if(ammo.GetComponent<Rigidbody2D>() == null)
                    ammo.AddComponent<Rigidbody2D>();
                
                // Acquire ammo controller.
                var ammoController = ammo.GetComponent<AmmoController>();
                
                // If null, create one.
                if(ammoController == null)
                    ammoController = ammo.AddComponent<AmmoController>();
                
                // Initialise the ammo with all the weapons information.
                ammoController.InitialiseAmmo(weapon);

                ResetCooldown();
            }
            
            /// <summary>
            /// Updates the cooldown of this weapon.
            /// </summary>
            
            public virtual void UpdateCooldown()
            {
                if(cooldown.y > -0.01f) cooldown.y -= Time.deltaTime;
            }
            
            /// <summary>
            /// Resets the cooldown of this weapon.
            /// </summary>
            
            public virtual void ResetCooldown() => cooldown.y = weapon.Cooldown;

            #endregion
            
            
        }

        #endregion
        
        
        
        #region Fields

        public List<LinkedWeapon> linkedWeapons = new();

        #endregion

        
        
        #region Methods

        /// <summary>
        /// Creates all weapons based on how many fire points are referenced. 
        /// </summary>
        /// <param name="firePoints">The fire points to link. Typically pulled from the Tank Builder.</param>
        
        public virtual void CreateWeapons(List<Transform> firePoints)
        {
            linkedWeapons.Clear();
            
            foreach (var firePoint in firePoints)
                AddWeapon(firePoint);
        }
        
        /// <summary>
        /// Adds a linked weapon to the linked weapon list.
        /// </summary>
        /// <param name="firePoint">The fire point to link.</param>
        
        public virtual void AddWeapon(Transform firePoint)
        {
            linkedWeapons.Add(new LinkedWeapon()
            {
                firePoint = firePoint
            });
        }
        
        /// <summary>
        /// Removes a linked weapon from the linked weapon list.
        /// </summary>
        /// <param name="firePoint">The linked fire point.</param>
        
        public virtual void RemoveWeapon(Transform firePoint)
        {
            foreach (var linkedWeapon in linkedWeapons.Where(linkedWeapon => linkedWeapon.firePoint == firePoint))
            {
                linkedWeapons.Remove(linkedWeapon);
                return;
            }
        }
        
        /// <summary>
        /// Shoots when the player presses the shoot key.
        /// </summary>
        
        protected virtual void Shoot()
        {
            // Iterate through the fire points.
            foreach (var linkedWeapon in linkedWeapons)
            {
                // If no weapon has been assigned, return.
                linkedWeapon?.SpawnProjectile();
            }
        }
        
        

        #endregion

        
        
        #region MonoBehaviour

        private void Update()
        {
            Shoot();
        }

        #endregion
        
    }
}