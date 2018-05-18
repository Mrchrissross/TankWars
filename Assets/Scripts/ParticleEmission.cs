using UnityEngine;
using System.Collections;

public class ParticleEmission : MonoBehaviour {
    private float shot;
    public PlayerController player;
    public GunController myGun;


    void Update()
    {
        if (player.combatReady == true)
        {

            shot = shot - .1f;
            if (Input.GetMouseButtonUp(0))
            {
                gameObject.GetComponent<ParticleSystem>().enableEmission = true;
                shot = 2f;
            }

            if (shot < 0f)
            {
                gameObject.GetComponent<ParticleSystem>().enableEmission = false;
            }
        }
        else
            gameObject.GetComponent<ParticleSystem>().enableEmission = false;
    }
}
