using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject player;

    private Vector3 offset;
//Here we are telling the camera to follow the player by offsetting the camera position to the players position.
	void Start ()
    {
        offset = transform.position - player.transform.position;
	}

	void LateUpdate ()
    {
        transform.position = player.transform.position + offset;
    }
}
