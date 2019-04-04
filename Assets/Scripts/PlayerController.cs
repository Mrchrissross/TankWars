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
    };

    public TankAssets assets;

    [Header("Style: "), Range(1, 8)]
        public int style = 5;
        int tempstyle;

    [Header("Health: "), Range(0.0f, 100.0f)]                    
        public int playerHealth = 100;

        [Header("Speeds"), VectorLabels("Foward", "Reverse")]
        public Vector2 vertical = new Vector2(40f, 25f);
        [VectorLabels("Turn", "Recoil", "Slow")]
        public Vector3 extra = new Vector3(30f, 5f, 13.4f);
        bool slow;

    [Header("Shooting: "), Tooltip("Time between shots in seconds."), Range(0.01f, 2.0f)]
        public float shotTimer = 0.8f;
        float shotDecreaseTime;
        [Tooltip("How fast the bullet will travel."), Range(10.0f, 80.0f)]
        public float bulletSpeed = 40.0f;
        [Tooltip("How much damage the bullet does.")]
        public int bulletDamage = 20;
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
            rb.AddForce(transform.up * (slow ? extra.z : vertical.x));                                  // Move forward

        if (Input.GetButton("Reverse"))                                         // If the keys set in the input menu "Reverse" are pressed the player will...
            rb.AddForce(transform.up * (slow ? extra.z : -vertical.y));                          // Move backward

        rb.AddTorque(Input.GetAxis("Left/Right") * (slow ? (-extra.x / 2) : -extra.x));                // This allows the player to move left or right if the input keys in left and right are pressed
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
        shotDecreaseTime -= Time.deltaTime;                                     // This is the timer for when the player can shoot next

        if (Input.GetMouseButtonDown(0) && shotDecreaseTime < 0f)               // And has clicked the left mouse button down and also has the shot decrease time below zero..
        {
            GameObject bullet = AssetManager.instance.SpawnObject("Shoot", firePoint.position, firePoint.rotation);                 // We will instantiate a bullet which is fired at whereever the player is aiming
            BulletController newBullet = bullet.transform.GetComponent<BulletController>();
            newBullet.bulletSpeed = bulletSpeed;                                                                                    // Gives the bullet a speed at which it will travel at
            newBullet.bulletDamage = bulletDamage;                                                                                  // How much damage the bullet does
            muzzleFlash.Play();                                                                                                     // Play a little smoke effect coming from the firepoint
            Recoil();                                                                                                               // Recoil from the shot
            shotDecreaseTime = shotTimer;                                                                                           // The shot decrease timer will be reset to the amount of the shot timer
        }
    }

    void Recoil()
    {
        if((Cannon.localEulerAngles.z > 0.0f && Cannon.localEulerAngles.z < 43.0f) || (Cannon.localEulerAngles.z > 318.0f && Cannon.localEulerAngles.z < 360.0f))
            rb.AddForce(transform.up * -extra.y, ForceMode2D.Impulse);

        else if (Cannon.localEulerAngles.z > 150.0f && Cannon.localEulerAngles.z < 210.0f)
            rb.AddForce(transform.up * extra.y, ForceMode2D.Impulse);
    }

    void CheckHealth()
    {
        if (playerHealth <= 0f)                                                                                     // If the player has zero life left
        {
            Destroy(gameObject);                                                                                    // Player tank will be destroyed
            AssetManager.instance.SpawnObject("TankExplosion", transform.position, transform.rotation);                             // Instantiate a tank explosion
            GameObject destroyedTank = AssetManager.instance.SpawnObject("DestroyedPlayerTank", transform.position, transform.rotation); // The destroyed tank prefab will instantiate at the players position
            destroyedTank.name = "DeadPlayer";
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Add triggers here.

        switch (other.gameObject.tag)
        {
            case "Mine":
                {
                    playerHealth = 0;
                    AssetManager.instance.SpawnObject("MineExplosion", other.transform.position, other.transform.rotation);
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
}