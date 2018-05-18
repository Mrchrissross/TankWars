using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour
{
    // This script is the controller for the enemy health

    [Header("Stats: ")]             // <---
    public int curHealth;
    public int maxHealth = 100;
    public int damageReceived = 25;

    [Header("Texts: ")]             // <---
    public Text Level1;

    public void Init()              // Sets the enemy max health.
    {
        curHealth = maxHealth;
    }

    [Header("Optional: ")]
    [SerializeField]
    private StatusIndicator statusIndicator;    // Reference to the Health bar above enemy

    void Start()
    {

        Init();                                                 // Use this method

        if (statusIndicator != null)                            // If the is no Health bar is not zero, set health bar to current health depending on how much damage you have taken
        {
            statusIndicator.SetHealth(curHealth, maxHealth);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Bullet"))              // If the enemy has taken damage from the players bullet it will...
        {
            curHealth -= damageReceived;                        // Have is health deduced by how much damage it has taken from the player.
            Destroy(other.gameObject);                          // Destroy the bullet

            if (curHealth <= 0f)                                // If the enemies health  is lower or equal to zero it will...
            {
                Destroy(gameObject);                            // Destroy the enemies tank
                Level1.text = "Commander: Target Destroyed. We have received a signal from a captured squad just north of your position, eliminate the captor and rescue our troops.";              // Display a text from the commander if the text has been referenced.
            }
        }

        if (statusIndicator != null)                            // If the Health bar is not zero display the current health withe its max health
        {
            statusIndicator.SetHealth(curHealth, maxHealth);
        }

    }
}