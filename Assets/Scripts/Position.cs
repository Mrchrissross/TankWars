using UnityEngine;
using System.Collections;

public class Position : MonoBehaviour {

    // This is the script used to set the Enemies position, to be set above the enemy itself

    public Health health;   // Reference to the Enemy Health

    public Transform enemy; // Enemy positioning
    Vector3 offset;         // Enemy positioning

    void Start () {
        offset = transform.position - enemy.position;   //Enemy Health Bar Positioning

    }
	
	void Update () {

        transform.position = (enemy.position + offset); // If the enemy moves the health will move with him without rotating

        if (health.enemyDead == true)
        {
            TurnOff();                                  // If the enemy is dead, the Health bar will turn off.
        }
    }

    void TurnOff()                                      // Method used to turn off the enemy health bar script
    {
        this.enabled = false;
    }
}
