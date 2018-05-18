using UnityEngine;
using System.Collections;

public class BulletExplosion : MonoBehaviour {
// In this method we are telling it how long the explosion will go on for before the gameobject is destroyed. 3 seconds.
void FixedUpdate()
    {
        Destroy(gameObject, 3.0f);
    }
}
