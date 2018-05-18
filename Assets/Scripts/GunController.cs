using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

    // This is our players Gun Controller which controls how the player fires a bullet

    public bool isFiring;                   // Simple bool to state when the player is firing, for reference to other scripts

    public BulletController bullet;         // Reference for the bullet prefab to fire
    public float bulletSpeed;

    private float timeBetweenShots = 1f;    // This is how long the player has to wait before being able to fire again
    private float shotCounter;              // This is the timer which will count down from the timeBetweenShots variable

    public Transform firePoint;             // This is the orgin of where the bullet will be fired from

    void Update ()
    {
	    if(isFiring)                                                                                                            // If the player is firing...
        {
            shotCounter -= Time.deltaTime;                                                                                      // ShotCounter/Timer starts going down

            if (shotCounter <= 0)                                                                                               // If our shot timer is below or equal to zero...
            {
                shotCounter = timeBetweenShots;                                                                                 // Shot timer equals the time between shots variable
                BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as BulletController;   // We will instantiate a bullet which is fired at whereever the player is aiming
                newBullet.bulletSpeed = bulletSpeed;                                                                            // Gives the bullet a speed at which it will travel at
            }
        }
        else                                                                                                                    // Else if the shotcounter is not equal zero
            shotCounter = 0;                                                                                                    // The is firing bool is false as the tank is not firing
    }
}
