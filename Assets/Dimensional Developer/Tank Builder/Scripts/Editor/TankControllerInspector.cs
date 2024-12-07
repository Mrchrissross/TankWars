using UnityEditor;
using UnityEngine;
using DimensionalDeveloper.TankBuilder.Controllers;

namespace DimensionalDeveloper.TankBuilder.Editor
{
    [CustomEditor(typeof(TankController))]
    public class TankControllerInspector : EditorTemplate
    {
        protected override string ScriptName => "Tank Controller";
        protected override bool EnableBaseGUI => false;
        
        private bool _customCamera;
        
        private TankController TankController => target as TankController;

        protected override void OnEnable()
        {
            base.OnEnable();
            _customCamera = false;
        }

        private void DrawActions()
        {
            TankController.accelerateInput = StringField("Accelerate",
                "Used to make the tank accelerate faster.",
                TankController.accelerateInput, 100);
        }

        protected override void DrawSections()
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
            if (DrawHeader(this, section, "Input", "", () =>
                {
                    if (_customCamera) 
                        return;
                    
                    GUILayout.Space(-20);

                    if (TexturedButton(cameraTexture, "Add a custom camera rather than the tank wars camera system."))
                        _customCamera = true;
                })) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (_customCamera)
                {
                    Label("Custom Camera:", "Custom camera that will be used by the tank.", 100);
                    
                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUIUtility.labelWidth = 100;
                            TankController.camera = (Camera) EditorGUILayout.ObjectField(new GUIContent("",
                                ""), TankController.camera, typeof(Camera), true);

                            if (Button("Done", ""))
                                _customCamera = false;
                        }
                        GUILayout.EndHorizontal();

                    }
                    GUILayout.EndVertical();
                    
                    DrawLine(0.5f, 2.5f, 0);
                }
                
                Label("Keyboard:",
                    "Ensure these settings match those that are within the input manager.", 100);
                    
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        Label("Invert:",
                            "Inverts the input on select axis.", 95);
                        
                        Toggle("Horizontal", "",
                            ref TankController.keyboardInvertHorizontal, 75);
                        
                        Toggle("Vertical", "",
                            ref TankController.keyboardInvertVertical, 60);
                        GUILayout.Space(-10);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2.5f);
                    
                    
                    TankController.keyboardHorizontalInput = StringField("Horizontal",
                        "This is the horizontal mouse input found in the input manager.",
                        TankController.keyboardHorizontalInput, 100);

                    TankController.keyboardVerticalInput = StringField("Vertical",
                        "This is the vertical mouse input found in the input manager.",
                        TankController.keyboardVerticalInput, 100);
                }
                GUILayout.EndVertical();

                DrawLine(0.5f, 2.5f, 0);
                
                Label("Gamepad:", "Ensure these settings match those that are within the input manager.",
                    60);
                    
                GUILayout.BeginVertical("box");
                {
                    Label("Left Joystick:", "Ensure these settings match those that are within the input manager.",
                        100);
                    
                    GUI.backgroundColor = Color.grey;
                    GUILayout.BeginVertical("box");
                    {
                        GUI.backgroundColor = guiColorBackup;
                        
                        GUI.backgroundColor = Color.grey;
                    
                        GUILayout.BeginHorizontal();
                        {
                            Label("Invert:",
                                "Inverts the input on select axis.", 95);

                            Toggle("Horizontal", "",
                                ref TankController.leftJoystickInvertHorizontal, 75);

                            Toggle("Vertical", "",
                                ref TankController.leftJoystickInvertVertical, 60);
                            GUILayout.Space(-10);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(2.5f);

                        TankController.leftJoystickHorizontalInput = StringField("Horizontal",
                            "This is the horizontal joystick input found in the input manager.",
                            TankController.leftJoystickHorizontalInput, 100);

                        TankController.leftJoystickVerticalInput = StringField("Vertical",
                            "This is the vertical joystick input found in the input manager.",
                            TankController.leftJoystickVerticalInput, 100);
                        
                        GUILayout.Space(4f);
                        
                        GUI.backgroundColor = guiColorBackup;

                        TankController.leftDeadZoneThreshold = Slider(" Dead Zone Threshold", 
                            "This is the dead zone threshold of the joystick, if the joystick input is below this " +
                            "threshold, no input is applied.", TankController.leftDeadZoneThreshold, 0.05f, 0.5f, 150);
                    }
                    GUILayout.EndVertical();
                    
                    DrawLine(0.5f, 2.5f, 2.5f);
                    
                    Label("Right Joystick:", "Ensure these settings match those that are within the input manager.",
                        100);
                    
                    GUI.backgroundColor = Color.grey;
                    GUILayout.BeginVertical("box");
                    {
                        GUI.backgroundColor = guiColorBackup;
                        
                        GUI.backgroundColor = Color.grey;
                        
                        GUILayout.BeginHorizontal();
                        {
                            Label("Invert:",
                                "Inverts the input on select axis.", 95);


                            Toggle("Horizontal", "",
                                ref TankController.rightJoystickInvertHorizontal, 75);

                            Toggle("Vertical", "",
                                ref TankController.rightJoystickInvertVertical, 60);
                            
                            
                            GUILayout.Space(-10);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(2.5f);

                        TankController.rightJoystickHorizontalInput = StringField("Horizontal",
                            "This is the horizontal joystick input found in the input manager.",
                            TankController.rightJoystickHorizontalInput, 100);

                        TankController.rightJoystickVerticalInput = StringField("Vertical",
                            "This is the vertical joystick input found in the input manager.",
                            TankController.rightJoystickVerticalInput, 100);
                        
                        GUILayout.Space(4f);
                        
                        GUI.backgroundColor = guiColorBackup;
                        
                        TankController.rightDeadZoneThreshold = Slider(" Dead Zone Threshold", 
                            "This is the dead zone threshold of the joystick, if the joystick input is below this " +
                            "threshold, no input is applied.", TankController.rightDeadZoneThreshold, 0.05f, 0.5f, 150);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                
                DrawLine(0.5f, 2.5f, 0);
                
                Label("Actions:",
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
            if (DrawHeader(this, section, "Speed")) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                EditorGUILayout.Space(1f);
             
                EditorGUILayout.BeginHorizontal();
                { 
                    GUILayout.FlexibleSpace();
                    
                    ReadOnlyValue("Current Speed", "Current movement speed of the character (in m/s).", 
                        TankController.Speed, "0", 100, 30);
                    
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                
                DrawLine(0.5f, 5f, 5);
                
                GUILayout.BeginVertical("box");
                {
                    TankController.ForwardSpeed = FloatField("Forward", 
                        "Speed when moving forward.", TankController.ForwardSpeed, 100);
                    
                    TankController.BackwardSpeed = FloatField("Backward", 
                        "Speed when moving backward.", TankController.BackwardSpeed, 100);
                    
                    TankController.TurnSpeed = FloatField("Turn", 
                        "Speed at which the tank turns.", TankController.TurnSpeed, 100);
                    
                    TankController.SpeedMultiplier = FloatField("Sprint Multiplier", 
                        "Speed multiplier while sprinting.", TankController.SpeedMultiplier, 100);
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }

        private void DrawAccelerationSettings(int section)
        {
            if (DrawHeader(this, section, "Acceleration")) return;

            const float width = 100.0f;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                var acceleration = TankController.Acceleration;
                acceleration = Slider("Acceleration",
                    "The rate at which the character accelerates in a given direction.",
                    acceleration, 0.5f, 150.0f, width);
                TankController.Acceleration = acceleration;
                
                var deceleration = TankController.Deceleration;
                deceleration = Slider("Deceleration",
                    "The rate at which the character decelerates and comes to a halt.",
                    deceleration, 0.5f, 150.0f, width);
                TankController.Deceleration = deceleration;
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
        
        private void DrawMaxSettings(int section)
        {
            if (DrawHeader(this, section, "Max Speed")) return;
        
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (Foldout("Limit Speed", "Enabling this will limit the characters maximum velocity including " +
                                            "those that are from external sources.", ref TankController.limitMaximumSpeed, true, true))
                {
                    GUILayout.BeginVertical("box");
                    {
                        TankController.MaxHorizontalSpeed = FloatField("Horizontal",
                        "The maximum speed at which the character can move along the X and Z axis.\n\n" +
                        "This includes all external physics that may be at work (eg. sliding, being pushed, etc).",
                        TankController.MaxHorizontalSpeed, 100);
            
                        TankController.MaxUpwardSpeed = FloatField("Upward",
                            "The maximum speed at which the character will go upward.\n\n" +
                            "This includes all external physics that may be at work (eg. elevator, etc).",
                            TankController.MaxUpwardSpeed, 100);
        
                        TankController.MaxDownwardSpeed = FloatField("Downward",
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
            if (DrawHeader(this, section, "Friction")) return;

            const float width = 100.0f;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                var friction = TankController.Friction;
                
                friction.x = Slider("Turning", "This affects how quickly the tank can turn. " +
                "The higher the friction, the quicker the turn.", friction.x, 0, 10, width);
                friction.y = Slider("Braking", "This affects how the tank comes to a halt. " +
                "The more friction the character has, the quicker it will stop moving.", friction.y, 0, 10, width);
                
                TankController.Friction = friction;
                
                EditorGUILayout.Space(1f);
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
        
        private void DrawRotorSettings(int section)
        {
            if (DrawHeader(this, section, "Rotor")) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                EditorGUILayout.Space(1f);
             
                EditorGUILayout.BeginHorizontal();
                { 
                    GUILayout.FlexibleSpace();
                    
                    ReadOnlyValue("Current Rotation", "Current rotation of the tank (in degrees).", 
                        TankController.CannonRotor.rotation.eulerAngles.z, "0.0", 100, 40);
                    
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                { 
                    GUILayout.FlexibleSpace();

                    var cannonAngle = TankController.cannonAngle > 0 ? 360.0f - TankController.cannonAngle :
                        TankController.cannonAngle < 0 ? -TankController.cannonAngle : 0;
                    
                    ReadOnlyValue("Desired Rotation", "Desired rotation of the tank (in degrees).", 
                        cannonAngle, "0.0", 100, 40);
                    
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                
                DrawLine(0.5f, 5f, 5);
                
                GUILayout.BeginVertical("box");
                {
                    EditorGUIUtility.labelWidth = 100;
                    TankController.rotorRotationMethod = (TankController.RotorRotationMethod) EditorGUILayout.EnumPopup(new GUIContent("Rotation Method",
                        "The method used to rotate the rotor."), TankController.rotorRotationMethod);

                    if (TankController.rotorRotationMethod != TankController.RotorRotationMethod.None)
                    {
                        if (TankController.rotorRotationMethod != TankController.RotorRotationMethod.SmoothDamp)
                        {
                            TankController.RotorSpeed = FloatField("Speed", 
                                "Speed at which the tanks rotor turns.", TankController.RotorSpeed, 100);
                        }
                        else
                        {
                            TankController.RotorSmoothSpeed = FloatField("Speed", 
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