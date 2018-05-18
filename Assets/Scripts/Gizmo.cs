using UnityEngine;
using System.Collections;

public class Gizmo : MonoBehaviour {

    // This allows me to see the pivot point of my cannon rotator.

    public float gizmoSize = .75f;              // Size of the Gizmo
    public Color gizmoColor = Color.yellow;     // Color of the Gizmo

	void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;                              // Assigns the color to the Gizmo
        Gizmos.DrawWireSphere(transform.position, gizmoSize);   // Assigns the size and shape of the Gizmo
	}
}
