using System;
using System.Collections.Generic;
using System.Linq;
using TankWars.Controllers;
using UnityEditor;
using UnityEngine;

namespace TankWars.Editor
{
    [CustomEditor(typeof(MovementController))]
    public class MovementControllerInspector : UnityEditor.Editor
    {
        
        private const float BoxMinWidth = 300f;
        private const float BoxMaxWidth = 1000f;
        
        private GUIStyle _boxStyle;
        private GUIStyle _foldoutStyle;
        
        private Color _guiColorBackup;
        private Texture2D _plusTexture;
        private Texture2D _minusTexture;
        private Texture2D _editTexture;
        
        private MovementController MovementController => target as MovementController;
        
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
                            ref MovementController.keyboardInvertHorizontal, 75);
                        
                        EditorTools.Toggle("Vertical", "",
                            ref MovementController.keyboardInvertVertical, 60);
                        GUILayout.Space(-10);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2.5f);
                    
                    
                    MovementController.keyboardHorizontalInput = EditorTools.StringField("Horizontal",
                        "This is the horizontal mouse input found in the input manager.",
                        MovementController.keyboardHorizontalInput, 100);

                    MovementController.keyboardVerticalInput = EditorTools.StringField("Vertical",
                        "This is the vertical mouse input found in the input manager.",
                        MovementController.keyboardVerticalInput, 100);
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
                            ref MovementController.joystickInvertHorizontal, 75);

                        EditorTools.Toggle("Vertical", "",
                            ref MovementController.joystickInvertVertical, 60);
                        GUILayout.Space(-10);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2.5f);


                    MovementController.joystickHorizontalInput = EditorTools.StringField("Horizontal",
                        "This is the horizontal joystick input found in the input manager.",
                        MovementController.joystickHorizontalInput, 100);

                    MovementController.joystickVerticalInput = EditorTools.StringField("Vertical",
                        "This is the vertical joystick input found in the input manager.",
                        MovementController.joystickVerticalInput, 100);
                    
                    EditorGUILayout.Space(7.5f);

                    MovementController.deadZoneThreshold = EditorTools.Slider(" Dead Zone Threshold", 
                        "This is the dead zone threshold of the joystick, if the joystick input is below this " +
                        "threshold, no input is applied.", MovementController.deadZoneThreshold, 0.05f, 0.5f, 150);
                    
                }
                GUILayout.EndVertical();
                
                EditorTools.DrawLine(0.5f, 7.5f, 0);
                
                EditorTools.Label("Actions:",
                    "Ensure these settings match those that are within the input manager.", 100);
                    
                GUILayout.BeginVertical("box");
                {
                    MovementController.accelerateInput = EditorTools.StringField("Accelerate",
                        "Used to make the tank accelerate faster.",
                        MovementController.accelerateInput, 100);

                    if (MovementController.firePoints.Count > 1)
                    {
                        MovementController.leftCannonFire = EditorTools.StringField("Left Cannon",
                            "Button used to fire the left cannon.",
                            MovementController.leftCannonFire, 100);
                        
                        MovementController.rightCannonFire = EditorTools.StringField("Right Cannon",
                            "Button used to fire the right cannon.",
                            MovementController.rightCannonFire, 100);
                    }
                    else
                    {
                        MovementController.mainCannonFire = EditorTools.StringField("Main Cannon",
                            "Button used to fire the main cannon.",
                            MovementController.mainCannonFire, 100);
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
                        MovementController.Speed, default, 100, 22.5f);
                    
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorTools.DrawLine(0.5f, 5f, 5);
                
                GUILayout.BeginVertical("box");
                {
                    MovementController.ForwardSpeed = EditorTools.FloatField("Forward", 
                        "Speed when moving forward.", MovementController.ForwardSpeed, 100);
                    
                    MovementController.BackwardSpeed = EditorTools.FloatField("Backward", 
                        "Speed when moving backward.", MovementController.BackwardSpeed, 100);
                    
                    MovementController.TurnSpeed = EditorTools.FloatField("Turn", 
                        "Speed at which the tank turns.", MovementController.TurnSpeed, 100);
                    
                    MovementController.SpeedMultiplier = EditorTools.FloatField("Sprint Multiplier", 
                        "Speed multiplier while sprinting.", MovementController.SpeedMultiplier, 100);
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
                var acceleration = MovementController.Acceleration;
                MovementController.Acceleration = EditorTools.Slider("Acceleration",
                    "The rate at which the character accelerates in a given direction.",
                    acceleration, 0.5f, 150.0f, width);
                MovementController.Acceleration = acceleration;
                
                var deceleration = MovementController.Deceleration;
                deceleration = EditorTools.Slider("Deceleration",
                    "The rate at which the character decelerates and comes to a halt.",
                    deceleration, 0.5f, 150.0f, width);
                MovementController.Deceleration = deceleration;
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
                                            "those that are from external sources.", ref MovementController.limitMaximumSpeed, true, true))
                {
                    GUILayout.BeginVertical("box");
                    {
                        MovementController.MaxHorizontalSpeed = EditorTools.FloatField("Horizontal",
                        "The maximum speed at which the character can move along the X and Z axis.\n\n" +
                        "This includes all external physics that may be at work (eg. sliding, being pushed, etc).",
                        MovementController.MaxHorizontalSpeed, 100);
            
                        MovementController.MaxUpwardSpeed = EditorTools.FloatField("Upward",
                            "The maximum speed at which the character will go upward.\n\n" +
                            "This includes all external physics that may be at work (eg. elevator, etc).",
                            MovementController.MaxUpwardSpeed, 100);
        
                        MovementController.MaxDownwardSpeed = EditorTools.FloatField("Downward",
                            "The maximum speed at which the character will go downward.\n\n" +
                            "This includes all external physics that may be at work (eg. falling, etc).",
                            MovementController.MaxDownwardSpeed, 100);
                    }
                    GUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
    }
}