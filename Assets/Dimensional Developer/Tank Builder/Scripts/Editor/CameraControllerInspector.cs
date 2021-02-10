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
        
        private bool _customTankController;
        
        private CameraController CameraController => target as CameraController;

        private void OnEnable() => EditorTools.InitTextures();

        public override void OnInspectorGUI()
        {
            EditorTools.InitStyles(out _boxStyle, out _foldoutStyle);
            
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(target, "Camera Controller");
            
            DrawTargetSettings(0);
            DrawInputSettings(1);
            DrawMovementSettings(2);

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUIUtility.labelWidth = 150;
            
            // base.OnInspectorGUI();
        }
        
        private void DrawTargetSettings(int section)
        {
            GUILayout.BeginHorizontal();
            {
                if (!CameraController.hideSection[section])
                {
                    if (EditorTools.TexturedButton(EditorTools.eyeOpenTexture,
                        "Hides all the content in this section.", 20f))
                        CameraController.hideSection[section] = true;
                    
                    GUILayout.Space(-20);
                }
                else
                {
                    if (EditorTools.TexturedButton(EditorTools.eyeClosedTexture,
                        "Shows all the content in this section.", 20f))
                        CameraController.hideSection[section] = false;
                    
                    GUILayout.Space(-20);
                }
                
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 13
                };
            
                EditorGUILayout.LabelField(new GUIContent("Targeting", ""), style);

                if (!_customTankController)
                {
                    GUILayout.Space(-20);

                    if (EditorTools.TexturedButton(EditorTools.tankTexture,
                        "Manually input the Tank Controller into the camera system.", 20f))
                        _customTankController = true;
                }
            }
            GUILayout.EndHorizontal();
            
            EditorTools.DrawLine();
            GUILayout.Space(5);

            if (CameraController.hideSection[section]) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (_customTankController)
                {
                    EditorTools.Label("Tank Controller:", "The tank controller to be used in this camera system.", 100);
                    
                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUIUtility.labelWidth = 100;
                            CameraController.tankController = (TankController) EditorGUILayout.ObjectField(new GUIContent("",
                                ""), CameraController.tankController, typeof(TankController), true);

                            if (EditorTools.Button("Done", ""))
                                _customTankController = false;
                        }
                        GUILayout.EndHorizontal();

                    }
                    GUILayout.EndVertical();
                    
                    EditorTools.DrawLine(0.5f, 2.5f, 0);
                }
                
                GUILayout.BeginHorizontal();
                {
                    CameraController.cameraTarget = EditorTools.TransformField("Target:",
                        "This is the current target that the camera will follow.",
                        CameraController.cameraTarget, 60);
                    
                    if(EditorTools.Button("Reset", "Resets the target to look at the tank."))
                        CameraController.ResetTarget();
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginVertical("box");
                {
                    EditorTools.Toggle("Rotate With Target?",
                        "If enabled, the camera will rotate with the target on the z axis.",
                        ref CameraController.rotateWithTarget, 120);
                    
                    GUILayout.Space(2.5f);
                    
                    GUILayout.BeginHorizontal();
                    {
                        EditorTools.Label("Offset:", "Offsets the cameras position relative to the target.", 50, 0);

                        var positionOffset = CameraController.PositionOffset;
                        positionOffset.x = EditorTools.FloatField("X", "", positionOffset.x, 20f);
                        positionOffset.y = EditorTools.FloatField("Y", "", positionOffset.y, 20f);
                        CameraController.PositionOffset = positionOffset;
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(5.0f);
                    
                    CameraController.CameraDistance = EditorTools.Slider("Camera Distance",
                        "This is the maximum distance that the camera will extend from the target.",
                        CameraController.CameraDistance, 15.0f, 40.0f, 120);
                    
                    EditorTools.DrawLine(0.5f, 2.5f, 2.5f);
                    // GUILayout.BeginHorizontal();
                    // {
                        EditorTools.Toggle("Far Look?",
                            "When pointing at a certain direction, the camera will offset to look further off in the distance.",
                            ref CameraController.farLook, 120);
                    // }
                    // GUILayout.EndHorizontal();
                    
                    CameraController.farLookSpeed = EditorTools.Slider("Far Look Speed",
                        "This is the speed that the camera will offset from the target while using far look.",
                        CameraController.farLookSpeed, 0.0f, 20.0f, 120);
                    
                    CameraController.farLookDistance = EditorTools.Slider("Far Look Distance",
                        "This is the maximum distance that the camera will offset from the target while using far look.",
                        CameraController.farLookDistance, 0.0f, 30.0f, 120);
                
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
        
        private void DrawInputSettings(int section)
        {
            if (EditorTools.DrawHeader("Input", ref CameraController.hideSection[section])) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    var cursorLabel = CameraController.lockCursor ? "<b>Cursor Locked</b>" : "Cursor Not Locked";
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

                CameraController.UnlockCursorKey = EditorTools.KeyCodeDropdown("Cursor Unlock Key",
                    "The keyboard key to unlock the mouse cursor.", CameraController.UnlockCursorKey, 110);

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
                            ref CameraController.joystickInvertHorizontal, 75);

                        EditorTools.Toggle("Vertical", "",
                            ref CameraController.joystickInvertVertical, 60);
                        GUILayout.Space(-10);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2.5f);


                    CameraController.joystickHorizontalInput = EditorTools.StringField("Horizontal",
                        "This is the horizontal joystick input found in the input manager.",
                        CameraController.joystickHorizontalInput, 100);

                    CameraController.joystickVerticalInput = EditorTools.StringField("Vertical",
                        "This is the vertical joystick input found in the input manager.",
                        CameraController.joystickVerticalInput, 100);

                    EditorGUILayout.Space(7.5f);

                    CameraController.deadZoneThreshold = EditorTools.Slider(" Dead Zone Threshold",
                        "This is the dead zone threshold of the joystick, if the joystick input is below this " +
                        "threshold, no input is applied.", CameraController.deadZoneThreshold, 0.05f, 0.5f, 150);

                }
                GUILayout.EndVertical();
                
                EditorTools.DrawLine(0.5f, 2.5f, 0);
                
                EditorTools.Label("Actions:",
                    "Ensure these settings match those that are within the input manager.", 100);
                    
                GUILayout.BeginVertical("box");
                {
                    CameraController.farLookInput = EditorTools.StringField("Far Look",
                        "Input to change the screen offset from positional to mouse/joystick.",
                        CameraController.farLookInput, 100);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }

        private void DrawMovementSettings(int section)
        {
            if (EditorTools.DrawHeader("Movement", ref CameraController.hideSection[section])) return;

            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (EditorTools.Foldout("Smooth Movement", "If enabled, the cameras movement toward the target will be dampened.", 
                    ref CameraController.smoothMovement, true, true))
                {
                    
                    GUI.backgroundColor = Color.black;
                    EditorGUILayout.BeginVertical(_foldoutStyle);
                    {
                        GUI.backgroundColor = EditorTools.guiColorBackup;
                        
                        EditorGUI.indentLevel++;

                        EditorGUIUtility.labelWidth = 150;
                        CameraController.followMethod = (CameraController.FollowMethod) EditorGUILayout.EnumPopup(new GUIContent("Follow Method",
                    "The method used to follow the target."),
                            CameraController.followMethod);

                        GUILayout.Space(2.25f);
                        
                        if (CameraController.followMethod == CameraController.FollowMethod.Lerp)
                            CameraController.lerpMovementSpeed = EditorTools.Slider("Speed",
                                "The speed at which the camera will be linearly interpolated toward the camera.",
                                CameraController.lerpMovementSpeed, 0.5f, 50.0f, 150);

                        else
                            CameraController.smoothMovementTime = EditorTools.Slider("Damp Time",
                                "The amount time that the camera will lag behind the target.", 
                                CameraController.smoothMovementTime, 0.0f, 1.5f, 150);
                        
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }
                
                EditorTools.DrawLine(0.5f, 5, 2.5f);
                
                if (EditorTools.Foldout("Smooth Rotation", "Smoothly moves the characters rotation toward the target rotation.", 
                    ref CameraController.smoothRotation, true, true))
                {
                    
                    GUI.backgroundColor = Color.black;

                    EditorGUILayout.BeginVertical(_foldoutStyle);
                    {
                        GUI.backgroundColor = EditorTools.guiColorBackup;
                        
                        EditorGUI.indentLevel++;

                        EditorGUILayout.Space(1f);
                        
                        CameraController.smoothRotationFactor = EditorTools.Slider("Time",
                            "Applies a drag to the camera, making it take longer to reach the target rotation.",
                            CameraController.smoothRotationFactor, 0, 10.0f, 100);

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