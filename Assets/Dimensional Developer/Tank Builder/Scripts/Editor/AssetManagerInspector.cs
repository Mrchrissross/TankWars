using NUnit.Framework;
using TankWars.Managers;
using UnityEditor;
using UnityEngine;

namespace TankWars.Editor
{
    [CustomEditor(typeof(AssetManager))]
    public class AssetManagerInspector : UnityEditor.Editor
    {
        private const float BoxMinWidth = 300f;
        private const float BoxMaxWidth = 1000f;
        
        private GUIStyle _boxStyle;
        private GUIStyle _foldoutStyle;

        private AssetManager AssetManager => target as AssetManager;

        private void OnEnable() => EditorTools.InitTextures();
        
        public override void OnInspectorGUI()
        {
            EditorTools.InitStyles(out _boxStyle, out _foldoutStyle);
            
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(target, "AssetManager");
            
            DrawSections();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUIUtility.labelWidth = 150;
            
            // base.OnInspectorGUI();
        }
        
        private void DrawSections() => DrawAssets(0);

        private void DrawAssets(int section)
        {
            GUILayout.BeginHorizontal();
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 13
                };
            
                EditorGUILayout.LabelField(new GUIContent("Assets", ""), style);

                GUILayout.Space(-20);

                if (EditorTools.TexturedButton(EditorTools.plusTexture,
                    "Adds a new sound.", 20f))
                    AssetManager.AddAsset();
            }
            GUILayout.EndHorizontal();
            
            EditorTools.DrawLine(0.5f, 0, 2.5f);

            if (AssetManager.assets.Count > 0)
            {
                EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
                {
                    var index = 0;
                    foreach (var asset in AssetManager.assets)
                    {
                        GUILayout.BeginHorizontal();
                        if (EditorTools.Foldout(asset.name, "", ref asset.hideSection))
                        {
                            GUILayout.Space(-100);

                            if (EditorTools.TexturedButton(EditorTools.plusTexture, "Creates a copy of this sound.", 20f))
                                AssetManager.CopyAsset(asset);

                            if (EditorTools.TexturedButton(EditorTools.minusTexture, "Removes this sound.", 20f))
                            {
                                AssetManager.assets.RemoveAt(index);
                                GUILayout.EndHorizontal();
                                return;
                            }
                            GUILayout.EndHorizontal();
                            
                            EditorTools.DrawLine(0.5f, 2.5f, 5f);
                            
                            DrawAsset(index, asset);
                        }
                        else GUILayout.EndHorizontal();

                        if(index < AssetManager.assets.Count - 1) 
                            EditorTools.DrawLine(0.5f, 0, 2.5f);

                        index++;
                    }
                }
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(5);
        }

        private void DrawAsset(int index, Asset asset)
        {
            GUILayout.BeginVertical("box");
            {
                asset.name = EditorTools.StringField("Name", "Name of the sound. This will be used within the code.",
                    asset.name, 100);
            
                EditorGUIUtility.labelWidth = 100;
                asset.prefab = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Prefab", "The prefab to be instantiated."),
                    asset.prefab, typeof(GameObject), true);
                
                EditorTools.DrawLine(0.5f, 0, 2.5f);
            
                EditorTools.Label("Size:", "Size of the asset when instantiated.", 100);
                
                const float floatWidth = 32.5f;
                
                GUILayout.BeginHorizontal();
                {
                    EditorTools.MinMaxSlider("X", "The scale of the asset on x axis when instantiated." +
                        "\n To add randomness, increase the range between the two handles.",
                        ref asset.scaleX.x, ref asset.scaleX.y, 0.05f, 2.0f, 20);
                
                    asset.scaleX.x = EditorGUILayout.FloatField("", asset.scaleX.x, GUILayout.MaxWidth(floatWidth));
                    GUILayout.Label("|", GUILayout.MaxWidth(7.5f));
                    asset.scaleX.y = EditorGUILayout.FloatField("", asset.scaleX.y, GUILayout.MaxWidth(floatWidth));
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(2.5f);
                
                GUILayout.BeginHorizontal();
                {
                    EditorTools.MinMaxSlider("Y", "The scale of the asset on y axis when instantiated." +
                        "\n To add randomness, increase the range between the two handles.",
                        ref asset.scaleY.x, ref asset.scaleY.y, 0.05f, 2.0f, 20);
                    
                    asset.scaleY.x = EditorGUILayout.FloatField("", asset.scaleY.x, GUILayout.MaxWidth(floatWidth));
                    GUILayout.Label("|", GUILayout.MaxWidth(7.5f));
                    asset.scaleY.y = EditorGUILayout.FloatField("", asset.scaleY.y, GUILayout.MaxWidth(floatWidth));
                }
                GUILayout.EndHorizontal();
            
                EditorTools.DrawLine(0.5f, 5.0f, 2.5f);
            
                EditorTools.Toggle("Infinite Life", "Does the asset ever time out by itself?",
                    ref asset.infiniteLife, 100);

                if (!asset.infiniteLife)
                    asset.lifeDuration = EditorTools.FloatField("Life Duration",
                        "How long before the asset destroys itself?", asset.lifeDuration, 100);
            }
            GUILayout.EndVertical();
        }
    }
}