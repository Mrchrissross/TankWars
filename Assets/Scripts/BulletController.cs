using UnityEngine;

public class BulletController : MonoBehaviour
{
    [HideInInspector, Tooltip("How much damage the bullet does.")]
    public float bulletDamage = 20f;
    [HideInInspector, Tooltip("How fast the bullet travels.")]
    public float bulletSpeed = 40f;

    void Update()
    {
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Add triggers here.

        switch(other.gameObject.tag)
        {
            case "Rocks":
                {
                    explode();
                    break;
                }
            case "Mine":
                {
                    explode();
                    AssetManager.Instance.SpawnObject("MineExplosion", other.transform.position, other.transform.rotation);
                    Destroy(other.gameObject);
                    break;
                }
        }
    }

    /// <summary>
    /// Triggers an explosion by destroying the game object and spawning an explosion animation.
    /// </summary>
    void explode()
    {
        Destroy(gameObject);
        AssetManager.Instance.SpawnObject("BulletExplosion", transform.position, transform.rotation);
    }
}

