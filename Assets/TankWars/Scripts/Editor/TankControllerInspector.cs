using System;
using System.Collections.Generic;
using System.Linq;
using TankWars.Controllers;
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
        
        private Color _guiColorBackup;
        private Texture2D _plusTexture;
        private Texture2D _minusTexture;
        private Texture2D _editTexture;
        
        private TankController TankController => target as TankController;
        
        private void OnEnable()
        {
            // Save the original background color.
            _guiColorBackup = GUI.backgroundColor;
            
            // Setup the textures
            const string path = "TankWars/EditorUI/";
            _plusTexture = EditorTools.InitTexture(path + "plus");
            _minusTexture = EditorTools.InitTexture(path + "minus");
            _editTexture = EditorTools.InitTexture(path + "edit");
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
            DrawInputSettings();
            DrawSpeedSettings();
            DrawAccelerationSettings();
            DrawMaxSettings();
            DrawFrictionSettings();
        }

        private void DrawInputSettings()
        {
            EditorTools.Header("Input");

            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
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

                EditorTools.DrawLine(0.5f, 7.5f, 0);
                
                EditorTools.Label("Joystick:", "Ensure these settings match those that are within the input manager.",
                    60);
                    
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorTools.Label("Invert:",
                            "Inverts the input on select axis.", 95);

                        EditorTools.Toggle("Horizontal", "",
                            ref TankController.joystickInvertHorizontal, 75);

                        EditorTools.Toggle("Vertical", "",
                            ref TankController.joystickInvertVertical, 60);
                        GUILayout.Space(-10);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2.5f);


                    TankController.joystickHorizontalInput = EditorTools.StringField("Horizontal",
                        "This is the horizontal joystick input found in the input manager.",
                        TankController.joystickHorizontalInput, 100);

                    TankController.joystickVerticalInput = EditorTools.StringField("Vertical",
                        "This is the vertical joystick input found in the input manager.",
                        TankController.joystickVerticalInput, 100);
                    
                    EditorGUILayout.Space(7.5f);

                    TankController.deadZoneThreshold = EditorTools.Slider(" Dead Zone Threshold", 
                        "This is the dead zone threshold of the joystick, if the joystick input is below this " +
                        "threshold, no input is applied.", TankController.deadZoneThreshold, 0.05f, 0.5f, 150);
                    
                }
                GUILayout.EndVertical();
                
                EditorTools.DrawLine(0.5f, 7.5f, 0);
                
                EditorTools.Label("Actions:",
                    "Ensure these settings match those that are within the input manager.", 100);
                    
                GUILayout.BeginVertical("box");
                {
                    TankController.accelerateInput = EditorTools.StringField("Accelerate",
                        "Used to make the tank accelerate faster.",
                        TankController.accelerateInput, 100);

                    if (TankController.firePoints.Count > 1)
                    {
                        TankController.leftCannonFire = EditorTools.StringField("Left Cannon",
                            "Button used to fire the left cannon.",
                            TankController.leftCannonFire, 100);
                        
                        TankController.rightCannonFire = EditorTools.StringField("Right Cannon",
                            "Button used to fire the right cannon.",
                            TankController.rightCannonFire, 100);
                    }
                    else
                    {
                        TankController.mainCannonFire = EditorTools.StringField("Main Cannon",
                            "Button used to fire the main cannon.",
                            TankController.mainCannonFire, 100);
                    }
                }
                GUILayout.EndVertical();
                
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawSpeedSettings()
        {
            EditorTools.Header("Speed");

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

        private void DrawAccelerationSettings()
        {
            EditorTools.Header("Acceleration");

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
        
        private void DrawMaxSettings()
        {
            EditorTools.Header("Max Speed");
        
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
        
        private void DrawFrictionSettings()
        {
            EditorTools.Header("Friction", "This is how much grip the tank has to its surface.");

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
        
    }
}