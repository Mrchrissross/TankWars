using UnityEngine;
using System.Collections;

public class DestroyedEnemyTank : MonoBehaviour {
// In this method we are telling it how long the explosion will go on for before the gameobject is destroyed. 20 seconds.
    void FixedUpdate()
    {
        Destroy(gameObject, 20.0f);
    }
}
