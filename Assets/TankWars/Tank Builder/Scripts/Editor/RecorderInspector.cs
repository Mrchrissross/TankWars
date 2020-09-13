using TankWars.Utility;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace TankWars.Editor
{
    [CustomEditor(typeof(Recorder))]
    public class RecorderInspector : UnityEditor.Editor
    {
        private const float BoxMinWidth = 300f;
        private const float BoxMaxWidth = 1000f;
        
        private GUIStyle _boxStyle;
        private GUIStyle _foldoutStyle;

        private SerializedProperty onPauseEvents;
        private SerializedProperty onResumeEvents;
        
        private Recorder recorder => target as Recorder;
        
        private void OnEnable()
        {
            EditorTools.InitTextures();
            onPauseEvents = serializedObject.FindProperty("onPauseEvents");
            onResumeEvents = serializedObject.FindProperty("onResumeEvents");
        }

        public override void OnInspectorGUI()
        {
            EditorTools.InitStyles(out _boxStyle, out _foldoutStyle);
            
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(target, "Recorder");
            
            DrawContent();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUIUtility.labelWidth = 150;
            
            // base.OnInspectorGUI();
        }

        private void DrawContent()
        {
            EditorTools.DrawHeader("Recorder");
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                GUILayout.Space(2.5f);
                
                if (Application.isPlaying)
                { 
                    var showIndex = false;
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        var recorderLabel = "Recording";
                        
                        if (recorder.IsRewinding) recorderLabel = "<b>Rewinding</b>";
                        else if (recorder.IsFastForwarding) recorderLabel = "<b>Fast Forwarding</b>";
                        else if (recorder.IsPaused) recorderLabel = "<b>Paused</b>";

                        if (recorderLabel != "Recording") showIndex = true;
                        
                        EditorTools.Label(recorderLabel, "", 
                            120, 0, new GUIStyle(GUI.skin.label)
                            {
                                alignment = TextAnchor.MiddleCenter,
                                richText = true
                            });

                        if(showIndex)
                            EditorTools.ReadOnlyValue("Frame", "", recorder.index,"0", 60);
                        
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    

                    if (showIndex)
                    {
                        var rect = GUILayoutUtility.GetLastRect();
                        rect.y += 20.0f;
                
                        recorder.index = (int)GUI.HorizontalSlider(rect, recorder.index, 0, recorder.records.Count - 1);
                        
                        GUILayout.Space(17.5f);
                    }
                    
                    EditorTools.DrawLine(0.5f, 5, 5f);
                }
                
                recorder.recordTime = EditorTools.Slider("Record Length", "", recorder.recordTime, 0, 10, 100);

                EditorTools.DrawLine(0.5f, 5, 2.5f);
                
                EditorTools.Label("Keys: ", "");
                
                GUILayout.BeginVertical("box");
                {
                    recorder.pause = EditorTools.KeyCodeDropdown("Pause", "The keyboard key to pause the recorder.", recorder.pause, 100);
                        
                    GUILayout.Space(5);

                    GUILayout.BeginVertical("box");
                    {
                        recorder.fastForward = EditorTools.KeyCodeDropdown("Fast Forward",
                            "The keyboard key to fast forward.", recorder.fastForward, 100);

                        recorder.rewind = EditorTools.KeyCodeDropdown("Rewind",
                            "The keyboard key to rewind.", recorder.rewind, 100);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical(); 
                
                GUILayout.Space(5);
                
                EditorTools.DrawLine(0.5f, 5, 2.5f);
                
                EditorTools.Label("On Pause: ", "These events will be called when the recorder is paused, " +
                                                "rewinding, or fast forwarding.", 100);
                
                GUILayout.BeginVertical("box");
                {
                    
                    // LookLikeControls Obsolete.
                    
#pragma warning disable 618
                    EditorGUIUtility.LookLikeControls();
#pragma warning restore 618

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(onPauseEvents);
                    EditorGUI.indentLevel--;
                    
#pragma warning disable 618
                    EditorGUIUtility.LookLikeControls();
#pragma warning restore 618

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(onResumeEvents);
                    EditorGUI.indentLevel--;
                }
                GUILayout.EndVertical();
                
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(5);
        }
    }
}