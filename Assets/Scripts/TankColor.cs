using UnityEngine;
using UnityEngine.SceneManagement;

public class TankColor : MonoBehaviour {

    [SerializeField]
    string hoverOverSound = "ButtonHover";      // Reference to the audio manager to play a sound upon hovering over a button.

    [SerializeField]
    string pressButtonSound = "ButtonPress";    // Reference to the audio manager to play a sound upon clicking on a button.

    [SerializeField]
    string Music = "MenuMusic";                 // Reference to the Menu Music

    AudioManager audioManager;                  // Reference to the Audio Manager

    void Start()
    {
        audioManager = AudioManager.instance;               // This allows this script to link to the audion manager to use its sounds

        if (audioManager == null)                           // If there is no audion manager in the scene a error will be displayed in the console
            Debug.LogError("No audiomanager found!");
    }

    public void StartGame1()                        // Called upon when player clicks on Tank 1 play
    {
        audioManager.PlaySound(pressButtonSound);   // Play this sound when the player clicks on Tank 1 play
        audioManager.StopSound(Music);              // Stop playing the Menu Music as we will no longer be in the Menu

        SceneManager.LoadScene("Tank 1 L1", LoadSceneMode.Single);      //This will change the scene, to go to Level 1
    }

    public void StartGame2()                        // Called upon when player clicks on Tank 2 play
    {
        audioManager.PlaySound(pressButtonSound);   // Play this sound when the player clicks on Tank 2 play
        audioManager.StopSound(Music);              // Stop playing the Menu Music as we will no longer be in the Menu

        SceneManager.LoadScene("Tank 2 L1", LoadSceneMode.Single);      //This will change the scene, to go to Level 1
    }

    public void StartGame3()                        // Called upon when player clicks on Tank 3 play
    {
        audioManager.PlaySound(pressButtonSound);   // Play this sound when the player clicks on Tank 3 play
        audioManager.StopSound(Music);              // Stop playing the Menu Music as we will no longer be in the Menu

        SceneManager.LoadScene("Tank 3 L1", LoadSceneMode.Single);      //This will change the scene, to go to Level 1
    }

    public void StartGame4()                        // Called upon when player clicks on Tank 4 play
    {
        audioManager.PlaySound(pressButtonSound);   // Play this sound when the player clicks on Tank 4 play
        audioManager.StopSound(Music);              // Stop playing the Menu Music as we will no longer be in the Menu

        SceneManager.LoadScene("Tank 4 L1", LoadSceneMode.Single);      // This will change the scene, to go to Level 1
    }

    public void QuitGame()                              // Called upon when player clicks on Quit
    {
        audioManager.PlaySound(pressButtonSound);       // Play this sound when the player clicks on Quit button

        Debug.Log("Application Quit!");                 // Console will log message saying application quit
        Application.Quit();                             // The application will now quit
    }

    public void OnMouseOver()                           // Called upon when the player hovers over the button he will click
    {
        audioManager.PlaySound(hoverOverSound);         // Sound it will play when the button is hovered over
    }
}
