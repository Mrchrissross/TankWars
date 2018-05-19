using UnityEngine;
using System.Collections;

public class Rock4Controller : MonoBehaviour {

    public PlayerController player;     // Reference to the players script
    public GameObject destroyedRock;    // Reference to the Destroyed Rock Prefab

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            if (player.countReady == true)              // If the players countReady bool has now true this will allow Rock 4 to be destructable
            {          
                Destroy(gameObject);
                Instantiate(destroyedRock);             // Once Rock 4 has been destroyed, it will be replaced with Destroyed Rock.
                this.enabled = false;
            }
        }       
    }
}
