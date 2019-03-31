using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Tooltip("The bullets explosion when it collides with an object.")]
    public GameObject bulletExplosion;

    [Tooltip("How fast the bullet travels.")]
    public float bulletSpeed = 40f;

    [Tooltip("How much damage the bullet does.")]
    public float bulletDamage = 20f;

    [Tooltip("How long the bullet lives.")]
    public float bulletLife = 2f;

    void Start()
    {
        Destroy(gameObject, bulletLife);
    }

    void Update()
    {
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Add triggers here.

        // Rock 1 is the objects name.
        if (other.gameObject.CompareTag("Rock 1"))
        {
            Destroy(gameObject);
            Instantiate(bulletExplosion, transform.position, transform.rotation = Quaternion.identity);
            AudioManager.instance.PlaySound("BulletExplosion");
        }
    }
}

