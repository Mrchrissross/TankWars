using UnityEngine;
using UnityEngine.UI;

public class InfiniteMode : MonoBehaviour
{
    public float count;                         // Score
    public Text countText;                      // Reference to the Score Text
    public PlayerController player;             // Reference to the Player
    public string GameAudio = "GameAudio";      // Reference to the Game Music audio
    private AudioManager audioManager;          // Reference to the Audio Manager

    void Start()
    {
        audioManager = AudioManager.instance;                                       // This allows the audio manager to be linked with this script with having a public link through unity UI
        if (audioManager == null)                                                   // If the audio manager is not in this scene, display error in console                                                                          
            Debug.LogError("AudioManager: No AudioManager found in this scene.");   // Error : "AudioManager: No AudioManager found in this scene."
        audioManager.PlaySound(GameAudio);                                          // Play the background music

        count = 0f;                                                                 // Set the score to zero when this script becomes active
        SetCountText();                                                             // Call upon this method

        player.combatReady = true;                                                  // Set the player combat ready bool (toggle) to true.
    }

    void Update()
    {
        SetCountText();                                                             // Call upon your SetCountText method
    }

    void SetCountText()
    {
        countText.text = "Score: " + count.ToString();                              // Set the Count Text to "Score: ~~"
    }
}
