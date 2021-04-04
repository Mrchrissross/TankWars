using System;
using System.Collections.Generic;
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
        
        #region Properties

        // The storage of components for easy access.
        #region Cached Components
            
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
        
        #endregion
        
        
        
        #region Fields

        #region Unity Inspector

        [HideInInspector] public bool[] hideSection = new bool[6];

        #endregion
        
        
        // Storage of fire point transforms.
        public List<Transform> firePoints = new List<Transform>();
        
        // Three separate booleans for each cannon.
        public string[] cannonInput = {"Main Shoot", "Left Shoot", "Right Shoot"};
        public bool[] shootCannon = new bool[3];
        public Weapon[] weapons = new Weapon[3];
        public bool[] weaponDropdown = new bool[3];

        #endregion

        
        
        #region Functions

        /// <summary>
        /// Shoots when the player presses the shoot key.
        /// </summary>
        
        void Shoot()
        {
            // Iterate through the fire points.
            var index = firePoints.Count == 1 ? 0 : 1;
            var end = firePoints.Count == 1 ? 1 : 3;
            
            for (; index < end; index++)
            {
                // Acquire the fire points weapon information.
                ref var weapon = ref weapons[index];

                // If no weapon has been assigned, return.
                if (weapon == null) continue;
                
                // Check the shot timer.
                var shotTimer = weapon.ShotTimer;
                if(shotTimer.y > 0f) shotTimer.y -= Time.deltaTime;
                weapon.ShotTimer = shotTimer;

                // Check for user input.
                var input = Input.GetButtonDown(cannonInput[index]) || 
                            Math.Abs(Input.GetAxisRaw(cannonInput[index])) > 0.0f;
                
                // If there has been no user input or the shot timer is not depleted,
                // continue on to next fire point.
                if (!input || !(shotTimer.y < Mathf.Epsilon)) continue;
                
                // The cannon has been fired.
                shootCannon[index] = false;

                // Acquire fire point.
                var firePoint =
                    index == 0 ? firePoints[0] :
                    index == 1 ? firePoints[0] :
                    firePoints[1];

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
                
                //
                // Perform the recoil (working progress).
                //
                
                // Restart the shot timer.
                weapon.ShotTimer = shotTimer.WithY(shotTimer.x);
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