using UnityEngine;

public class Spawner : MonoBehaviour
{ 
    public PlayerController player;         // Reference to the player's heatlh.
    public GameObject[] enemy;                // The enemy prefab to be spawned.
    public float spawnTime = 3f;            // How long between each spawn.
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
    //private InfiniteMode IM;                // Link to the Infinite Mode Script.


    void Start()
    {
        // Assigns the variable IM to find the Infinite Mode Script.
        //InfiniteMode IM = GetComponent<InfiniteMode>();

        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }


    void Spawn()
    {
        // If the player has no health left...
        if (player.playerHealth <= 0)
        {
            // ... exit the function.
            return;
        }

        // Find a random index between zero and one less than the number of spawn points.
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        // allows the enemy to spawn at a point in the area around the index of the set spawn point. - Lincs
        Instantiate(enemy[0], spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);

        // If the players score is higher or equal to ten then the spawner will spawn two at a time. - Rogue Tank + Lincs Tank.
        if (GetComponent<InfiniteMode>().count >= 10f)
        Instantiate(enemy[1], spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
}
