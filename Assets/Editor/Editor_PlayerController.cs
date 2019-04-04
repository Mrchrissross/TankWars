using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayerController))]
[CanEditMultipleObjects]
public class Editor_PlayerController : Editor
{
    SerializedProperty styleProperty;
    SerializedProperty healthProperty;
    SerializedProperty damageProperty;
    SerializedProperty assetsProperty;

    int tempStyle = 0;

    void OnEnable()
    {
        // Setup the SerializedProperties
        styleProperty = serializedObject.FindProperty("style");
        healthProperty = serializedObject.FindProperty("playerHealth");
        damageProperty = serializedObject.FindProperty("bulletDamage");
        assetsProperty = serializedObject.FindProperty("m_Script");
    }

    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        EditorGUILayout.PropertyField(assetsProperty);
        EditorGUILayout.Space();

        // Show the custom GUI controls
        EditorGUILayout.IntSlider(styleProperty, 1, 8, new GUIContent("Style"));

        // Only show the style progress bar if all the object has the same value:
        if (!styleProperty.hasMultipleDifferentValues)
            ProgressBar(styleProperty.intValue / 8.0f, "Style");

        //EditorGUILayout.IntSlider(healthProperty, 0, 100, new GUIContent("Health"));
        if (!healthProperty.hasMultipleDifferentValues)
            ProgressBar(healthProperty.intValue / 100.0f, "Health");

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.

        DrawPropertiesExcluding(serializedObject, "m_Script", "style", "playerHealth", "bulletDamage");

        EditorGUILayout.IntSlider(damageProperty, 0, 100, new GUIContent("Damage"));
        if (!damageProperty.hasMultipleDifferentValues)
            ProgressBar(damageProperty.intValue / 100.0f, "Damage");

        ChangeModel();

        serializedObject.ApplyModifiedProperties();
    }

    // Custom GUILayout progress bar.
    void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18);
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }

    void ChangeModel()
    {
        PlayerController myScript = (PlayerController)target;

        if (myScript.style != tempStyle)
        {
            myScript.GetComponent<SpriteRenderer>().sprite = myScript.assets.Hulls[myScript.style - 1];
            myScript.transform.Find("Cannon").Find("PlayerTankCannon").GetComponent<SpriteRenderer>().sprite = myScript.assets.Cannons[myScript.style - 1];
            DestroyImmediate(myScript.transform.Find("Cannon").Find("PlayerTankCannon").GetComponent<PolygonCollider2D>());
            myScript.transform.Find("Cannon").Find("PlayerTankCannon").gameObject.AddComponent<PolygonCollider2D>();
            tempStyle = myScript.style;
        }
    }
}