using UnityEngine;

public class EnemyShooting : MonoBehaviour {

    // This Script will contol exacty when and how the enemy fires a bullet

    public bool isFiring;                   // Simple bool to state when the enemy is firing, for reference to other scripts
    public bool playerspotted;              // Simple bool to state when the enemy has got the player in its sight, for reference to other scripts

    public Transform sightStart;            // This is location of when the enemy can see from
    public Transform sightEnd;              // This is location of how far the enemy can see
    public Transform firePoint;             // This is the orgin of where the bullet will be fired from

    private bool playerInView = false;      // Simple bool to state when the player is in view of the enemy, for reference to other scripts
    private bool viewBlocked = false;       // Simple bool to state if the enemies view of the player is being obstructed by Rocks, another enemy ect..

    public BulletController bullet;         // Reference for the bullet prefab to fire
    [Header("Shot timer")]
    public float timeBetweenShots = 1f;     // This is how long the enemy has to wait before being able to fire again
    private float shotCounter;              // This is the timer which will count down from the timeBetweenShots variable
    [Header("Audio")]
    public string shotAudio;                // Reference to what sound we will use when a bullet is fired

    private AudioManager audioManager;      // Reference to our Audio Manager, to play our audio

    void Start()
    {
        audioManager = AudioManager.instance;                                       // This allows the audio manager to be linked with this script with having a public link through unity UI
        if (audioManager == null)                                                   // If the audio manager is not in this scene, display error in console                                                                          
            Debug.LogError("AudioManager: No AudioManager found in this scene.");   // Error : "AudioManager: No AudioManager found in this scene."
    }

    void Update ()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();               // Reference to our rigidbody

        Raycasting();                                               // Use the Raycasting Method

        if (viewBlocked == false && playerInView == true)           // If the enemies view is not blocked and the player is in view...
        {
            shotCounter -= Time.deltaTime;                          // ShotCounter/Timer starts going down 
            playerspotted = true;                                   // Player Spotted bool becomes true

            if (shotCounter <= 0)                                                                                               // If our shot timer is below or equal to zero...
            {
                isFiring = true;                                                                                                // Is firing bool becomes true
                shotCounter = timeBetweenShots;                                                                                 // Shot timer equals the time between shots variable
                BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as BulletController;   // We will instantiate a bullet which is fired at the player
                rb.AddForce(transform.up * -25 * 17);                                                                           // We add a little recoil to our tank for realness
                audioManager.PlaySound(shotAudio);                                                                              // The shot audio will be played
            }
            else                                                                                                                // Else if the shotcounter is not equal zero
                isFiring = false;                                                                                               // The is firing bool is false as the tank is not firing
        }
    }

    void Raycasting()                                                                                                       // The Raycasting Method
    {
        Debug.DrawLine(sightStart.position, sightEnd.position, Color.green);                                                // This adds a gizmo to our scene-editor to see how far our enemy can fire
        viewBlocked = Physics2D.Linecast(sightStart.position, sightEnd.position, 1 << LayerMask.NameToLayer("Rocks"));      // This is used to detect if the enemies view is blocked by a gameobject with the layer name "Rocks".
        playerInView = Physics2D.Linecast(sightStart.position, sightEnd.position, 1 << LayerMask.NameToLayer("Player"));    // This is used to detect if the player has come into the enemies line of sight.
    }
}
