using UnityEngine;
using System.Collections;

public class PlayerStartDamage : MonoBehaviour {

    // This is the controller for the start damage when the player enters first level

    public PlayerController player;     // Reference to the player

    void Update()
    {
        if (player.combatReady == true)
        {
            gameObject.GetComponent<ParticleSystem>().enableEmission = false;       // if the player is now combat ready and fully repaired then this will turn off the smoke from the start
        }

    }
}
