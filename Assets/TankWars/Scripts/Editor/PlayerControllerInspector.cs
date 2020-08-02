using UnityEngine;
using UnityEditor;
using TankWars.Controllers;

namespace TankWars.Editor
{
    [CustomEditor(typeof(PlayerController)), CanEditMultipleObjects]
    public class PlayerControllerInspector : UnityEditor.Editor
    {
        // SerializedProperty styleProperty;
        // SerializedProperty healthProperty;
        // SerializedProperty damageProperty;
        // SerializedProperty assetsProperty;
        //
        // int tempStyle = 0;
        //
        // void OnEnable()
        // {
        //     // Setup the SerializedProperties
        //     styleProperty = serializedObject.FindProperty("style");
        //     healthProperty = serializedObject.FindProperty("playerHealth");
        //     damageProperty = serializedObject.FindProperty("bulletDamage");
        //     assetsProperty = serializedObject.FindProperty("m_Script");
        // }
        //
        // public void OnInspectorGUI()
        // {
        //     // Update the serializedProperty - always do this at the beginning of OnInspectorGUI.
        //     serializedObject.Update();
        //
        //     EditorGUILayout.PropertyField(assetsProperty);
        //     EditorGUILayout.Space();
        //
        //     //EditorGUILayout.IntSlider(healthProperty, 0, 100, new GUIContent("Health"));
        //     if (!healthProperty.hasMultipleDifferentValues)
        //         ProgressBar(healthProperty.intValue / 100.0f, "Health");
        //
        //     EditorGUILayout.IntSlider(styleProperty, 1, 8, new GUIContent("Style"));
        //
        //     // Draw all the properties excluding the ones shown below.
        //     DrawPropertiesExcluding(serializedObject, "m_Script", "style", "playerHealth");
        //
        //     // Change the tank model.
        //     ChangeModel();
        //
        //     // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        //     serializedObject.ApplyModifiedProperties();
        // }
        //
        // /// <summary>
        // /// Custom GUILayout progress bar.
        // /// </summary>
        // /// <param name="value">Current amount.</param>
        // /// <param name="label">The text that appear on the bar.</param>
        // void ProgressBar(float value, string label)
        // {
        //     // Get a rect for the progress bar using the same margins as a textfield:
        //     Rect rect = GUILayoutUtility.GetRect(18, 18);
        //     EditorGUI.ProgressBar(rect, value, label);
        //     EditorGUILayout.Space();
        // }
        //
        // /// <summary>
        // /// Changes the tanks style.
        // /// </summary>
        // void ChangeModel()
        // {
        //     var myScript = (PlayerController)target;
        //
        //     if (myScript.style != tempStyle)
        //         myScript.ChangeStyle();
        // }
    }
}
