using TankWars.Managers;
using UnityEditor;
using UnityEngine;

namespace TankWars.Editor
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerInspector : UnityEditor.Editor
    {
        private const float BoxMinWidth = 300f;
        private const float BoxMaxWidth = 1000f;
        
        private GUIStyle _boxStyle;
        private GUIStyle _foldoutStyle;

        private AudioManager AudioManager => target as AudioManager;

        private void OnEnable() => EditorTools.InitTextures();
        
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
        
        private void DrawSections() => DrawSounds(0);

        private void DrawSounds(int section)
        {
            GUILayout.BeginHorizontal();
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 13
                };
            
                EditorGUILayout.LabelField(new GUIContent("Audio", ""), style);

                GUILayout.Space(-20);

                if (EditorTools.TexturedButton(EditorTools.plusTexture,
                    "Adds a new sound.", 20f))
                    AudioManager.AddSound();
            }
            GUILayout.EndHorizontal();
            
            EditorTools.DrawLine(0.5f, 0, 2.5f);
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                var index = 0;
                foreach (var sound in AudioManager.sounds)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (!EditorTools.Foldout(sound.name, "", ref sound.hideSection))
                        {
                            GUILayout.EndHorizontal();
                            
                            if(index < AudioManager.sounds.Count - 1) 
                                EditorTools.DrawLine(0.5f, 1.5f, 5f);
                            
                            index++;
                            continue;
                        }
                
                        GUILayout.Space(-100);

                        if (EditorTools.TexturedButton(EditorTools.plusTexture, "Creates a copy of this sound.", 20f))
                            AudioManager.CopySound(sound);

                        if (EditorTools.TexturedButton(EditorTools.minusTexture, "Removes this sound.", 20f))
                        {
                            AudioManager.sounds.RemoveAt(index);
                            GUILayout.EndHorizontal();
                            break;
                        }
                    }
                    GUILayout.EndHorizontal();
            
                    if(index < AudioManager.sounds.Count - 1) 
                        EditorTools.DrawLine(0.5f, 1.5f, 5f);
                    
                    DrawSound(index, sound);

                    index++;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawSound(int index, Sound sound)
        {
            GUILayout.BeginVertical("box");
            {
                sound.name = EditorTools.StringField("Name", "Name of the sound. This will be used within the code.",
                    sound.name, 100);

                EditorGUIUtility.labelWidth = 100;
                sound.clip = (AudioClip) EditorGUILayout.ObjectField(new GUIContent("Clip", "The audio clip to be played."),
                    sound.clip, typeof(AudioClip), true);
                
                EditorTools.DrawLine(0.5f, 0, 2.5f);

                const float floatWidth = 32.5f;
                
                GUILayout.BeginHorizontal();
                {
                    EditorTools.MinMaxSlider("Volume", "Volume of the sound." +
                        "\n To add randomness, increase the range between the two handles.",
                        ref sound.audioVolume.x, ref sound.audioVolume.y, 0.0f, 1.0f,
                        60);

                    sound.audioVolume.x = EditorGUILayout.FloatField("", sound.audioVolume.x, GUILayout.MaxWidth(floatWidth));
                    GUILayout.Label("|", GUILayout.MaxWidth(7.5f));
                    sound.audioVolume.y = EditorGUILayout.FloatField("", sound.audioVolume.y, GUILayout.MaxWidth(floatWidth));
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(2.5f);
                
                GUILayout.BeginHorizontal();
                {
                    EditorTools.MinMaxSlider("Pitch", "Pitch of the sound." +
                        "\n To add randomness, increase the range between the two handles.",
                        ref sound.audioPitch.x, ref sound.audioPitch.y, 0.25f, 1.75f,
                        60);
                    
                    sound.audioPitch.x = EditorGUILayout.FloatField("", sound.audioPitch.x, GUILayout.MaxWidth(floatWidth));
                    GUILayout.Label("|", GUILayout.MaxWidth(7.5f));
                    sound.audioPitch.y = EditorGUILayout.FloatField("", sound.audioPitch.y, GUILayout.MaxWidth(floatWidth));
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(1.5f);
                
                EditorTools.Toggle("Loop", "When the track ends, it will automatically replay.",
                    ref sound.loop, 100);
                
                EditorTools.DrawLine(0.5f, 2.5f, 2.5f);

                if (EditorTools.Button("Play", "Plays the selected sound."))
                {
                    AudioManager.CreateSource(index, sound);
                    sound.Play();
                }
            }
            GUILayout.EndVertical();
        }
    }
}