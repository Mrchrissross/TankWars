using TankWars.Utility;
using UnityEngine;
using UnityEditor;

namespace TankWars.Editor
{
    [CustomEditor(typeof(Recorder))]
    public class RecorderInspector : UnityEditor.Editor
    {
        private GUIStyle _boxStyle;
        private Recorder recorder => target as Recorder;
        
        private void InitStyles()
        {
            _boxStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(5, 5, 5, 2), 
                normal = {background = Texture2D.whiteTexture}
            };
        }
        
        public override void OnInspectorGUI()
        {
            EditorTools.InitStyles(out _boxStyle);
            
            EditorTools.DrawHeader("Recorder");
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(370), GUILayout.MaxWidth(1000));
            {
                GUILayout.Space(2.5f);
                
                if (Application.isPlaying)
                { 
                    var showIndex = false;
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        var recorderLabel = "Recording";
                        
                        if (recorder.isRewinding) recorderLabel = "<b>Rewinding</b>";
                        else if (recorder.isFastForwarding) recorderLabel = "<b>Fast Forwarding</b>";
                        else if (recorder.isPaused) recorderLabel = "<b>Paused</b>";

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
                    
                    GUILayout.Space(4);

                    if (showIndex)
                    {
                        var rect = GUILayoutUtility.GetLastRect();
                        rect.y += 2.5f;
                
                        recorder.index = (int)GUI.HorizontalSlider(rect, recorder.index, 0, recorder.records.Count - 1);
                        
                        GUILayout.Space(17.5f);
                    }
                    
                    EditorTools.DrawLine(0.5f, 5, 5f);
                }
                
                recorder._recordTime = EditorTools.Slider("Record Length", "", recorder._recordTime, 0, 10, 100);

                EditorTools.DrawLine(0.5f, 5, 2.5f);
                
                EditorTools.Label("Keys: ", "");                
                GUILayout.BeginHorizontal();
                {
                    recorder.pause = EditorTools.KeyCodeDropdown("Pause", "The keyboard key to pause the recorder.", recorder.pause, 125);
                }
                GUILayout.EndHorizontal();
                    
                GUILayout.Space(5);
                
                GUILayout.BeginHorizontal();
                {
                    recorder.rewind = EditorTools.KeyCodeDropdown("Rewind",
                        "The keyboard key to rewind.", recorder.rewind, 55);
                    
                    recorder.fastForward = EditorTools.KeyCodeDropdown("Fast Forward",
                        "The keyboard key to fast forward.", recorder.fastForward, 85);
                }
                GUILayout.EndHorizontal();
                    
                GUILayout.Space(5);
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(5);

            Undo.RecordObject(target, "character");
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            
            EditorGUIUtility.labelWidth = 150;
            
            // base.OnInspectorGUI();
        }
    }
}