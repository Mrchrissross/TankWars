using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{

    // This is the script which controls the enemies health

    public bool enemyDead;                      // Simple bool to state when the enemy is firing, for reference to other scripts

    [Header("Stats: ")]
    public int curHealth;                       // This is the enemies current health
    public int maxHealth = 100;                 // This is the enemeis maximum health
    public int damageReceived = 25;             // This is how much damage the enemy will receive when hit by the player
    public string deathAudio;                   // This is the reference to the audi that will be played when the enemies health reaches zero

    [SerializeField]                            // Used to make a variable appear on the inspector even though it is private
    private Transform enemy;                    // This is used to tell the script the position of the enemy
    [SerializeField]
    private GameObject tankExplosion;           // This is the prefab that will be used when the tank explodes
    [SerializeField]
    public GameObject destroyedTank;            // This is the prefab that will be used when the tank is destroyed 
    [SerializeField]
    public GameObject playerHealth;             // This is a prefab that will be spawned with the destroyedTank object and will be a pickup item for the player and give the player health 

    private AudioManager audioManager;          // Reference to our Audio Manager, to play our audio    

    [Header("Required: ")]      
    [SerializeField]
    private StatusIndicator statusIndicator;    // Reference to our Status Indicator to display enemies health

    public void Init()                          // Initialization method
    {
        curHealth = maxHealth;                  // Current health will equal maxhealth
    }

    void Start()
    {
        audioManager = AudioManager.instance;                                       // This allows the audio manager to be linked with this script with having a public link through unity UI

        if (audioManager == null)                                                   // If the audio manager is not in this scene, display error in console
        {
            Debug.LogError("AudioManager: No AudioManager found in this scene.");   // Error : "AudioManager: No AudioManager found in this scene."
        }

        Init();                                                                     // Run Init method

        if (statusIndicator != null)                                                // If the status indicator has been referenced and is active...
        {
            statusIndicator.SetHealth(curHealth, maxHealth);                        // Set the values of current health and max health to the variables within the indicator
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Bullet"))                                                      // If the enemy has come into contact with a game object with the tag name "Bullet"...
        {
            curHealth -= damageReceived;                                                                // Current health will be deducted by the amount of the damage received variable
            Destroy(other.gameObject);                                                                  // The other game object (bullet) will be destroyed

            if (curHealth <= 0f)                                                                        // If the enemies current health has become equal or less than zero...
            {
                Destroy(gameObject);                                                                    // The enemy will be destroyed
                Instantiate(tankExplosion, enemy.position, transform.rotation = Quaternion.identity);   // A tank explosion will be instatiated at the location of the enemy
                audioManager.PlaySound(deathAudio);                                                     // A tank destruction audio will play
                Instantiate(destroyedTank, enemy.position, enemy.rotation);                             // The destroyed tank prefab will instantiate at the enemies position
                enemyDead = true;                                                                       // The bool enemyDead will become true
                Instantiate(playerHealth, enemy.position, enemy.rotation);                              // Player health prefab will spawn close to the wreckage

                if (SceneManager.GetActiveScene().name == "Infinite Mode")
                    GameObject.Find("GameMaster").GetComponent<InfiniteMode>().count += 1f;                 // If in Infinite Mode, the score will go up by one. 
            }
        }

        if (statusIndicator != null)                                                // If the status indicator has been referenced and is active...
        {
            statusIndicator.SetHealth(curHealth, maxHealth);                        // Set the values of current health and max health to the variables within the indicator
        }

    }
}
