using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{
//Variables. (self-explan)
    public GameObject bulletExplosion;

    public Transform bullet;

    public float bulletSpeed = 20f;

    public string shotAudio;

    public float bulletLife = 2f;

    private AudioManager audioManager;

//This method will be called upon as soon as the bullet comes into existance and will be destroyed once the bullet's life has expired. Any bullet already in the game will start as any bullet except from its start position.
    void Start()
    {
        Destroy(gameObject, bulletLife);

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager: No AudioManager found in this scene.");
        }
    }
//This method allows the bullet to move the way it does, and how fast.
    void Update()
    {
        transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime);
    }
//This method is used to tell the bullet to expire if it ever hits a 2D Collider. If it does, then it will automatically expire and be replaced, instantiate an explosion at itsw position.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Rock 1"))
        {
            Destroy(gameObject);
            Instantiate(bulletExplosion, bullet.position, transform.rotation = Quaternion.identity);
            audioManager.PlaySound(shotAudio);
        }
        if (other.gameObject.CompareTag("Rock 2"))
        {
            Destroy(gameObject);
            Instantiate(bulletExplosion, bullet.position, transform.rotation = Quaternion.identity);
            audioManager.PlaySound(shotAudio);
        }
        if (other.gameObject.CompareTag("Rock 3"))
        {
            Destroy(gameObject);
            Instantiate(bulletExplosion, bullet.position, transform.rotation = Quaternion.identity);
            audioManager.PlaySound(shotAudio);
        }
        if (other.gameObject.CompareTag("Rock 4"))
        {
            Destroy(gameObject);
            Instantiate(bulletExplosion, bullet.position, transform.rotation = Quaternion.identity);
            audioManager.PlaySound(shotAudio);
        }
        if (other.gameObject.CompareTag("Rock 5"))
        {
            Destroy(gameObject);
            Instantiate(bulletExplosion, bullet.position, transform.rotation = Quaternion.identity);
            audioManager.PlaySound(shotAudio);
        }
    }
}

