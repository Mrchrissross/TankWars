using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public Toggle Music;

    [SerializeField]
    string hoverOverSound = "ButtonHover";          // Reference to the audio manager to play a sound upon hovering over a button.

    [SerializeField]
    string pressButtonSound = "ButtonPress";        // Reference to the audio manager to play a sound upon clicking on a button.

    [SerializeField]
    string MenuMusic = "MenuMusic";                 // Reference to the Menu Music

    AudioManager audioManager;                      // Reference to the Audio Manager

    void Start()
    {
        audioManager = AudioManager.instance;       // This allows this script to link to the audion manager to use its sounds
        if (audioManager == null)                   // If there is no audion manager in the scene a error will be displayed in the console       
            Debug.LogError("No audiomanager found!");          
    }

    public void StartGame ()                        // Called upon when player clicks play
    {
        audioManager.PlaySound(pressButtonSound);   // Play this sound when the player clicks the button

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

    public void InfiniteMode()                      // Called upon when player clicks Infinte Mode
    {
        audioManager.PlaySound(pressButtonSound);   // Called upon when player clicks play
        audioManager.StopSound(MenuMusic);          // Stop playing the Menu Music as we will no longer be in the Menu

        SceneManager.LoadScene("Infinite Mode", LoadSceneMode.Single);
    }

    public void QuitGame()                          // Called upon when player clicks on Quit
    {
        audioManager.PlaySound(pressButtonSound);   // Play this sound when the player clicks on Quit button

        Debug.Log("Application Quit!");             // Console will log message saying application quit
        Application.Quit();                         // The application will now quit
    }

    public void OnMouseOver()                       // Called upon when the player hovers over the button he will click
    {
        audioManager.PlaySound(hoverOverSound);     // Sound it will play when the button is hovered over
    }

    public void ActiveToggle()                      // A toggle to toggle music on and off
    {
        if(Music.isOn)                              // Music On
            audioManager.PlaySound("MenuMusic");    // Play Music
        if(!Music.isOn)                             // Music Off
            audioManager.StopSound("MenuMusic");    // Stop Music
    }
}
