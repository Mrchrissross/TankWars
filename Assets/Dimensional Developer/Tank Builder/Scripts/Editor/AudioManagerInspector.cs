using UnityEditor;
using UnityEngine;
using DimensionalDeveloper.TankBuilder.Managers;

namespace DimensionalDeveloper.TankBuilder.Editor
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerInspector : EditorTemplate
    {
        protected override string ScriptName => "Audio Manager";
        
        protected override bool EnableBaseGUI => false;
        
        private AudioManager AudioManager => target as AudioManager;

        protected override void DrawSections() => DrawSounds(0);

        private void DrawSounds(int section)
        {
            if(DrawHeader(this, section, "Audio", "", () =>
            {
                GUILayout.Space(-20);

                if (TexturedButton(plusTexture, "Adds a new sound."))
                    AudioManager.AddSound();
            })) return;

            if (AudioManager.sounds.Count > 0)
            {
                EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
                {
                    var index = 0;
                    foreach (var sound in AudioManager.sounds)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (!Foldout(sound.name, "", ref sound.hideSection))
                            {
                                GUILayout.EndHorizontal();
                                
                                if(index < AudioManager.sounds.Count - 1) 
                                    DrawLine(0.5f, 1.5f, 5f);
                                
                                index++;
                                continue;
                            }
                    
                            GUILayout.Space(-100);

                            if (TexturedButton(plusTexture, "Creates a copy of this sound."))
                                AudioManager.CopySound(sound);

                            if (TexturedButton(minusTexture, "Removes this sound."))
                            {
                                AudioManager.sounds.RemoveAt(index);
                                GUILayout.EndHorizontal();
                                break;
                            }
                        }
                        GUILayout.EndHorizontal();
                
                        if(index < AudioManager.sounds.Count - 1) 
                            DrawLine(0.5f, 1.5f, 5f);
                        
                        DrawSound(index, sound);

                        index++;
                    }
                }
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(5);
        }

        private void DrawSound(int index, Sound sound)
        {
            GUI.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = guiColorBackup;
            {
                sound.name = StringField("Name", "Name of the sound. This will be used within the code.",
                    sound.name, 100);

                EditorGUIUtility.labelWidth = 100;
                sound.clip = (AudioClip) EditorGUILayout.ObjectField(new GUIContent("Clip", "The audio clip to be played."),
                    sound.clip, typeof(AudioClip), true);
                
                DrawLine(0.5f, 0, 2.5f);

                const float floatWidth = 32.5f;
                
                GUILayout.BeginHorizontal();
                {
                    MinMaxSlider("Volume", "Volume of the sound." +
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
                    MinMaxSlider("Pitch", "Pitch of the sound." +
                        "\n To add randomness, increase the range between the two handles.",
                        ref sound.audioPitch.x, ref sound.audioPitch.y, 0.25f, 1.75f,
                        60);
                    
                    sound.audioPitch.x = EditorGUILayout.FloatField("", sound.audioPitch.x, GUILayout.MaxWidth(floatWidth));
                    GUILayout.Label("|", GUILayout.MaxWidth(7.5f));
                    sound.audioPitch.y = EditorGUILayout.FloatField("", sound.audioPitch.y, GUILayout.MaxWidth(floatWidth));
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(1.5f);
                
                Toggle("Loop", "When the track ends, it will automatically replay.",
                    ref sound.loop, 100);
                
                DrawLine(0.5f, 2.5f, 2.5f);

                if (Button("Play", "Plays the selected sound."))
                {
                    AudioManager.CreateSource(index, sound);
                    sound.Play();
                }
            }
            GUILayout.EndVertical();
        }
    }
}