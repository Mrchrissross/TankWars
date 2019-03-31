using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class TankAssets
    {
        [Header("Hulls")]
            public List<Sprite> Hulls = new List<Sprite>();

        [Header("Cannons")]
            public List<Sprite> Cannons = new List<Sprite>();

        [Header("Bullet")]
            public BulletController bullet;

        [Header("Player Death")]
            public GameObject tankExplosion;
            public GameObject destroyedTank;
    };

    public TankAssets assets;

    [Header("Style: "), Range(1, 8)]
        public int style = 5;
        int tempstyle;

    [Header("Health: "), Range(0.0f, 100.0f)]                    
        public float playerHealth = 100f;
           
    [Header("Speeds: ")]                    
        public float Speed = 40f;
        public float reverseSpeed = 25f;
        public float turnSpeed = 30f;
        public float recoilSpeed = 5f;
        public float reducedMovementSpeed = 13.4f;

    [Header("Shooting: "), Tooltip("Time between shots in seconds."), Range(0.01f, 2.0f)]
        public float shotTimer = 0.8f;
        float shotDecreaseTime;
        [Tooltip("How fast the bullet will travel."), Range(10.0f, 80.0f)]
        public float bulletSpeed = 40.0f;
        [Tooltip("How much damage the bullet does.")]
        public float bulletDamage = 20.0f;
        [Tooltip("Plays when the tanks shoots.")]
        ParticleSystem muzzleFlash;

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

    void Update()
    {
        ChangeStyle();
        UpdateCamera();
        RotateCannon();
        Shoot();
        CheckHealth();
    }

    void FixedUpdate()
    {
        Move();
    }

    void ChangeStyle()
    {
        if(tempstyle != style)
        {
            GetComponent<SpriteRenderer>().sprite = assets.Hulls[style - 1];
            transform.Find("Cannon").Find("PlayerTankCannon").GetComponent<SpriteRenderer>().sprite = assets.Cannons[style - 1];
            Destroy(transform.Find("Cannon").Find("PlayerTankCannon").GetComponent<PolygonCollider2D>());
            transform.Find("Cannon").Find("PlayerTankCannon").gameObject.AddComponent<PolygonCollider2D>();
            tempstyle = style;
        }
    }

    void UpdateCamera()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
    }

    void Move()
    {
        rb.velocity = transform.up * Vector2.Dot(rb.velocity, transform.up);    // Give the rigidbody a velocity through the method forward velocity

        if (Input.GetButton("Accelerate"))                                      // If the keys set in the input menu "Accelerate" are pressed the player will...
            rb.AddForce(transform.up * Speed);                                  // Move forward

        if (Input.GetButton("Reverse"))                                         // If the keys set in the input menu "Reverse" are pressed the player will...
            rb.AddForce(transform.up * -reverseSpeed);                          // Move backward

        rb.AddTorque(Input.GetAxis("Left/Right") * -turnSpeed);                // This allows the player to move left or right if the input keys in left and right are pressed
    }

    void RotateCannon()
    {
        Vector3 mousePos = Input.mousePosition;                                             //Used to get mouse position                                     
        Vector3 screenPos = Camera.main.WorldToScreenPoint(Cannon.position);                //Used to get object position and put it on the screen               
        Vector3 offset = new Vector3(mousePos.x - screenPos.x, mousePos.y - screenPos.y);   //Check where the mouse is relative to the object 

        float angle = Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg;                      //Turn that into an angle and convert to degrees
        Cannon.rotation = Quaternion.AngleAxis(angle, Vector3.back);                        //Set the object's rotation to be of the angle over -Z
    }

    void Shoot()
    {
        shotDecreaseTime = shotDecreaseTime -= Time.deltaTime;                  // This is the timer for when the player can shoot next

        if (Input.GetMouseButtonDown(0) && shotDecreaseTime < 0f)               // And has clicked the left mouse button down and also has the shot decrease time below zero..
        {
            BulletController newBullet = Instantiate(assets.bullet, firePoint.position, firePoint.rotation) as BulletController;    // We will instantiate a bullet which is fired at whereever the player is aiming
            newBullet.bulletSpeed = bulletSpeed;                                                                                    // Gives the bullet a speed at which it will travel at
            newBullet.bulletDamage = bulletDamage;                                                                                  // How much damage the bullet does
            AudioManager.instance.PlaySound("PlayerShot");                                                                          // Play the shotClip 
            muzzleFlash.Play();                                                                                                     // Play a little smoke effect coming from the firepoint
            Recoil();                                                                                                               // Recoil from the shot
            shotDecreaseTime = shotTimer;                                                                                           // The shot decrease timer will be reset to the amount of the shot timer
        }
    }

    void Recoil()
    {
        if((Cannon.localEulerAngles.z > 0.0f && Cannon.localEulerAngles.z < 43.0f) || (Cannon.localEulerAngles.z > 318.0f && Cannon.localEulerAngles.z < 360.0f))
            rb.AddForce(transform.up * -recoilSpeed, ForceMode2D.Impulse);

        else if (Cannon.localEulerAngles.z > 150.0f && Cannon.localEulerAngles.z < 210.0f)
            rb.AddForce(transform.up * recoilSpeed, ForceMode2D.Impulse);
    }

    void CheckHealth()
    {
        if (playerHealth <= 0f)                                                                                     // If the player has zero life left
        {
            Destroy(gameObject);                                                                                    // Player tank will be destroyed
            AudioManager.instance.PlaySound("TankExplosion");                                                       // Play the tank explosion sound
            Instantiate(assets.tankExplosion, transform.position, transform.rotation = Quaternion.identity);        // A tank explosion will be instatiated at the location of the player
            GameObject destroyedTank = Instantiate(assets.destroyedTank, transform.position, transform.rotation);   // The destroyed tank prefab will instantiate at the players position
            destroyedTank.name = "DeadPlayer";
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Add triggers here.

        if (other.gameObject.CompareTag("Mine"))
        {
            playerHealth = 0.0f;
            Destroy(other.gameObject);
            Instantiate(assets.tankExplosion, other.transform.position, other.transform.rotation = Quaternion.identity);
        }
    }
}