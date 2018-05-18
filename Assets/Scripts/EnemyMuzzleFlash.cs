using UnityEngine;
using System.Collections;

public class EnemyMuzzleFlash : MonoBehaviour {

    public float muzzleTimer;       // How long the cannon muzzle will smoke
    public EnemyShooting myGun;     // Reference to the cannon

    void Update()
    {
        muzzleTimer = muzzleTimer - .1f;                                                // Muzzle timer (2) = 2 - 0.1 --- Muzzle will smoke of aprox. 2 seconds

        if (myGun.isFiring == true)                                                     // If the Player has fired... 
            {
                gameObject.GetComponent<ParticleSystem>().enableEmission = true;        // Smoke Emission will be true
                muzzleTimer = 2f;                                                       // Muzzle timer becomes 2 again and then counts down
            }

        if (muzzleTimer < 0f)                                                           // If the Muzzle timer reaches zero... 
            {
                gameObject.GetComponent<ParticleSystem>().enableEmission = false;       // the smoke emission will stop
            }        
    }
}
