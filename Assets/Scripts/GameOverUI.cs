using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {

    [SerializeField]
    string mouseHoverSound = "ButtonHover";     // Reference to the audio manager to play a sound upon hovering over a button.

    [SerializeField]
    string buttonPressSound = "ButtonPress";    // Reference to the audio manager to play a sound upon clicking on a button.

    AudioManager audioManager;                  // Reference to the Audio Manager

    public string gameOver = "GameOver";            // Reference to the Game Over audio
    public string gameMusic = "GameAudio";          // Reference to the Game Music audio

    void Start()
    {
        audioManager = AudioManager.instance;                                       // This allows the audio manager to be linked with this script with having a public link through unity UI
        if (audioManager == null)                                                   // If the audio manager is not in this scene, display error in console                                                                          
            Debug.LogError("AudioManager: No AudioManager found in this scene.");   // Error : "AudioManager: No AudioManager found in this scene."

        audioManager.PlaySound(gameOver);            // Play NextLevel sound
        audioManager.StopSound(gameMusic);          // Stop playing the Game Music
    }

    public void Quit ()                             // Called upon when player clicks on Quit
    {
        audioManager.PlaySound(buttonPressSound);   // Play this sound when the player clicks Quit

        Debug.Log("Application Quit!");             // Console will log message saying application quit
        Application.Quit();                         // The application will now quit
    }

	public void Restart ()                                                  // Called upon when the player clicks restart
	{
        audioManager.PlaySound(buttonPressSound);                           // Play this sound when the player clicks Restart
        audioManager.StopSound(gameOver);                                   // Stop playing the Game Over Music
        audioManager.PlaySound(gameMusic);                                  // Play the background music

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   // Reload the scene, so that it appears to restart the level when the player clicks restart
	}

    public void OnMouseOver()                       // Called upon when the player hovers over the button he will click
    {
        audioManager.PlaySound(mouseHoverSound);    // Sound it will play when the button is hovered over
    }

}
