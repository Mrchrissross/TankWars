using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class Editor_ChangeModel : Editor
{
    int tempStyle = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerController myScript = (PlayerController)target;

        if(myScript.style != tempStyle)
        {
            myScript.GetComponent<SpriteRenderer>().sprite = myScript.assets.Hulls[myScript.style - 1];
            myScript.transform.Find("Cannon").Find("PlayerTankCannon").GetComponent<SpriteRenderer>().sprite = myScript.assets.Cannons[myScript.style - 1];
            DestroyImmediate(myScript.transform.Find("Cannon").Find("PlayerTankCannon").GetComponent<PolygonCollider2D>());
            myScript.transform.Find("Cannon").Find("PlayerTankCannon").gameObject.AddComponent<PolygonCollider2D>();
            tempStyle = myScript.style;
        }
    }
}