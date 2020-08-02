using UnityEngine;
using System.Collections.Generic;
using TankWars.Managers;

namespace TankWars.Controllers
{
    /// <summary>
    /// Controls the movement, style and shooting of the player tank.
    /// </summary>
    
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// The assets to the tank.
        /// </summary>
        [System.Serializable]
        public class TankAssets
        {
            [Header("Hulls")]
                public List<Sprite> Hulls = new List<Sprite>();

            [Header("Cannons")]
                public List<Sprite> Cannons = new List<Sprite>();
        };

        [Header("Style: "), Range(1, 8)]
            public int style = 5;
            int tempstyle;

        [Header("Health: "), Range(0.0f, 100.0f)]                    
            public int playerHealth = 100;

        [Header("Speeds")]
            public Vector2 vertical = new Vector2(40f, 25f);
            public Vector3 extra = new Vector3(30f, 5f, 13.4f);
            bool slow;

        [Header("Shooting: "), Tooltip("Time between shots in seconds.")]
            public Vector2 shotTimer = new Vector2(0.8f, 0.0f);
            [Tooltip("How fast the bullet will travel."), Range(10.0f, 80.0f)]
            public float bulletSpeed = 40.0f;
            [Tooltip("How much damage the bullet does."), Range(1.0f, 100.0f)]
            public int bulletDamage = 20;
            [Tooltip("Plays when the tanks shoots.")]
            ParticleSystem muzzleFlash;

        [Space]
            public TankAssets assets;

        Rigidbody2D rb;
        Transform Cannon;
        Transform firePoint;

        void Start()
        {
            ChangeStyle();
            rb = GetComponent<Rigidbody2D>();                              
            Cannon = transform.Find("Cannon");
            firePoint = transform.Find("Cannon").Find("PlayerTankCannon").Find("FirePoint");
            muzzleFlash = firePoint.transform.parent.Find("MuzzleFlash").GetComponent<ParticleSystem>();
        }

        #region Update()

        void Update()
        {
            ChangeStyle();
            UpdateCamera();
            RotateCannon();
            Shoot();
            CheckHealth();
        }

        /// <summary>
        /// Changes the tanks style.
        /// </summary>
        public void ChangeStyle()
        {
            if(tempstyle != style)
            {
                GetComponent<SpriteRenderer>().sprite = assets.Hulls[style - 1];
                transform.Find("Cannon").Find("PlayerTankCannon").GetComponent<SpriteRenderer>().sprite = assets.Cannons[style - 1];
                DestroyImmediate(transform.Find("Cannon").Find("PlayerTankCannon").GetComponent<PolygonCollider2D>());
                transform.Find("Cannon").Find("PlayerTankCannon").gameObject.AddComponent<PolygonCollider2D>();
                tempstyle = style;
            }
        }

        /// <summary>
        /// Ensures the main camera stays on top of the player tank.
        /// </summary>
        void UpdateCamera()
        {
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
        }

        /// <summary>
        /// Rotates the players cannon to the direction of the mouse.
        /// </summary>
        void RotateCannon()
        {
            Vector3 mousePos = Input.mousePosition;                                                                        
            Vector3 screenPos = Camera.main.WorldToScreenPoint(Cannon.position);                              
            Vector3 offset = new Vector3(mousePos.x - screenPos.x, mousePos.y - screenPos.y);   

            float angle = Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg;                      
            Cannon.rotation = Quaternion.AngleAxis(angle, Vector3.back);                        
        }

        /// <summary>
        /// Shoots when the player clicks the left mouse button.
        /// </summary>
        void Shoot()
        {
            if(shotTimer.y > 0f) shotTimer.y -= Time.deltaTime;

            if (!Input.GetMouseButtonDown(0) || !(shotTimer.y < Mathf.Epsilon)) return;
            
            var bullet = AssetManager.Instance.SpawnObject("Shoot", firePoint.position, firePoint.rotation, true);                 
            var newBullet = bullet.transform.GetComponent<BulletController>();
            newBullet.BulletSpeed = bulletSpeed;                                                                                    
            newBullet.BulletDamage = bulletDamage;                                                                                  
            muzzleFlash.Play();                                                                                                     
            Recoil();                                                                                                               
            shotTimer.y = shotTimer.x;
        }

        /// <summary>
        /// Recoils the tank in the opposite direction to the shot.
        /// </summary>
        void Recoil()
        {
            if((Cannon.localEulerAngles.z > 0.0f && Cannon.localEulerAngles.z < 43.0f) || (Cannon.localEulerAngles.z > 318.0f && Cannon.localEulerAngles.z < 360.0f))
                rb.AddForce(transform.up * -extra.y, ForceMode2D.Impulse);

            else if (Cannon.localEulerAngles.z > 150.0f && Cannon.localEulerAngles.z < 210.0f)
                rb.AddForce(transform.up * extra.y, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Performs a health check on the tank.
        /// </summary>
        void CheckHealth()
        {
            if (playerHealth <= 0f)                                                                                     
            {
                Destroy(gameObject);                                                                                    
                AssetManager.Instance.SpawnObject("TankExplosion", transform.position, transform.rotation);                             
                GameObject destroyedTank = AssetManager.Instance.SpawnObject("DestroyedPlayerTank", transform.position, transform.rotation); 
                destroyedTank.name = "DeadPlayer";
            }
        }

        #endregion

        #region FixedUpdate()

        void FixedUpdate()
        {
            Move();
        }

        /// <summary>
        /// Moves the player tank in the desired direction.
        /// </summary>
        void Move()
        {
            rb.velocity = transform.up * Vector2.Dot(rb.velocity, transform.up);    

            if (Input.GetButton("Accelerate"))                                      
                rb.AddForce(transform.up * (slow ? extra.z : vertical.x));                                  

            if (Input.GetButton("Reverse"))                                         
                rb.AddForce(transform.up * (slow ? extra.z : -vertical.y));                          

            rb.AddTorque(Input.GetAxis("Left/Right") * (slow ? (-extra.x / 2) : -extra.x));                
        }

        #endregion

        #region Triggers

        void OnTriggerEnter2D(Collider2D other)
        {
            switch (other.gameObject.tag)
            {
                case "Mine":
                    {
                        playerHealth = 0;
                        AssetManager.Instance.SpawnObject("MineExplosion", other.transform.position, other.transform.rotation);
                        Destroy(other.gameObject);
                        break;
                    }
                case "DestroyedTank":
                    {
                        slow = true;
                        break;
                    }
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            switch (other.gameObject.tag)
            {
                case "DestroyedTank":
                    {
                        slow = false;
                        break;
                    }
            }
        }

        #endregion
    }
}
