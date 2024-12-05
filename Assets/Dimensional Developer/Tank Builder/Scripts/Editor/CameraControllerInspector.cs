using UnityEditor;
using UnityEngine;
using DimensionalDeveloper.TankBuilder.Controllers;

namespace DimensionalDeveloper.TankBuilder.Editor
{
    [CustomEditor(typeof(CameraController))]
    public class CameraControllerInspector : EditorTemplate
    {
        protected override string ScriptName => "Camera Controller";
        protected override bool EnableBaseGUI => false;
        
        private bool _customTankController;
        
        private CameraController CameraController => target as CameraController;
        
        protected override void DrawSections()
        {
            DrawTargetSettings(0);
            DrawInputSettings(1);
            DrawMovementSettings(2);
        }
        
        private void DrawTargetSettings(int section)
        {
            if (DrawHeader(this, section, "Rotor", "", () =>
                {
                    if (_customTankController) 
                        return;
                    
                    GUILayout.Space(-20);

                    if (TexturedButton(tankTexture,
                            "Manually input the Tank Controller into the camera system.", 20f))
                        _customTankController = true;
                })) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (_customTankController)
                {
                    Label("Tank Controller:", "The tank controller to be used in this camera system.", 100);
                    
                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUIUtility.labelWidth = 100;
                            CameraController.tankController = (TankController) EditorGUILayout.ObjectField(new GUIContent("",
                                ""), CameraController.tankController, typeof(TankController), true);

                            if (Button("Done", ""))
                                _customTankController = false;
                        }
                        GUILayout.EndHorizontal();

                    }
                    GUILayout.EndVertical();
                    
                    DrawLine(0.5f, 2.5f, 0);
                }
                
                GUILayout.BeginHorizontal();
                {
                    ObjectField("Target:",
                        "This is the current target that the camera will follow.", 
                        ref CameraController.cameraTarget, 60, 0, false);
                    
                    if(Button("Reset", "Resets the target to look at the tank."))
                        CameraController.ResetTarget();
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginVertical("box");
                {
                    Toggle("Rotate With Target?",
                        "If enabled, the camera will rotate with the target on the z axis.",
                        ref CameraController.rotateWithTarget, 120);
                    
                    GUILayout.Space(2.5f);
                    
                    GUILayout.BeginHorizontal();
                    {
                        Label("Offset:", "Offsets the cameras position relative to the target.", 50, 0);

                        var positionOffset = CameraController.PositionOffset;
                        positionOffset.x = FloatField("X", "", positionOffset.x, 20f);
                        positionOffset.y = FloatField("Y", "", positionOffset.y, 20f);
                        CameraController.PositionOffset = positionOffset;
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(5.0f);
                    
                    CameraController.CameraDistance = Slider("Camera Distance",
                        "This is the maximum distance that the camera will extend from the target.",
                        CameraController.CameraDistance, 15.0f, 40.0f, 120);
                    
                    DrawLine(0.5f, 2.5f, 2.5f);
                    // GUILayout.BeginHorizontal();
                    // {
                        Toggle("Far Look?",
                            "When pointing at a certain direction, the camera will offset to look further off in the distance.",
                            ref CameraController.farLook, 120);
                    // }
                    // GUILayout.EndHorizontal();
                    
                    CameraController.farLookSpeed = Slider("Far Look Speed",
                        "This is the speed that the camera will offset from the target while using far look.",
                        CameraController.farLookSpeed, 0.0f, 20.0f, 120);
                    
                    CameraController.farLookDistance = Slider("Far Look Distance",
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
            if (DrawHeader(this, section, "Input"))
                return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    var cursorLabel = CameraController.lockCursor ? "<b>Cursor Locked</b>" : "Cursor Not Locked";
                    Label(cursorLabel, "Displays whether the mouse cursor is locked to the screen or not.",
                        120, 0, new GUIStyle(GUI.skin.label)
                        {
                            alignment = TextAnchor.MiddleCenter,
                            richText = true
                        });
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();

                DrawLine(0.5f, 2.5f, 2.5f);

                CameraController.UnlockCursorKey = KeyCodeDropdown("Cursor Unlock Key",
                    "The keyboard key to unlock the mouse cursor.", CameraController.UnlockCursorKey, 110);

                DrawLine(0.5f, 2.5f, 0);

                Label("Joystick:", "Ensure these settings match those that are within the input manager.",
                    60);

                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        Label("Invert:",
                            "Inverts the input on select axis.", 95);

                        Toggle("Horizontal", "",
                            ref CameraController.joystickInvertHorizontal, 75);

                        Toggle("Vertical", "",
                            ref CameraController.joystickInvertVertical, 60);
                        GUILayout.Space(-10);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2.5f);


                    CameraController.joystickHorizontalInput = StringField("Horizontal",
                        "This is the horizontal joystick input found in the input manager.",
                        CameraController.joystickHorizontalInput, 100);

                    CameraController.joystickVerticalInput = StringField("Vertical",
                        "This is the vertical joystick input found in the input manager.",
                        CameraController.joystickVerticalInput, 100);

                    EditorGUILayout.Space(7.5f);

                    CameraController.deadZoneThreshold = Slider(" Dead Zone Threshold",
                        "This is the dead zone threshold of the joystick, if the joystick input is below this " +
                        "threshold, no input is applied.", CameraController.deadZoneThreshold, 0.05f, 0.5f, 150);

                }
                GUILayout.EndVertical();
                
                DrawLine(0.5f, 2.5f, 0);
                
                Label("Actions:",
                    "Ensure these settings match those that are within the input manager.", 100);
                    
                GUILayout.BeginVertical("box");
                {
                    CameraController.farLookInput = StringField("Far Look",
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
            if (DrawHeader(this, section, "Movement")) 
                return;

            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (Foldout("Smooth Movement", "If enabled, the cameras movement toward the target will be dampened.", 
                    ref CameraController.smoothMovement, true, true))
                {
                    
                    GUI.backgroundColor = Color.black;
                    EditorGUILayout.BeginVertical(_foldoutStyle);
                    {
                        GUI.backgroundColor = guiColorBackup;
                        
                        EditorGUI.indentLevel++;

                        EditorGUIUtility.labelWidth = 150;
                        CameraController.followMethod = (CameraController.FollowMethod) EditorGUILayout.EnumPopup(new GUIContent("Follow Method",
                    "The method used to follow the target."),
                            CameraController.followMethod);

                        GUILayout.Space(2.25f);
                        
                        if (CameraController.followMethod == CameraController.FollowMethod.Lerp)
                            CameraController.lerpMovementSpeed = Slider("Speed",
                                "The speed at which the camera will be linearly interpolated toward the camera.",
                                CameraController.lerpMovementSpeed, 0.5f, 50.0f, 150);

                        else
                            CameraController.smoothMovementTime = Slider("Damp Time",
                                "The amount time that the camera will lag behind the target.", 
                                CameraController.smoothMovementTime, 0.0f, 1.5f, 150);
                        
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }
                
                DrawLine(0.5f, 5, 2.5f);
                
                if (Foldout("Smooth Rotation", "Smoothly moves the characters rotation toward the target rotation.", 
                    ref CameraController.smoothRotation, true, true))
                {
                    
                    GUI.backgroundColor = Color.black;

                    EditorGUILayout.BeginVertical(_foldoutStyle);
                    {
                        GUI.backgroundColor = guiColorBackup;
                        
                        EditorGUI.indentLevel++;

                        EditorGUILayout.Space(1f);
                        
                        CameraController.smoothRotationFactor = Slider("Time",
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