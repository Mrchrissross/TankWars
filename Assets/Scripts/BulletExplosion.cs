﻿using UnityEngine;

public class BulletExplosion : MonoBehaviour
{
    // Automatically destroys the explosion gameobject after 3 seconds.
void FixedUpdate()
    {
        Destroy(gameObject, 3.0f);
    }
}
