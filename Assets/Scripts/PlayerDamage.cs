using UnityEngine;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    //This is the full controller of how much damage the player will take from enemy bullets

    public PlayerController Player;     // Player reference, for player health

    [Header("Damage to Player: ")]  // <---
    public float rogue = 25f;       // Rogue Bullet
    public float lincs = 25f;       // Lincs Bullet
    public float storm = 25f;       // (Not Implemented)
    public float stormV2 = 25f;     // (Not Implemented)

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("RogueBullet"))     //If the player is hit by a rogue bullet
        {
            Player.playerHealth -= rogue;           // Player Health (100) minus Rogue Bullet (25) = 75
            Destroy(other.gameObject);              // Destroy the bullet
        }
        if (other.gameObject.CompareTag("LincsBullet"))     //If the player is hit by a Lincs bullet
        {
            Player.playerHealth -= lincs;           // Player Health (100) minus Lincs Bullet (25) = 75
            Destroy(other.gameObject);              // Destroy the bullet
        }
        if (other.gameObject.CompareTag("StormBullet"))     //If the player is hit by a Storm bullet (Not Implemented)
        {
            Player.playerHealth -= storm;           // Player Health (100) minus Storm Bullet (25) = 75
            Destroy(other.gameObject);              // Destroy the bullet
        }        
        if (other.gameObject.CompareTag("StormV2Bullet"))   //If the player is hit by a StormV2 bullet (Not Implemented)
        {
            Player.playerHealth -= stormV2;         // Player Health (100) minus StormV2 Bullet (25) = 75
            Destroy(other.gameObject);              // Destroy the bullet
        }
    }
}
