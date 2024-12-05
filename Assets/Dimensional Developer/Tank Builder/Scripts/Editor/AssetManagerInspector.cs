using UnityEditor;
using UnityEngine;
using DimensionalDeveloper.TankBuilder.Managers;

namespace DimensionalDeveloper.TankBuilder.Editor
{
    [CustomEditor(typeof(AssetManager))]
    public class AssetManagerInspector : EditorTemplate
    {
        protected override string ScriptName => "Asset Manager";
        
        protected override bool EnableBaseGUI => false;

        private AssetManager AssetManager => target as AssetManager;

        protected override void DrawSections() => DrawAssets(0);

        private void DrawAssets(int section)
        {
            if(DrawHeader(this, section, "Assets", "", () =>
               {
                   GUILayout.Space(-20);

                   if (TexturedButton(plusTexture,
                           "Adds a new asset.", 20f))
                       AssetManager.AddAsset();
               })) return;

            if (AssetManager.assets.Count > 0)
            {
                EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
                {
                    var index = 0;
                    foreach (var asset in AssetManager.assets)
                    {
                        GUILayout.BeginHorizontal();
                        if (Foldout(asset.name, "", ref asset.hideSection))
                        {
                            GUILayout.Space(-100);

                            if (TexturedButton(plusTexture, "Creates a copy of this sound.", 20f))
                                AssetManager.CopyAsset(asset);

                            if (TexturedButton(minusTexture, "Removes this sound.", 20f))
                            {
                                AssetManager.assets.RemoveAt(index);
                                GUILayout.EndHorizontal();
                                return;
                            }
                            GUILayout.EndHorizontal();
                            
                            DrawLine(0.5f, 2.5f, 5f);
                            
                            DrawAsset(index, asset);
                        }
                        else GUILayout.EndHorizontal();

                        if(index < AssetManager.assets.Count - 1) 
                            DrawLine(0.5f, 0, 2.5f);

                        index++;
                    }
                }
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(5);
        }

        private void DrawAsset(int index, Asset asset)
        {
            GUI.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = guiColorBackup;
            {
                asset.name = StringField("Name", "Name of the sound. This will be used within the code.",
                    asset.name, 100);
            
                EditorGUIUtility.labelWidth = 100;
                asset.prefab = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Prefab", "The prefab to be instantiated."),
                    asset.prefab, typeof(GameObject), true);
                
                DrawLine(0.5f, 0, 2.5f);
            
                Label("Size:", "Size of the asset when instantiated.", 100);
                
                const float floatWidth = 32.5f;
                
                GUILayout.BeginHorizontal();
                {
                    MinMaxSlider("X", "The scale of the asset on x axis when instantiated." +
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
                    MinMaxSlider("Y", "The scale of the asset on y axis when instantiated." +
                        "\n To add randomness, increase the range between the two handles.",
                        ref asset.scaleY.x, ref asset.scaleY.y, 0.05f, 2.0f, 20);
                    
                    asset.scaleY.x = EditorGUILayout.FloatField("", asset.scaleY.x, GUILayout.MaxWidth(floatWidth));
                    GUILayout.Label("|", GUILayout.MaxWidth(7.5f));
                    asset.scaleY.y = EditorGUILayout.FloatField("", asset.scaleY.y, GUILayout.MaxWidth(floatWidth));
                }
                GUILayout.EndHorizontal();
            
                DrawLine(0.5f, 5.0f, 2.5f);
            
                Toggle("Infinite Life", "Does the asset ever time out by itself?",
                    ref asset.infiniteLife, 100);

                if (!asset.infiniteLife)
                    asset.lifeDuration = FloatField("Life Duration",
                        "How long before the asset destroys itself?", asset.lifeDuration, 100);
            }
            GUILayout.EndVertical();
        }
    }
}