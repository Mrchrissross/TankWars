using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TankWars.Controllers;
using TankWars.Utility;
using UnityEditor;
using UnityEngine;

namespace TankWars.Editor
{
    [CustomEditor(typeof(TankController))]
    public class TankControllerInspector : UnityEditor.Editor
    {
        private const float BoxMinWidth = 300f;
        private const float BoxMaxWidth = 1000f;
        
        private GUIStyle _boxStyle;
        private GUIStyle _foldoutStyle;

        private bool _customCamera;
        
        private TankController TankController => target as TankController;

        private void OnEnable()
        {
            EditorTools.InitTextures();
            _customCamera = false;
        }

        private void DrawActions()
        {
            TankController.accelerateInput = EditorTools.StringField("Accelerate",
                "Used to make the tank accelerate faster.",
                TankController.accelerateInput, 100);

            // Draw additional actions here.
        }
        
        public override void OnInspectorGUI()
        {
            EditorTools.InitStyles(out _boxStyle, out _foldoutStyle);
            
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(target, "MovementController");
            
            DrawSections();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUIUtility.labelWidth = 150;
            
            // base.OnInspectorGUI();
        }

        private void DrawSections()
        {
            DrawInputSettings(0);
            DrawSpeedSettings(1);
            DrawAccelerationSettings(2);
            DrawMaxSettings(3);
            DrawFrictionSettings(4);
            DrawRotorSettings(5);
        }

        private void DrawInputSettings(int section)
        {
            GUILayout.BeginHorizontal();
            {
                if (!TankController.hideSection[section])
                {
                    if (EditorTools.TexturedButton(EditorTools.eyeOpenTexture,
                        "Hides all the content in this section.", 20f))
                        TankController.hideSection[section] = true;
                    
                    GUILayout.Space(-20);
                }
                else
                {
                    if (EditorTools.TexturedButton(EditorTools.eyeClosedTexture,
                        "Shows all the content in this section.", 20f))
                        TankController.hideSection[section] = false;
                    
                    GUILayout.Space(-20);
                }
                
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 13
                };
            
                EditorGUILayout.LabelField(new GUIContent("Input", ""), style);

                if (!_customCamera)
                {
                    GUILayout.Space(-20);

                    if (EditorTools.TexturedButton(EditorTools.cameraTexture,
                        "Add a custom camera rather than the tank wars camera system.", 20f))
                        _customCamera = true;
                }
            }
            GUILayout.EndHorizontal();
            
            EditorTools.DrawLine();
            GUILayout.Space(5);

            if (TankController.hideSection[section]) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (_customCamera)
                {
                    EditorTools.Label("Custom Camera:", "Custom camera that will be used by the tank.", 100);
                    
                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUIUtility.labelWidth = 100;
                            TankController.camera = (Camera) EditorGUILayout.ObjectField(new GUIContent("",
                                ""), TankController.camera, typeof(Camera), true);

                            if (EditorTools.Button("Done", ""))
                                _customCamera = false;
                        }
                        GUILayout.EndHorizontal();

                    }
                    GUILayout.EndVertical();
                    
                    EditorTools.DrawLine(0.5f, 2.5f, 0);
                }
                
                EditorTools.Label("Keyboard:",
                    "Ensure these settings match those that are within the input manager.", 100);
                    
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorTools.Label("Invert:",
                            "Inverts the input on select axis.", 95);
                        
                        EditorTools.Toggle("Horizontal", "",
                            ref TankController.keyboardInvertHorizontal, 75);
                        
                        EditorTools.Toggle("Vertical", "",
                            ref TankController.keyboardInvertVertical, 60);
                        GUILayout.Space(-10);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2.5f);
                    
                    
                    TankController.keyboardHorizontalInput = EditorTools.StringField("Horizontal",
                        "This is the horizontal mouse input found in the input manager.",
                        TankController.keyboardHorizontalInput, 100);

                    TankController.keyboardVerticalInput = EditorTools.StringField("Vertical",
                        "This is the vertical mouse input found in the input manager.",
                        TankController.keyboardVerticalInput, 100);
                }
                GUILayout.EndVertical();

                EditorTools.DrawLine(0.5f, 2.5f, 0);
                
                EditorTools.Label("Gamepad:", "Ensure these settings match those that are within the input manager.",
                    60);
                    
                GUILayout.BeginVertical("box");
                {
                    EditorTools.Label("Left Joystick:", "Ensure these settings match those that are within the input manager.",
                        100);
                    
                    GUI.backgroundColor = Color.grey;
                    GUILayout.BeginVertical("box");
                    {
                        GUI.backgroundColor = EditorTools.guiColorBackup;
                        
                        GUI.backgroundColor = Color.grey;
                    
                        GUILayout.BeginHorizontal();
                        {
                            EditorTools.Label("Invert:",
                                "Inverts the input on select axis.", 95);

                            EditorTools.Toggle("Horizontal", "",
                                ref TankController.leftJoystickInvertHorizontal, 75);

                            EditorTools.Toggle("Vertical", "",
                                ref TankController.leftJoystickInvertVertical, 60);
                            GUILayout.Space(-10);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(2.5f);

                        TankController.leftJoystickHorizontalInput = EditorTools.StringField("Horizontal",
                            "This is the horizontal joystick input found in the input manager.",
                            TankController.leftJoystickHorizontalInput, 100);

                        TankController.leftJoystickVerticalInput = EditorTools.StringField("Vertical",
                            "This is the vertical joystick input found in the input manager.",
                            TankController.leftJoystickVerticalInput, 100);
                        
                        GUILayout.Space(4f);
                        
                        GUI.backgroundColor = EditorTools.guiColorBackup;

                        TankController.leftDeadZoneThreshold = EditorTools.Slider(" Dead Zone Threshold", 
                            "This is the dead zone threshold of the joystick, if the joystick input is below this " +
                            "threshold, no input is applied.", TankController.leftDeadZoneThreshold, 0.05f, 0.5f, 150);
                    }
                    GUILayout.EndVertical();
                    
                    EditorTools.DrawLine(0.5f, 2.5f, 2.5f);
                    
                    EditorTools.Label("Right Joystick:", "Ensure these settings match those that are within the input manager.",
                        100);
                    
                    GUI.backgroundColor = Color.grey;
                    GUILayout.BeginVertical("box");
                    {
                        GUI.backgroundColor = EditorTools.guiColorBackup;
                        
                        GUI.backgroundColor = Color.grey;
                        
                        GUILayout.BeginHorizontal();
                        {
                            EditorTools.Label("Invert:",
                                "Inverts the input on select axis.", 95);


                            EditorTools.Toggle("Horizontal", "",
                                ref TankController.rightJoystickInvertHorizontal, 75);

                            EditorTools.Toggle("Vertical", "",
                                ref TankController.rightJoystickInvertVertical, 60);
                            
                            
                            GUILayout.Space(-10);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(2.5f);

                        TankController.rightJoystickHorizontalInput = EditorTools.StringField("Horizontal",
                            "This is the horizontal joystick input found in the input manager.",
                            TankController.rightJoystickHorizontalInput, 100);

                        TankController.rightJoystickVerticalInput = EditorTools.StringField("Vertical",
                            "This is the vertical joystick input found in the input manager.",
                            TankController.rightJoystickVerticalInput, 100);
                        
                        GUILayout.Space(4f);
                        
                        GUI.backgroundColor = EditorTools.guiColorBackup;
                        
                        TankController.rightDeadZoneThreshold = EditorTools.Slider(" Dead Zone Threshold", 
                            "This is the dead zone threshold of the joystick, if the joystick input is below this " +
                            "threshold, no input is applied.", TankController.rightDeadZoneThreshold, 0.05f, 0.5f, 150);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                
                EditorTools.DrawLine(0.5f, 2.5f, 0);
                
                EditorTools.Label("Actions:",
                    "Ensure these settings match those that are within the input manager.", 100);
                    
                GUILayout.BeginVertical("box");
                {
                    DrawActions();
                }
                GUILayout.EndVertical();
                
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawSpeedSettings(int section)
        {
            if (EditorTools.DrawHeader("Speed", ref TankController.hideSection[section])) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                EditorGUILayout.Space(1f);
             
                EditorGUILayout.BeginHorizontal();
                { 
                    GUILayout.FlexibleSpace();
                    
                    EditorTools.ReadOnlyValue("Current Speed", "Current movement speed of the character (in m/s).", 
                        TankController.Speed, "0", 100, 30);
                    
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorTools.DrawLine(0.5f, 5f, 5);
                
                GUILayout.BeginVertical("box");
                {
                    TankController.ForwardSpeed = EditorTools.FloatField("Forward", 
                        "Speed when moving forward.", TankController.ForwardSpeed, 100);
                    
                    TankController.BackwardSpeed = EditorTools.FloatField("Backward", 
                        "Speed when moving backward.", TankController.BackwardSpeed, 100);
                    
                    TankController.TurnSpeed = EditorTools.FloatField("Turn", 
                        "Speed at which the tank turns.", TankController.TurnSpeed, 100);
                    
                    TankController.SpeedMultiplier = EditorTools.FloatField("Sprint Multiplier", 
                        "Speed multiplier while sprinting.", TankController.SpeedMultiplier, 100);
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }

        private void DrawAccelerationSettings(int section)
        {
            if (EditorTools.DrawHeader("Acceleration", ref TankController.hideSection[section])) return;

            const float width = 100.0f;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                var acceleration = TankController.Acceleration;
                acceleration = EditorTools.Slider("Acceleration",
                    "The rate at which the character accelerates in a given direction.",
                    acceleration, 0.5f, 150.0f, width);
                TankController.Acceleration = acceleration;
                
                var deceleration = TankController.Deceleration;
                deceleration = EditorTools.Slider("Deceleration",
                    "The rate at which the character decelerates and comes to a halt.",
                    deceleration, 0.5f, 150.0f, width);
                TankController.Deceleration = deceleration;
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
        
        private void DrawMaxSettings(int section)
        {
            if (EditorTools.DrawHeader("Max Speed", ref TankController.hideSection[section])) return;
        
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (EditorTools.Foldout("Limit Speed", "Enabling this will limit the characters maximum velocity including " +
                                            "those that are from external sources.", ref TankController.limitMaximumSpeed, true, true))
                {
                    GUILayout.BeginVertical("box");
                    {
                        TankController.MaxHorizontalSpeed = EditorTools.FloatField("Horizontal",
                        "The maximum speed at which the character can move along the X and Z axis.\n\n" +
                        "This includes all external physics that may be at work (eg. sliding, being pushed, etc).",
                        TankController.MaxHorizontalSpeed, 100);
            
                        TankController.MaxUpwardSpeed = EditorTools.FloatField("Upward",
                            "The maximum speed at which the character will go upward.\n\n" +
                            "This includes all external physics that may be at work (eg. elevator, etc).",
                            TankController.MaxUpwardSpeed, 100);
        
                        TankController.MaxDownwardSpeed = EditorTools.FloatField("Downward",
                            "The maximum speed at which the character will go downward.\n\n" +
                            "This includes all external physics that may be at work (eg. falling, etc).",
                            TankController.MaxDownwardSpeed, 100);
                    }
                    GUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
        
        private void DrawFrictionSettings(int section)
        {
            if (EditorTools.DrawHeader("Friction", ref TankController.hideSection[section])) return;

            const float width = 100.0f;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                var friction = TankController.Friction;
                
                friction.x = EditorTools.Slider("Turning", "This affects how quickly the tank can turn. " +
                "The higher the friction, the quicker the turn.", friction.x, 0, 10, width);
                friction.y = EditorTools.Slider("Braking", "This affects how the tank comes to a halt. " +
                "The more friction the character has, the quicker it will stop moving.", friction.y, 0, 10, width);
                
                TankController.Friction = friction;
                
                EditorGUILayout.Space(1f);
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
        
        private void DrawRotorSettings(int section)
        {
            if (EditorTools.DrawHeader("Rotor", ref TankController.hideSection[5])) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                EditorGUILayout.Space(1f);
             
                EditorGUILayout.BeginHorizontal();
                { 
                    GUILayout.FlexibleSpace();
                    
                    EditorTools.ReadOnlyValue("Current Rotation", "Current rotation of the tank (in degrees).", 
                        TankController.CannonRotor.rotation.eulerAngles.z, "0.0", 100, 40);
                    
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                { 
                    GUILayout.FlexibleSpace();

                    var cannonAngle = TankController.cannonAngle > 0 ? 360.0f - TankController.cannonAngle :
                        TankController.cannonAngle < 0 ? -TankController.cannonAngle : 0;
                    
                    EditorTools.ReadOnlyValue("Desired Rotation", "Desired rotation of the tank (in degrees).", 
                        cannonAngle, "0.0", 100, 40);
                    
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorTools.DrawLine(0.5f, 5f, 5);
                
                GUILayout.BeginVertical("box");
                {
                    EditorGUIUtility.labelWidth = 100;
                    TankController.rotorRotationMethod = (TankController.RotorRotationMethod) EditorGUILayout.EnumPopup(new GUIContent("Rotation Method",
                        "The method used to rotate the rotor."), TankController.rotorRotationMethod);

                    if (TankController.rotorRotationMethod != TankController.RotorRotationMethod.None)
                    {
                        if (TankController.rotorRotationMethod != TankController.RotorRotationMethod.SmoothDamp)
                        {
                            TankController.RotorSpeed = EditorTools.FloatField("Speed", 
                                "Speed at which the tanks rotor turns.", TankController.RotorSpeed, 100);
                        }
                        else
                        {
                            TankController.RotorSmoothSpeed = EditorTools.FloatField("Speed", 
                                "Speed at which the tanks rotor turns.", TankController.RotorSmoothSpeed, 100);
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
    }
}