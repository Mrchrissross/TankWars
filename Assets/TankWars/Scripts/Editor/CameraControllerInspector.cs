using System;
using TankWars.Controllers;
using UnityEditor;
using UnityEngine;

namespace TankWars.Editor
{
    [CustomEditor(typeof(CameraController))]
    public class CameraControllerInspector : UnityEditor.Editor
    {
        private const float BoxMinWidth = 300f;
        private const float BoxMaxWidth = 1000f;
        
        private GUIStyle _boxStyle;
        private GUIStyle _foldoutStyle;
        
        private Color _guiColorBackup;
        
        private CameraController cameraController => target as CameraController;


        private void OnEnable()
        {
            // Save the original background color.
            _guiColorBackup = GUI.backgroundColor;
        }

        public override void OnInspectorGUI()
        {
            EditorTools.InitStyles(out _boxStyle, out _foldoutStyle);
            
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(target, "Camera Controller");
            
            DrawTargetSettings();
            DrawInputSettings();
            DrawMovementSettings();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUIUtility.labelWidth = 150;
            
            // base.OnInspectorGUI();
        }
        
        private void DrawTargetSettings()
        {
            EditorTools.Header("Target");

            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                GUILayout.BeginHorizontal();
                {
                    cameraController.cameraTarget = EditorTools.TransformField("Target:",
                        "This is the current target that the camera will follow.",
                        cameraController.cameraTarget, 60);
                    
                    if(EditorTools.Button("Reset", "Resets the target to look at the tank."))
                        cameraController.ResetTarget();
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginVertical("box");
                {
                    EditorTools.Toggle("Rotate With Target?",
                        "If enabled, the camera will rotate with the target on the z axis.",
                        ref cameraController.rotateWithTarget, 120);
                    
                    GUILayout.Space(2.5f);
                    
                    GUILayout.BeginHorizontal();
                    {
                        EditorTools.Label("Offset:", "Offsets the cameras position relative to the target.", 50, 0);

                        var positionOffset = cameraController.PositionOffset;
                        positionOffset.x = EditorTools.FloatField("X", "", positionOffset.x, 20f);
                        positionOffset.y = EditorTools.FloatField("Y", "", positionOffset.y, 20f);
                        cameraController.PositionOffset = positionOffset;
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(5.0f);
                    
                    cameraController.CameraDistance = EditorTools.Slider("Camera Distance",
                        "This is the maximum distance that the camera will extend from the target.",
                        cameraController.CameraDistance, 15.0f, 40.0f, 120);
                    
                    EditorTools.DrawLine(0.5f, 2.5f, 2.5f);
                    
                    EditorTools.Toggle("Far Look?",
                        "When pointing at a certain direction, the camera will offset to look further off in the distance.",
                        ref cameraController.farLook, 120);
                    
                    cameraController.farLookSpeed = EditorTools.Slider("Far Look Speed",
                        "This is the speed that the camera will offset from the target while using far look.",
                        cameraController.farLookSpeed, 0.0f, 20.0f, 120);
                    
                    cameraController.farLookDistance = EditorTools.Slider("Far Look Distance",
                        "This is the maximum distance that the camera will offset from the target while using far look.",
                        cameraController.farLookDistance, 0.0f, 30.0f, 120);
                
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
        
        private void DrawInputSettings()
        {
            EditorTools.Header("Input");

            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    var cursorLabel = cameraController.lockCursor ? "<b>Cursor Locked</b>" : "Cursor Not Locked";
                    EditorTools.Label(cursorLabel, "Displays whether the mouse cursor is locked to the screen or not.",
                        120, 0, new GUIStyle(GUI.skin.label)
                        {
                            alignment = TextAnchor.MiddleCenter,
                            richText = true
                        });
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();

                EditorTools.DrawLine(0.5f, 2.5f, 2.5f);

                cameraController.UnlockCursorKey = EditorTools.KeyCodeDropdown("Cursor Unlock Key",
                    "The keyboard key to unlock the mouse cursor.", cameraController.UnlockCursorKey, 110);

                EditorTools.DrawLine(0.5f, 2.5f, 0);

                EditorTools.Label("Joystick:", "Ensure these settings match those that are within the input manager.",
                    60);

                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorTools.Label("Invert:",
                            "Inverts the input on select axis.", 95);

                        EditorTools.Toggle("Horizontal", "",
                            ref cameraController.joystickInvertHorizontal, 75);

                        EditorTools.Toggle("Vertical", "",
                            ref cameraController.joystickInvertVertical, 60);
                        GUILayout.Space(-10);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2.5f);


                    cameraController.joystickHorizontalInput = EditorTools.StringField("Horizontal",
                        "This is the horizontal joystick input found in the input manager.",
                        cameraController.joystickHorizontalInput, 100);

                    cameraController.joystickVerticalInput = EditorTools.StringField("Vertical",
                        "This is the vertical joystick input found in the input manager.",
                        cameraController.joystickVerticalInput, 100);

                    EditorGUILayout.Space(7.5f);

                    cameraController.deadZoneThreshold = EditorTools.Slider(" Dead Zone Threshold",
                        "This is the dead zone threshold of the joystick, if the joystick input is below this " +
                        "threshold, no input is applied.", cameraController.deadZoneThreshold, 0.05f, 0.5f, 150);

                }
                GUILayout.EndVertical();
                
                EditorTools.DrawLine(0.5f, 2.5f, 0);
                
                EditorTools.Label("Actions:",
                    "Ensure these settings match those that are within the input manager.", 100);
                    
                GUILayout.BeginVertical("box");
                {
                    cameraController.farLookInput = EditorTools.StringField("Far Look",
                        "Input to change the screen offset from positional to mouse/joystick.",
                        cameraController.farLookInput, 100);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }

        private void DrawMovementSettings()
        {
            EditorTools.Header("Following Target");

            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (EditorTools.Foldout("Smooth Movement", "If enabled, the cameras movement toward the target will be dampened.", 
                    ref cameraController.smoothMovement, true, true))
                {
                    
                    GUI.backgroundColor = Color.black;
                    EditorGUILayout.BeginVertical(_foldoutStyle);
                    {
                        GUI.backgroundColor = _guiColorBackup;
                        
                        EditorGUI.indentLevel++;

                        EditorGUIUtility.labelWidth = 150;
                        cameraController.followMethod = (CameraController.FollowMethod) EditorGUILayout.EnumPopup(new GUIContent("Follow Method",
                    "The method used to follow the target."),
                            cameraController.followMethod);

                        GUILayout.Space(2.25f);
                        
                        if (cameraController.followMethod == CameraController.FollowMethod.Lerp)
                            cameraController.lerpMovementSpeed = EditorTools.Slider("Speed",
                                "The speed at which the camera will be linearly interpolated toward the camera.",
                                cameraController.lerpMovementSpeed, 0.5f, 50.0f, 150);

                        else
                            cameraController.smoothMovementTime = EditorTools.Slider("Damp Time",
                                "The amount time that the camera will lag behind the target.", 
                                cameraController.smoothMovementTime, 0.0f, 1.5f, 150);
                        
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }
                
                EditorTools.DrawLine(0.5f, 5, 2.5f);
                
                if (EditorTools.Foldout("Smooth Rotation", "Smoothly moves the characters rotation toward the target rotation.", 
                    ref cameraController.smoothRotation, true, true))
                {
                    
                    GUI.backgroundColor = Color.black;

                    EditorGUILayout.BeginVertical(_foldoutStyle);
                    {
                        GUI.backgroundColor = _guiColorBackup;
                        
                        EditorGUI.indentLevel++;

                        EditorGUILayout.Space(1f);
                        
                        cameraController.smoothRotationFactor = EditorTools.Slider("Time",
                            "Applies a drag to the camera, making it take longer to reach the target rotation.",
                            cameraController.smoothRotationFactor, 0, 10.0f, 100);

                        EditorGUILayout.Space(5);

                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
    }
}