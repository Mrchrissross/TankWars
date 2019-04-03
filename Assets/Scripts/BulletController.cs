using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Tooltip("How fast the bullet travels.")]
    public float bulletSpeed = 40f;

    [Tooltip("How much damage the bullet does.")]
    public float bulletDamage = 20f;

    void Update()
    {
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }

    void explode()
    {
        Destroy(gameObject);
        AssetManager.instance.SpawnObject("BulletExplosion", transform.position, transform.rotation);
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
                    AssetManager.instance.SpawnObject("MineExplosion", other.transform.position, other.transform.rotation);
                    Destroy(other.gameObject);
                    break;
                }
        }
    }
}

