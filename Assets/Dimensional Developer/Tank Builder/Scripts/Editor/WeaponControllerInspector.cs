﻿using UnityEditor;
using UnityEngine;
using DimensionalDeveloper.TankBuilder.Controllers;
using DimensionalDeveloper.TankBuilder.Managers;
using DimensionalDeveloper.TankBuilder.Utility;

namespace DimensionalDeveloper.TankBuilder.Editor
{
    [CustomEditor(typeof(WeaponController))]
    public class WeaponControllerInspector : EditorTemplate
    {
        protected override string ScriptName => "Weapon Controller";
        protected override bool EnableBaseGUI => false;
        
        public string[] assets;
        public string[] sounds;
        
        private WeaponController WeaponController => target as WeaponController;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            InitAssets();
            InitSounds();
        }

        private void InitAssets()
        {
            if (AssetManager.Instance == null) return;

            ref var amAssets = ref AssetManager.Instance.assets;
            var length = amAssets.Count;
            
            assets = new string[length + 1];
            assets[0] = "None";
            for (var i = 0; i < length; i++) assets[i + 1] = amAssets[i].name;
        }
        
        private void InitSounds()
        {
            if (AudioManager.Instance == null) return;

            ref var amSounds = ref AudioManager.Instance.sounds;
            var length = amSounds.Count;
            
            sounds = new string[length + 1];
            sounds[0] = "None";
            for (var i = 0; i < length; i++) sounds[i + 1] = amSounds[i].name;
        }

        protected override void DrawSections()
        {
            DrawInputSettings(0);
            DrawWeaponSettings(1);
        }
        
        private void DrawInputSettings(int section)
        {
            if (DrawHeader(this , section, "Input")) 
                return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                Label("Shoot:",
                    "Ensure these settings match those that are within the input manager.", 100);
                    
                GUILayout.BeginVertical("box");
                {
                    if (WeaponController.firePoints.Count > 1)
                    {
                        WeaponController.cannonInput[1] = StringField("Left Cannon",
                            "Button used to fire the left cannon.",
                            WeaponController.cannonInput[1], 100);
                        
                        WeaponController.cannonInput[2] = StringField("Right Cannon",
                            "Button used to fire the right cannon.",
                            WeaponController.cannonInput[2], 100);
                    }
                    else
                    {
                        WeaponController.cannonInput[0] = StringField("Main Cannon",
                            "Button used to fire the main cannon.",
                            WeaponController.cannonInput[0], 100);
                    }
                }
                GUILayout.EndVertical();
                
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        private void DrawWeaponSettings(int section)
        {
            if (DrawHeader(this , section, "Weapons")) return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                if (WeaponController.firePoints.Count > 1)
                {
                    DrawWeapon("Left Cannon", 1);
                    
                    DrawLine(0.5f, 0f, 2.5f);
                    
                    DrawWeapon("Right Cannon", 2);
                }
                else DrawWeapon("Main Cannon", 0);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawWeapon(string weaponName, int weaponNumber)
        {
            if (!Foldout(weaponName + ":", "", ref WeaponController.weaponDropdown[weaponNumber], false,
                true)) return;
            
            GUILayout.Space(-2.5f);
                
            GUILayout.BeginVertical("box");
            {
                ref var weapon = ref WeaponController.weapons[weaponNumber];

                EditorGUI.BeginChangeCheck();
                if (weapon != null) Undo.RecordObject(weapon, weapon.Name);

                weapon = (Weapon) EditorGUILayout.ObjectField("",
                    weapon, typeof(Weapon), true);

                if (weapon == null)
                {
                    GUILayout.EndVertical();
                    return;
                }

                DrawLine(0.5f, 5f, 2.5f);

                weapon.Name = StringField("Name", "The name of the weapon.", weapon.Name, 100);
                
                DrawLine(0.5f, 3f, 2.5f);
                
                EditorGUIUtility.labelWidth = 100;
                weapon.assetIndex = EditorGUILayout.Popup(new GUIContent("Asset",
                    "The asset to spawn from the asset manager."), weapon.assetIndex, assets);
                weapon.Asset = assets[weapon.assetIndex];

                GUILayout.Space(1.5f);
                
                weapon.CollisionLayer = LayerMaskField("Collision",
                    "Layers that the ammo can collide with.", weapon.CollisionLayer, 100);

                DrawLine(0.5f, 3f, 2.5f);

                Label("Weapon Settings:", "", 100);

                GUI.backgroundColor = Color.grey;
                GUILayout.BeginVertical("box");
                {
                    GUI.backgroundColor = guiColorBackup;

                    var shotTimer = weapon.ShotTimer;

                    GUILayout.BeginHorizontal();
                    {
                        shotTimer.x = FloatField("Shot Timer",
                            "Time between shots fired (in seconds). " +
                            "The X value is the set time that the timer will reset to. " +
                            "The Y value is the current count of the timer (the one being reduced).",
                            shotTimer.x, 100);

                        GUILayout.Space(-25);
                        Label("Count:   " + shotTimer.y.ToString("0.0"), "", 90);
                        GUILayout.Space(-40);
                    }
                    GUILayout.EndHorizontal();

                    weapon.ShotTimer = shotTimer;

                    GUILayout.Space(3.25f);

                    weapon.Speed = FloatField("Speed", "How fast the bullet travels.",
                        weapon.Speed, 100);

                    weapon.Damage = FloatField("Damage", "How much damage the bullet does.",
                        weapon.Damage, 100);
                }
                GUILayout.EndVertical();

                DrawLine(0.5f, 2.5f, 2.5f);

                Label("Sounds:", "", 100);

                GUI.backgroundColor = Color.grey;
                GUILayout.BeginVertical("box");
                {
                    GUI.backgroundColor = guiColorBackup;

                    EditorGUIUtility.labelWidth = 100;
                    weapon.shotSoundIndex = EditorGUILayout.Popup(new GUIContent("Shoot",
                        "The sound that this weapon makes when the tanks shoots."), weapon.shotSoundIndex, sounds);
                    weapon.ShotSound = sounds[weapon.shotSoundIndex];

                    GUILayout.Space(1.5f);

                    EditorGUIUtility.labelWidth = 100;
                    weapon.explosionSoundIndex = EditorGUILayout.Popup(new GUIContent("Explosion",
                        "Sound effect when the ammo explodes."), weapon.explosionSoundIndex, sounds);
                    weapon.ExplosionSound = sounds[weapon.explosionSoundIndex];

                    GUILayout.Space(1.5f);
                }
                GUILayout.EndVertical();

                DrawLine(0.5f, 2.5f, 2.5f);

                Label("Particle Systems:", "", 100);

                GUI.backgroundColor = Color.grey;
                GUILayout.BeginVertical("box");
                {
                    GUI.backgroundColor = guiColorBackup;

                    EditorGUIUtility.labelWidth = 100;
                    weapon.muzzleFlashIndex = EditorGUILayout.Popup(new GUIContent("Muzzle Flash",
                        "The particle system to play when the tanks shoots."), weapon.muzzleFlashIndex, assets);
                    weapon.MuzzleFlash = assets[weapon.muzzleFlashIndex];

                    GUILayout.Space(1.5f);
                    
                    EditorGUIUtility.labelWidth = 100;
                    weapon.explosionIndex = EditorGUILayout.Popup(new GUIContent("Explosion",
                        "The particle system to play when the bullet explodes."), weapon.explosionIndex, assets);
                    
                    if(weapon.explosionIndex < assets.Length) weapon.Explosion = assets[weapon.explosionIndex];

                    GUILayout.Space(1.5f);
                }
                GUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(weapon);
            }
            GUILayout.EndVertical();
        }
    }
}