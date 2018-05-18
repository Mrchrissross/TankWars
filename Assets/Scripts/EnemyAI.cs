using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
public class EnemyAI : MonoBehaviour {

    private Transform Player;       // Reference to our player for the enemy to chase

    [Header("Pathfinding: ")]
    public float updateRate = 2f;   // Path Update rate per second
    private Seeker seeker;          // Reference to our seeker script (A*)
    private Rigidbody2D rb;         // Reference to our enemies Rigidbody component
    public Path path;               // Enemies calculated path
    public float speed = 300f;      // The enemies speed
    public ForceMode2D fMode;       // Adding force to the enemy
    [HideInInspector]
    public bool pathIsEnded = false;                // Simple bool to tell when the path has ended
    public float nextWaypointDistance = 3;          // The maximum distance the enemy has to be for it to seek the next waypoint
    private int currentWaypoint = 0;                // Our current waypoint
    private bool hasWaypoint = false;               // Simple bool to tell if the enemy has a waypoint
    public EnemyPathFinding enemyPathFinding;       // Reference to our enemy pathfinding script, once the player has come into range it will then switch to this script and attempt to kill player
    public PlayerController playerController;       // Reference to our player, if the player has been successfully killed this script will turn off

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;      // Allows the player to be linked to this script with a tag, instead of through public link

        seeker = GetComponent<Seeker>();                                    // Used to reference/get our seeker script component
        rb = GetComponent<Rigidbody2D>();                                   // Used to reference/get our rigidbody component

        if (Player == null)                                                 // If player is none existant, start search for the player
        {

            if (!hasWaypoint)                                               // ^^
            {
                hasWaypoint = true;                                         // ^^
                StartCoroutine(SearchForWaypoint());                        
            }

            return;                                                         // ^^
        }

        seeker.StartPath(transform.position, Player.position, OnPathComplete);      // Start path to the player's position


        StartCoroutine(UpdatePath());                                               // Use the UpdatePath method
     
	}

    IEnumerator SearchForWaypoint()                                             // Seardch for next waypoint
    {
        GameObject sResult = GameObject.FindGameObjectWithTag("Waypoint");      // Find a GameObject with the tag Waypoint

        if (sResult == null)                                                    // If the GameObject with the tag Waypoint is none existant
        {
            yield return new WaitForSeconds(0.5f);                              // Search for waypoint again
            StartCoroutine(SearchForWaypoint());                                // ^^
        }
        else                                                                    // Else the variable sResult will now equal player
        {
            Player = sResult.transform;                                         // So the Enemy will now seek the player
            StartCoroutine(UpdatePath());
            hasWaypoint = false;
            return false;
        }
    }

    IEnumerator UpdatePath()                                                    // Update the Enemies Path
    {
        if (Player == null)                                                     // If player is none existant, start search for the player
        {
            if (!hasWaypoint)                                                   // ^^
            {
                hasWaypoint = true;                                             // ^^
                StartCoroutine(SearchForWaypoint());                            // ^^
            }
            return false;                                                       // ^^
        }

        seeker.StartPath(transform.position, Player.position, OnPathComplete);  // Start path to the player's position

        yield return new WaitForSeconds(1f / updateRate);                       // Update the path
        StartCoroutine(UpdatePath());                                           // ^^
    }

    public void OnPathComplete(Path p)                                          // If the path is complete then reset to 0
    {
        if (enemyPathFinding.playerInRange == true)                             
        {
            if (!p.error)                                                       // ^^
            {
                path = p;                                                       // ^^
                currentWaypoint = 0;                                            // ^^
            }
        }
    }

    void FixedUpdate()                                                          // This method will ensure the enemy moves and goes exactly where it needs to be
    {
        if (enemyPathFinding.playerInRange == true)                             // If the player is in range this part of the script turns on
        {
            if (Player == null)                                                 // If player is none existant, start search for the player
            {
                if (!hasWaypoint)                                               // ^^
                {
                    hasWaypoint = true;                                         // ^^
                    StartCoroutine(SearchForWaypoint());                        // ^^
                }
                return;                                                         //^^
            }

            if (path == null)                                                   // If there is no path the script
                return;                                                         // Loop

            if (currentWaypoint >= path.vectorPath.Count)                       // If the current waypoint is higher or equal to the Vector3 Path count...
            {
                if (pathIsEnded)                                                // And if the path has ended...
                    return;                                                     // Return the Value

                pathIsEnded = true;                                             // Path has ended
                return;
            }

            pathIsEnded = false;                                                // Path has not ended

            //------------------------------------------------------------------------------------ Enemy Movement

            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            dir *= speed * Time.fixedDeltaTime;

            transform.up = Vector3.RotateTowards(transform.up, Player.position - transform.position, speed * Time.deltaTime, 0.0f);

            rb.AddForce(dir, fMode);

            float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

            if (dist < nextWaypointDistance)
            {
                currentWaypoint++;
                return;                
            }

            //-------------------------------------------------------------------------------------

        }

        if (playerController.playerDead == true)                                // If the player has successfully been killed...
            TurnOff();                                                          // Use TurnOff method
    }

    void TurnOff()                                                              // Turnoff method will...
    {
        this.enabled = false;                                                   // Turn off this script
    }
}


