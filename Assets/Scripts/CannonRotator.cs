using UnityEngine;
using System.Collections;

public class CannonRotator : MonoBehaviour {

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;                                             //Used to get mouse position                                     
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);             //Used to get object position and put it on the screen               
        Vector3 offset = new Vector3(mousePos.x - screenPos.x, mousePos.y - screenPos.y);   //Check where the mouse is relative to the object 

        float angle = Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg;                      //Turn that into an angle and convert to degrees
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);                     //Set the object's rotation to be of the angle over -Z
    }
}