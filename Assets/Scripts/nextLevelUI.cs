using UnityEngine;
using UnityEngine.SceneManagement;

public class nextLevelUI : MonoBehaviour {

    [SerializeField]
    string mouseHoverSound = "ButtonHover";         // Reference to the audio manager to play a sound upon hovering over a button.

    [SerializeField]
    string buttonPressSound = "ButtonPress";        // Reference to the audio manager to play a sound upon clicking on a button.

    public string nextLevel;                        // A string used to indicate what the scene name of next level is
    public string success = "NextLevel";            // Reference to the next level audio
    public string GameMusic = "GameAudio";          // Reference to the Game Music audio

    AudioManager audioManager;                      // Reference to the Audio Manager

    void Start()
    {
        audioManager = AudioManager.instance;       // This allows this script to link to the audion manager to use its sounds
        if (audioManager == null)                   // If there is no audion manager in the scene a error will be displayed in the console       
            Debug.LogError("No audiomanager found!");

        audioManager.PlaySound(success);            // Play NextLevel sound
        audioManager.StopSound(GameMusic);          // Stop playing the Game Music
    }

    public void Restart()                           // Called upon when player clicks Restart
    {
        audioManager.PlaySound(buttonPressSound);   // Restarts the level

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()                         // Called upon when player clicks Next Level
    {
        audioManager.PlaySound(buttonPressSound);   // Starts next level

        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
    }

    public void OnMouseOver()                       // Called upon when the player hovers over the button he will click
    {
        audioManager.PlaySound(mouseHoverSound);    // Sound it will play when the button is hovered over
    }
}
