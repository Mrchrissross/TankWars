using UnityEngine;
using System.Collections;

public class ScrapHealth : MonoBehaviour {
// In this method we are telling it how long the explosion will go on for before the gameobject is destroyed. 35 seconds.
    void FixedUpdate()
    {
        Destroy(gameObject, 35.0f);
    }
}
