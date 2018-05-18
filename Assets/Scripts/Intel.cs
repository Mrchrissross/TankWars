using UnityEngine;
using UnityEngine.UI;

public class Intel : MonoBehaviour {

    //This is the Intel of the level, Please use this as reference for the rest of the Intels. Intel 2, 3, 4 etc...

    public PlayerController player;         // Reference to the player's script
    public EnemyShooting enemyShoot;        // Reference to the Enemies Shooting Script
    public Health enemyHealth;              // Reference to the first enemies health
    public Rock4Controller Rock4;           // Reference to the Rock 4 script
    private AudioManager audioManager;      // Reference to the Audio Manager

    public Text Level1;                     // Reference to the Level 1 text which will be displayed in the middle of the screen, Commander's speech
    public Text Controls;                   // Reference to the Controls text which will only be displayed at the start
    float textTimer = 40f;                  // A timer to control exactly when text should appear on the players screen
    bool timerPaused = true;                // A bool to used to pause the text timer
    public float textTimer2 = 30f;          // A second timer to used only once rock 4 has been destroyed    

    [SerializeField]
    private GameObject nextLevelUI;         // Reference to the next level UI which will appear only once the level is complete.

    void Start ()
    {
        audioManager = AudioManager.instance;                                       // This allows the audio manager to be linked with this script with having a public link through unity UI
        if (audioManager == null)                                                   // If the audio manager is not in this scene, display error in console                                                                          
            Debug.LogError("AudioManager: No AudioManager found in this scene.");   // Error : "AudioManager: No AudioManager found in this scene."

        Level1.text = "";
    }

    void Update()
    {
        textTimer -= Time.deltaTime;        // Timer begins it's count down

        if (!timerPaused)                   // If the timer is not paused...
        textTimer2 -= Time.deltaTime;       // begin count down

        if (textTimer >= 39.5)                                              // Once the text timer has reached 39.5 display the message below
            Level1.text = "Commander: K2-Wolverine, do you read?!";      

        if (textTimer <= 37)      
            Level1.text = "";        

        if (textTimer <= 35)    
            Level1.text = "Commander: Ah, we have your signal, you made it!";       

        if (textTimer <= 31)        
            Level1.text = "Commander: Now that we have a clear connection, troops are scattered everywhere from the recent ambush.";        

        if (textTimer <= 26)        
            Level1.text = "Commander: We need you to perform a search and rescue, this operation is extremely dangerous as enemy insurgents are closing in.";        

        if (textTimer <= 18)
        {
            Level1.text = "Commander: Your tank should now be repaired. We've detected some troops near by to the east, pick them up and search the area for more.";
            player.combatReady = true;                                      // Player's bool combat ready becomes true and the player will be able to move
            Controls.text = "W = Forward \r\n  S = Back \r\n  A = Left \r\n D = Right \r\n Mouse = Aim \r\n Left-Click = Shoot";
        }

        if (textTimer <= 10)        
            Level1.text = "";        

        if (textTimer <= 0)
        {
            Level1.text = "";
            Controls.text = "";
        }

        if (player.countReady == true)                                      // If the player's bool count ready is true 
            Level1.text = "Commander: Thats all of them. Destroy the large boulder to the east, just north of the sea. We've planted C-4 to assist you.";        

        if (Rock4 != isActiveAndEnabled)                                    // If the Rock 4 gameobject has been destroyed
        {
            timerPaused = false;                                            // text timer 2 will be unpaused
            Level1.text = "Commander: Good job! But its not all good news, we've received intel that enemy forces are near by.";
        }

        if (textTimer2 <= 26)                                               // If text timer 2 is at 26 or under, display the message below
            Level1.text = "";        

        if (textTimer2 <= 25)        
            Level1.text = "Commander: Our radar shows that the enemy is approaching from the west. Head south-east and search for survivers. Approach with caution!";        

        if (textTimer2 <= 18)
        {
            timerPaused = true;                                             // Text timer 2 becomes paused
            Level1.text = "";
        }

        if (enemyShoot.playerspotted == true)
            Level1.text = "Commander: You've been spotted! Contact! Enemy has made Contact!";

        if (enemyHealth.enemyDead == true)
            Level1.text = "Commander: Target Destroyed. We have received a signal from a captured squad just north of your position, eliminate the captor and rescue our troops.";

        if (player.levelComplete == true)
        {
            Level1.text = "Commander: Congratulations on the success of your first mission, please await our next instructions.";
            player.combatReady = false;
            timerPaused = false;            
        }

        if (textTimer2 <= 13)
        {
            Level1.text = "";
            NextLevel();                                                    // Use NextLevel method
        }

        //---------------------------------------------------------------------------------------------------- Player Death

        if (player.playerHealth <= 0f)        
            Level1.text = "Commander: K2-Wolverine?! We have lost your signal. Do you read? K2-Wolverine!?!";   
            
    }

    public void NextLevel()                                                 // Next Level method
    {
        nextLevelUI.SetActive(true);                                        // Display the next level screen on the canvas
    }
}
