using System;
using System.Collections.Generic;
using TankWars.Managers;
using TankWars.Utility;
using UnityEngine;

namespace TankWars.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    
    public class WeaponController : MonoBehaviour
    {
        #region Properties

        

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
            var index = firePoints.Count == 1 ? 0 : 1;
            var end = firePoints.Count == 1 ? 1 : 3;
            
            for (; index < end; index++)
            {
                ref var weapon = ref weapons[index];
                
                var shotTimer = weapon.ShotTimer;
                if(shotTimer.y > 0f) shotTimer.y -= Time.deltaTime;
                weapon.ShotTimer = shotTimer;

                var input = Input.GetButtonDown(cannonInput[index]) || 
                            Math.Abs(Input.GetAxisRaw(cannonInput[index])) > 0.0f;
                
                if (!input || !(shotTimer.y < Mathf.Epsilon)) continue;
                shootCannon[index] = false;

                var firePoint =
                    index == 0 ? firePoints[0] :
                    index == 1 ? firePoints[0] :
                    firePoints[1];
                
                var ammo = AssetManager.Instance.SpawnObject(weapon.Asset, firePoint.position, firePoint.rotation);
                AudioManager.Instance.PlaySound(weapon.ShotSound);
                
                // Muzzle Flash

                var ammoController = ammo.AddComponent<AmmoController>();
                ammoController.InitialiseAmmo(weapon);
                
                ammo.AddComponent<Rigidbody2D>();
                
                //Recoil
                
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