using UnityEngine;
using System.Collections;

public class EnemyPathFinding : MonoBehaviour
{
    public Transform[] wayPointList;        // Reference to our waypoints, the list is made as an array to control how many waypoint we want
    public Transform player;                // Reference to our player

    public int currentWayPoint = 0;         // Which waypoint the enemy is currently eading to
    Transform targetWayPoint;               // the waypoint that the enemy is currently targeting

    public float speed = 4f;                // Speed of the enemy
    public float range = 10f;               // Range at which the player will be seen on the enemies radar
    public bool playerInRange;              // Simple bool telling if the player is in the enemies range
    
    void Update()
    {

        float dist = Vector3.Distance(player.position, transform.position);     // This is the distance between the enemy and the player

        if (currentWayPoint < this.wayPointList.Length)                         // Check if the enemies waypoint and whereabouts in the list
        {
            if (targetWayPoint == null)                                         // If the enemies target waypoint is none existent then the it will go to default currentwaypoint in the waypoint list
                targetWayPoint = wayPointList[currentWayPoint];
            Move();                                                             // Use the Move method
        }

        if (currentWayPoint == 7)                                               // If the current waypoint has gotten to seven, it will then go back to zero
        {
            currentWayPoint = 0;
        }

        if (dist <= range)                                                      // If the distance between the player and enemy goes below or equal to the range variable...
        {
            currentWayPoint = 8;                                                // The current waypoint will be come eight
            playerInRange = true;                                               // The Player in Range bool will become true (EnemyAI script will become active)
            TurnOff();                                                          // This script will turn off
        }
    }

    void Move()
    {        
        transform.up = Vector3.RotateTowards(transform.up, targetWayPoint.position - transform.position, speed * Time.deltaTime, 0.0f); // Enemy rotation, so that it is looking at its target

        transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, speed * Time.deltaTime);                  // Enemy movement, so that it moves toward its location

        if (transform.position == targetWayPoint.position)                                                                              // If the Enemy has reached the target way point and has same position on the x, y, z...
        {
            currentWayPoint++;                                                                                                          // Current way point will go up by one and the enemy will now start moving toward the next way point
            targetWayPoint = wayPointList[currentWayPoint];                                                                             // Reset the target way point to be that of the current waypoint
        }
    }

    void TurnOff()                                                                              // Turn off methos will...
    {
        this.enabled = false;                                                                   // Turn off this script
    }
}