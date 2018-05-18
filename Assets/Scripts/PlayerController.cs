using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool countReady;                 // Simple bool to tell when the count ready is true or false
    public bool combatReady;                // Simple bool to tell when the combat ready is true or false
    public bool levelComplete;              // Simple bool to tell when the level is complete
    public bool playerDead;                 // Simple bool to tell when the player is dead
    bool timerPaused = true;                // Simple bool to tell if the timer has been paused, this is set to true
    public bool reducedSpeed = false;       // Simple bool to tell  when the tank has reduced speed

    [Header("Health: ")]                    
        public float playerHealth = 100f;
           
    [Header("Speeds: ")]                    
        public float Speed = 40f;
        float reverseSpeed = -25f;
        float torqueForce = -30f;
        public float recoilSpeed = 25f;
        public float shotTimer = 5f;
        float shotDecreaseTime;
        public float reducedMovementSpeed = 13.4f;

    [Header("Texts: ")]                     
        public Text countText;          
        public Text Health;
        float textTimer = 18f;

    [Header("Audio")]                       
        public string shotAudio;
        public string moveAudio;
        public string standAudio;
        public string deathAudio;
        public string GameAudio = "GameAudio";

    int count;                              // This is used to cound the number of squads you have rescued

    public GunController myGun;             // Reference to the player's cannon
    private AudioManager audioManager;      // Reference to the Audio Manager

    public GameObject destroyedTank;        // Reference to the destroyed tank prefab
    [SerializeField]
    private GameObject gameOverUI;          // Reference to the Gameover UI
    [SerializeField]
    private Transform player;               // Reference to the player's position
    [SerializeField]
    private GameObject tankExplosion;       // Reference to the tank explosion animation prefab

    void Start()
    {
        audioManager = AudioManager.instance;                                       // This allows the audio manager to be linked with this script with having a public link through unity UI
        if (audioManager == null)                                                   // If the audio manager is not in this scene, display error in console                                                                          
            Debug.LogError("AudioManager: No AudioManager found in this scene.");   // Error : "AudioManager: No AudioManager found in this scene."       

        count = 0;                                                                  // On script start, count will be set to zero
        SetCountText();                                                             // Use Set count method to display the count ("Rescued = 0")

        audioManager.PlaySound(standAudio);                                         // Use the stand audio.
        audioManager.PlaySound(GameAudio);                                          // Play the background music
    }

    void Update()
    {
        if (!timerPaused)                                                           // If the timer is not paused
        textTimer -= Time.deltaTime;                                                // Timer count down

        //-------------------------------------------------------------------------------- Player Shooting

        shotDecreaseTime = shotDecreaseTime - .1f;                                  // This is the timer for when the player can shoot next

        Rigidbody2D rb = GetComponent<Rigidbody2D>();                               // Get the player's rigidbody component

        if (combatReady == true)                                                    // If the player is combat ready...
        {
            if (Input.GetMouseButtonDown(0) && shotDecreaseTime < 0f)               // And has clicked the left mouse button down and also has the shot decrease time below zero..
            {
                myGun.isFiring = true;                                              // The player is firing
                audioManager.PlaySound(shotAudio);                                  // The shot audio is playing
                rb.AddForce(transform.up * reverseSpeed * recoilSpeed);             // The player will recoil
                shotDecreaseTime = shotTimer;                                       // The shot decrease timer will be reset to the amount in the shot timer
            }

            if (Input.GetMouseButtonUp(0))                                          // If the player has let go of the left mouse button and it is up...
                myGun.isFiring = false;                                             // The player is no longer firing
        }

        //------------------------------------------------------------------------------------ Texts      

        if (combatReady == true)                                                    // If the player is combat ready...
        {
            timerPaused = false;                                                    // The timer will be unpaused
            Health.text = "Health = " + playerHealth + "%";                         // The health text will display
        }
     
        if (textTimer <= 0)                                                         // If the text timer has hit zero...
            Health.text = "" + playerHealth + "%";                                  // The health text on the player screen will no longer display "Health = 100%" and will now only display "100%" 
                 
    }

    void FixedUpdate()
    {

        //-------------------------------------------------------------------------------------- Player Movement

        if (combatReady == true)                                                    // If the player is combat ready...
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();                           // Get the player's rigidbody component

            rb.velocity = ForwardVelocity();                                        // Give the rigidbody a velocity through the method forward velocity

            if (Input.GetButton("Accelerate"))                                      // If the keys set in the input menu "Accelerate" are pressed the player will...
                rb.AddForce(transform.up * Speed);                                  // Move forward
         
            if (Input.GetButton("Reverse"))                                         // If the keys set in the input menu "Reverse" are pressed the player will...
                rb.AddForce(transform.up * reverseSpeed);                           // Move backward

            rb.AddTorque(Input.GetAxis("Left/Right") * torqueForce);                // This allows the player to move left or right if the input keys in left and right are pressed
        }

        if (reducedSpeed == true)
        {
            Speed = reducedMovementSpeed;
        }
        else
        {
            Speed = 40f;
        }

        //-------------------------------------------------------------------------------------- Player Death

        if (playerHealth <= 0f)                                                                         // If the player has zero life left
        {
            Health.text = "0%";                                                                         // Health text will display 0%
            Destroy(gameObject);                                                                        // Player tank will be destroyed
            Instantiate(tankExplosion, player.position, transform.rotation = Quaternion.identity);      // A tank explosion will be instatiated at the location of the player
            audioManager.PlaySound(deathAudio);                                                         // A tank destruction audio will play
            Instantiate(destroyedTank, player.position, player.rotation);                               // The destroyed tank prefab will instantiate at the players position
            playerDead = true;                                                                          // The bool playerDead will become true
            EndGame();                                                                                  // The game over UI will be displayed
        }

        //-------------------------------------------------------------------------------------- Player Health

        if (playerHealth <= 100f)                                                                       // If the players health is below 100%
            Health.color = new Color(30.0f / 255.0f, 26.0f / 255.0f, 8.0f / 255.0f);                    // The color will change to <----
     
        if (playerHealth > 100f)                                                                        // If the players health is at 100% or above
            Health.color = new Color(211.0f / 255.0f, 211.0f / 255.0f, 211.0f / 255.0f);                // The color will change to <----

        if (playerHealth > 150f)                                                                        // If the players health is above 150%
            playerHealth = 150f;                                                                        // The player's health will be limited at 150%
    }

        //-------------------------------------------------------------------------------------- Movement Methods

    Vector2 ForwardVelocity()                                                                       //Move Forward/Backward
    {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);
    }

    Vector2 RightVelocity()                                                                         //Move Left/Right
    {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.right);
    }

        //-------------------------------------------------------------------------------------- Colliders

    void OnTriggerEnter2D(Collider2D other)                                     // If the Player comes in contact with...
    {        
        if (other.gameObject.CompareTag("PickUp"))                  // A squad
        {
            other.gameObject.SetActive(false);                          // The squad will deactivate
            count = count + 1;                                          // Rescues will be added by one more
            SetCountText();                                             // Count Text will be reset with the new number
        }
        if (other.gameObject.CompareTag("Mine"))                    // A mine
        {
            other.gameObject.SetActive(false);                          // Death...
            playerHealth = 0;
        }
        if (other.gameObject.CompareTag("RogueScrapHealth"))        // Scrap metal from a rogue tank
        {
            Destroy(other.gameObject);                                  // Destroy the other game object
            playerHealth += 25;                                         // Players health will go up by 25%
        }
        if (other.gameObject.CompareTag("LincsScrapHealth"))
        {
            Destroy(other.gameObject);                                  // Destroy the other game object
            playerHealth += 15;                                         // Players health will go up by 15%
        }
        if (other.gameObject.CompareTag("StormScrapHealth"))
        {
            Destroy(other.gameObject);                                  // Destroy the other game object
            playerHealth += 35;                                         // Players health will go up by 35%
        }
        if (other.gameObject.CompareTag("StormV2ScrapHealth"))
        {
            Destroy(other.gameObject);                                  // Destroy the other game object
            playerHealth += 55;                                         // Players health will go up by 55%
        }
        if (other.gameObject.CompareTag("DestroyedEnemyTank"))       
            reducedSpeed = true;       
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DestroyedEnemyTank"))
            reducedSpeed = false;
    }
        //-------------------------------------------------------------------------------------- Count Text

        void SetCountText()
    {        
        countText.text = "Rescued: " + count.ToString();                // Display "Rescued:" and the amount of squads rescued 

        if (count >= 7)                                                 // If you have rescued seven or more squads...
            countReady = true;                                          // Count ready bool will become true, enabling other scripts to activate dialogue, destructable rocks etc...

        if (count >= 8)                                                 // If you have rescued eight or more squads...
            levelComplete = true;                                       // The level is now completed
    }

        //-------------------------------------------------------------------------------------- Game Over

    public void EndGame()                                               
    {
        gameOverUI.SetActive(true);                                     // Display the game over UI 
    }
}