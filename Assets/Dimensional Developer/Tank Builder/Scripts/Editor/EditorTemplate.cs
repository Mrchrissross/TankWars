using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;

namespace DimensionalDeveloper.TankBuilder.Editor
{
    /// <summary>
    /// Tools used to assist in the creation of editor scripts. Redistribution is not allowed.
    /// </summary>
    
    public abstract class EditorTemplate  : UnityEditor.Editor
    {
        
        #region Fields

        protected const float BoxMinWidth = 300f;
        protected const float BoxMaxWidth = 1000f;
        
        protected GUIStyle _boxStyle;
        protected GUIStyle _foldoutStyle;

        protected abstract string ScriptName { get; }
        protected abstract bool EnableBaseGUI { get; }
        protected virtual float LabelWidth => 110.0f;
        
        #endregion

        
        
        #region Editor Methods

        protected virtual void OnEnable() => InitTextures();
        
        protected virtual void OnDisable() {}
        
        public override void OnInspectorGUI()
        {
            InitStyles(out _boxStyle, out _foldoutStyle);
            
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(target, ScriptName);
            
            DrawSections();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUIUtility.labelWidth = 150;

            if (serializedObject.ApplyModifiedProperties() || 
                (Event.current.type == EventType.ExecuteCommand && 
                 Event.current.commandName == "UndoRedoPerformed")) OnUndoRedo();
            
            if (!EnableBaseGUI) return;

            DrawLine(0.5f, 12.5f, 2.5f);
            DrawLine(0.5f, 2.5f, 2.5f);
            DrawLine(0.5f, 2.5f, 2.5f);
            DrawLine(0.5f, 2.5f, 12.5f);
            
            ShowBaseGUI();
            
            DrawLine(0.5f, 12.5f, 2.5f);
            DrawLine(0.5f, 2.5f, 2.5f);
            DrawLine(0.5f, 2.5f, 2.5f);
            DrawLine(0.5f, 2.5f, 12.5f);
        }
        
        public string[] GetSortingLayerNames() {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            var sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
            return sortingLayers;
        }
        
        #endregion
        


        #region Virtual Methods
        
        /// <summary>
        /// Draws the individual sections.
        /// </summary>
        protected virtual void DrawSections() {}
        
        /// <summary>
        /// If an undo/redo command has been executed, this method will be called.
        /// </summary>
        protected virtual void OnUndoRedo() {}

        protected virtual void ShowBaseGUI() => base.OnInspectorGUI();
        
        #endregion
        
        
        
        #region Utilities

        
            #region Textures

            private const string Path = "Editor UI/";
            public static Texture2D cameraTexture;
            public static Texture2D deleteTexture;
            public static Texture2D editTexture;
            public static Texture2D eyeClosedTexture;
            public static Texture2D eyeOpenTexture;
            public static Texture2D minusTexture;
            public static Texture2D pauseTexture;
            public static Texture2D playTexture;
            public static Texture2D plusTexture;
            public static Texture2D settingsTexture;
            public static Texture2D tankTexture;
            
            public static Color guiColorBackup;
            
            /// <summary>
            /// Initialises a texture.
            /// </summary>
            /// <param name="path">The path to the texture.</param>
            
            public static Texture2D InitTexture(string path)
            {
                return Resources.Load<Texture2D>(path);
            }

            /// <summary>
            /// Initialises all textures.
            /// </summary>
            
            public static void InitTextures()
            {
                cameraTexture = InitTexture(Path + "camera");
                deleteTexture = InitTexture(Path + "delete");
                editTexture = InitTexture(Path + "edit");
                eyeClosedTexture = InitTexture(Path + "eye_closed");
                eyeOpenTexture = InitTexture(Path + "eye_open");
                minusTexture = InitTexture(Path + "minus");
                pauseTexture = InitTexture(Path + "pause");
                playTexture = InitTexture(Path + "play");
                plusTexture = InitTexture(Path + "plus");
                settingsTexture = InitTexture(Path + "settings");
                tankTexture = InitTexture(Path + "tank");

                guiColorBackup = GUI.backgroundColor;
            }
            
            #endregion

            
            
            #region Labels

            public static void DrawHeader(string label, string tooltip = "", float space = 5, GUIStyle style = null)
            {
                if (style == null)
                {
                    style = new GUIStyle(GUI.skin.label)
                    {
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 13
                    };
                }
                
                EditorGUILayout.LabelField(new GUIContent(label, tooltip), style);
                DrawLine();
                Space(space);
            }
            
            public static bool DrawHeader<T>(T type, int index, string header, string tooltip = "", Action extraContent = null, float space = 5, 
                GUIStyle style = null)
            {
                bool hideToggle = PlayerPrefs.GetInt($"{type.ToString()}: Section Enabled {index}", 1) == 1;

                GUILayout.BeginHorizontal();
                {
                    if (!hideToggle)
                    {
                        if (TexturedButton(eyeOpenTexture,
                            "Hides all the content in this section.", 20f))
                            hideToggle = true;
                        
                        GUILayout.Space(-20);
                    }
                    else
                    {
                        if (TexturedButton(eyeClosedTexture,
                            "Shows all the content in this section.", 20f))
                            hideToggle = false;
                        
                        GUILayout.Space(-20);
                    }

                    if (style == null)
                    {
                        style = new GUIStyle(GUI.skin.label)
                        {
                            fontStyle = FontStyle.Bold,
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 13
                        };
                    }
                
                    EditorGUILayout.LabelField(new GUIContent(header, tooltip), style);

                    extraContent?.Invoke();
                }
                GUILayout.EndHorizontal();
                
                DrawLine();
                GUILayout.Space(space);

                PlayerPrefs.SetInt($"{type.ToString()}: Section Enabled {index}", hideToggle ? 1 : 0);
                
                return hideToggle;
            }
            
            public static void Label(string label, string tooltip, float width = 50, float space = 0, GUIStyle style = null)
            {
                EditorGUILayout.LabelField(new GUIContent(label, tooltip), style ?? EditorStyles.label, GUILayout.Width(width));
                Space(space);
            }

            public static void ReadOnlyValue(string label, string tooltip, float value, string decimalPlaces = "0.00", float labelWidth = 50, float valueWidth = 50, float space = 0, 
                GUIStyle labelStyle = null, GUIStyle valueStyle = null)
            {
                Label(label, tooltip, labelWidth, 0, labelStyle);
                Label(value.ToString(decimalPlaces), "", valueWidth, 0, valueStyle);
                
                Space(space);
            }
            
            #endregion

            
            
            #region Buttons

            /// <summary>
            /// Draws an icon in the inspector.
            /// </summary>
            /// <param name="texture">The texture of the icon.</param>
            /// <param name="size">The size of the icon.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the element.</param>
            /// <returns>Returns true when the button is pressed.</returns>
            public static bool TexturedButton(Texture2D texture, string tooltip, float size) => GUILayout.Button(new GUIContent(texture, tooltip), 
                GUIStyle.none, GUILayout.Width(size), GUILayout.Height(size));

            /// <summary>
            /// Draws a button in the inspector.
            /// </summary>
            /// <param name="text">The text displayed on the button.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the element.</param>
            /// <returns>Returns true when the button is pressed.</returns>
            public static bool Button(string text, string tooltip) => GUILayout.Button(new GUIContent(text, tooltip));
            
            /// <summary>
            /// Draws a button in the inspector.
            /// </summary>
            /// <param name="text">The text displayed on the button.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the element.</param>
            /// <param name="width">Width of the button.</param>
            /// <returns>Returns true when the button is pressed.</returns>
            public static bool Button(string text, string tooltip, float width = 100) => GUILayout.Button(new GUIContent(text, tooltip),
                GUILayout.Width(width));
            
            /// <summary>
            /// Draws a button in the inspector.
            /// </summary>
            /// <param name="text">The text displayed on the button.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the element.</param>
            /// <param name="width">Width of the button.</param>
            /// <param name="height">Height of the button.</param>
            /// <returns>Returns true when the button is pressed.</returns>
            public static bool Button(string text, string tooltip, float width = 100, float height = 20) => GUILayout.Button(new GUIContent(text, tooltip),
                GUILayout.Width(width), GUILayout.Height(height));
            
            #endregion

            
            
            #region Fields
            
            /// <summary>
            /// Draws a int field in the inspector.
            /// </summary>
            /// <param name="label">Label of the slider.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The float within the script.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static int IntField(string label, string tooltip, int value, float width = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = width;
                value = EditorGUILayout.IntField(new GUIContent(label, tooltip), value);
                
                Space(space);

                return value;
            }
            
            /// <summary>
            /// Draws a float field in the inspector.
            /// </summary>
            /// <param name="label">Label of the slider.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The float within the script.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static float FloatField(string label, string tooltip, float value, float width = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = width;
                value = EditorGUILayout.FloatField(new GUIContent(label, tooltip), value);
                
                Space(space);

                return value;
            }
            
            /// <summary>
            /// Draws a float field in the inspector.
            /// </summary>
            /// <param name="label">Label of the slider.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The float within the script.</param>
            /// <param name="labelWidth">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static string StringField(string label, string tooltip, string value, float labelWidth = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = labelWidth;
                value = EditorGUILayout.TextField(new GUIContent(label, tooltip), value);
                
                Space(space);

                return value;
            }
            
            /// <summary>
            /// Draws a Vector 2 field in the inspector.
            /// </summary>
            /// <param name="label">Label of the vector.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The vector within the script.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static Vector2 VectorField(string label, string tooltip, Vector2 value, float width = 50, float space = 0)
            {
                var rect = EditorGUILayout.BeginHorizontal();
                {
                    rect.y += 1.5f;
                    Label("", "");
                    EditorGUI.LabelField(rect, new GUIContent(label, tooltip));

                    var rectWidth = rect.width - width;
                    rect.x += width;

                    // X
                    var vectorLabelWidth = rectWidth * (0.15f / 2f);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, vectorLabelWidth, rect.height), 
                        new GUIContent("X", ""));
                        
                    var vectorFieldWidth = rectWidth * (0.8f / 2f);
                    value.x = EditorGUI.FloatField(
                        new Rect(rect.x += vectorLabelWidth, rect.y, vectorFieldWidth, rect.height), value.x);

                    // Y
                    EditorGUI.LabelField(new Rect(rect.x += vectorFieldWidth + 10f, rect.y, vectorLabelWidth, rect.height), 
                        new GUIContent("Y", ""));
                        
                    value.y = EditorGUI.FloatField(
                        new Rect(rect.x += vectorLabelWidth, rect.y, vectorFieldWidth, rect.height), value.y);
                }
                EditorGUILayout.EndHorizontal();
                
                Space(space);

                return value;
            }

            /// <summary>
            /// Draws a Vector 3 field in the inspector.
            /// </summary>
            /// <param name="label">Label of the vector.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The vector within the script.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static Vector3 VectorField(string label, string tooltip, Vector3 value, float width = 50, float space = 0)
            {
                var rect = EditorGUILayout.BeginHorizontal();
                {
                    rect.y += 1.5f;
                    Label("", "");
                    EditorGUI.LabelField(rect, new GUIContent(label, tooltip));

                    var rectWidth = rect.width - width;
                    rect.x += width;

                    // X
                    var vectorLabelWidth = rectWidth * (0.2f / 3f);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, vectorLabelWidth, rect.height), 
                        new GUIContent("X", ""));
                        
                    var vectorFieldWidth = rectWidth * (0.705f / 3f);
                    value.x = EditorGUI.FloatField(
                        new Rect(rect.x += vectorLabelWidth, rect.y, vectorFieldWidth, rect.height), value.x);

                    // Y
                    EditorGUI.LabelField(new Rect(rect.x += vectorFieldWidth + 10f, rect.y, vectorLabelWidth, rect.height), 
                        new GUIContent("Y", ""));
                        
                    value.y = EditorGUI.FloatField(
                        new Rect(rect.x += vectorLabelWidth, rect.y, vectorFieldWidth, rect.height), value.y);
                    
                    // Z
                    EditorGUI.LabelField(new Rect(rect.x += vectorFieldWidth + 10f, rect.y, vectorLabelWidth, rect.height), 
                        new GUIContent("Z", ""));
                        
                    value.z = EditorGUI.FloatField(
                        new Rect(rect.x += vectorLabelWidth, rect.y, vectorFieldWidth, rect.height), value.z);
                }
                EditorGUILayout.EndHorizontal();
                
                Space(space);

                return value;
            }

            public static Vector2Int VectorField(string label, string tooltip, Vector2Int value, float width = 50, float space = 0)
            {
                var field = VectorField(label, tooltip, (Vector2)value, width, space);

                var x = Mathf.RoundToInt(field.x);
                var y = Mathf.RoundToInt(field.y);

                return new Vector2Int(x, y);
            }

            public static Vector3Int VectorField(string label, string tooltip, Vector3Int value, float width = 50, float space = 0)
            {
                var field = VectorField(label, tooltip, (Vector3)value, width, space);

                var x = Mathf.RoundToInt(field.x);
                var y = Mathf.RoundToInt(field.y);
                var z = Mathf.RoundToInt(field.z);

                return new Vector3Int(x, y, z);
            }
            
            // ---------------------------------------------------------------------------------------------------------

            /// <summary>
            /// Recommended: Draws an object field directly.
            /// Usage: ObjectField("Player Transform", "The players transform.", ref script.playerTransform, 50, 0, false);
            /// </summary>
            /// <param name="label">Label of the slider.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The value within the script.</param>
            /// <param name="labelWidth">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            /// <param name="allowSceneReferences">Whether to allow references from the scene.</param>
            /// <typeparam name="T">The script type.</typeparam>
            
            public static void ObjectField<T>(string label, string tooltip, ref T value, float labelWidth = 50, float space = 0, bool allowSceneReferences = true) where T : UnityEngine.Object
            {
                EditorGUIUtility.labelWidth = labelWidth;
                
                value = (T)EditorGUILayout.ObjectField(new GUIContent(label, tooltip), value, typeof(T), allowSceneReferences);
                
                if(space > 0) Space(space);
            }

            /// <summary>
            /// Draws an object field.
            /// Usage:  Script.playerTransform = ObjectField("Player Transform",
            ///         "The players transform.", typeof(*ScriptType*), Script.playerTransform, 50, 0, false) as *ScriptType*;
            /// </summary>
            /// <param name="label">Label of the slider.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="type">The type of component you desire. Use typeof(*Your Component*).</param>
            /// <param name="value">The value within the script.</param>
            /// <param name="labelWidth">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            /// <param name="allowSceneReferences">Whether to allow references from the scene.</param>
            
            public static Object ObjectField(string label, string tooltip, Type type, Object value, float labelWidth = 50, float space = 0, bool allowSceneReferences = true)
            {
                EditorGUIUtility.labelWidth = labelWidth;
                
                value = EditorGUILayout.ObjectField(new GUIContent(label, tooltip), value, type, allowSceneReferences);
                
                if(space > 0) Space(space);

                return value;
            }
            
            // ---------------------------------------------------------------------------------------------------------

            /// <summary>
            /// Draws a color field in the inspector.
            /// </summary>
            /// <param name="label">Label of the toggle.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The bool within the script.</param>
            /// <param name="side">true = color field is displayed beside label.
            ///                     false = color field is displayed under label.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element.</param>
            public static Color ColorField(string label, string tooltip, Color value, bool side = true, float width = 50, float space = 0)
            {
                if(side)EditorGUILayout.BeginHorizontal();
                else EditorGUILayout.BeginVertical();
                
                EditorGUIUtility.labelWidth = width;
                var newColor = EditorGUILayout.ColorField(new GUIContent(label, tooltip), value, true, true, false);
                
                if(side)EditorGUILayout.EndHorizontal();
                else EditorGUILayout.EndVertical();
                
                Space(space);

                return newColor;
            }
            
            /// <summary>
            /// Draws the layer mask dropdown.
            /// </summary>
            /// <param name="label">Label of the toggle.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="layerMask">The current layer mask.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            /// <returns></returns>
            public static LayerMask LayerMaskField (string label, string tooltip, LayerMask layerMask, float width = 50, float space = 0) 
            {
                var layers = new List<string>();
                var layerNumbers = new List<int>();
     
                for (var i = 0; i < 32; i++) 
                {
                    var layerName = LayerMask.LayerToName(i);
                    
                    if (layerName == "") continue;
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
                
                var maskWithoutEmpty = 0;
                for (var i = 0; i < layerNumbers.Count; i++) 
                {
                    if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                        maskWithoutEmpty |= (1 << i);
                }
                
                EditorGUIUtility.labelWidth = width;
                maskWithoutEmpty = EditorGUILayout.MaskField(new GUIContent(label, tooltip), maskWithoutEmpty, layers.ToArray());
                
                var mask = layerNumbers.Where((t, i) => (maskWithoutEmpty & (1 << i)) > 0).Aggregate(0, 
                    (current, t) => current | (1 << t));
                
                layerMask.value = mask;
                
                Space(space);
                
                return layerMask;
            }
            
            /// <summary>
            /// Draws a popup of the project's existing sorting layers.
            /// </summary>
            /// <param name="label">Label of the field.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="layerID">A serialized property must be created in the calling script.</param>
            /// <param name="boxStyle">Style of the popup box.</param>
            /// <param name="labelStyle">Style of the label.</param>
            public static void SortingLayerField(string label, string tooltip, SerializedProperty layerID, GUIStyle boxStyle, GUIStyle labelStyle, float labelWidth = 50)
            {
                var methodInfo = typeof(EditorGUILayout).GetMethod("SortingLayerField", 
                    BindingFlags.Static | BindingFlags.NonPublic, null, new[] 
                        { typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle), typeof(GUIStyle) }, null);

                if (methodInfo == null) return;

                EditorGUIUtility.labelWidth = labelWidth;
                
                var parameters = new object[] { new GUIContent(label, tooltip), layerID, boxStyle, labelStyle };
                methodInfo.Invoke(null, parameters);
            }
            
            /// <summary>
            /// Draws a popup of the project's existing sorting layers.
            /// </summary>
            /// <param name="label">Label of the field.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="layerID">A serialized property must be created in the calling script.</param>
            /// <param name="boxStyle">Style of the popup box.</param>
            /// <param name="labelStyle">Style of the label.</param>
            public static void SortingLayerField(string label, string tooltip, ref int layerID, string[] sortingLayerNames, float labelWidth = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = labelWidth;
                
                layerID = EditorGUILayout.Popup (new GUIContent(label, tooltip), layerID, sortingLayerNames);

                Space(space);
            }
            
            /// <summary>
            /// Draws a KeyCode dropdown. 
            /// </summary>
            /// <param name="label">Label of the toggle.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="key">The current key code.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            /// <returns></returns>
            public static KeyCode KeyCodeDropdown(string label, string tooltip, KeyCode key, float width = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = width;
                var newKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent(label, tooltip), key);
                
                Space(space);

                return newKey;
            }
            
            // ---------------------------------------------------------------------------------------------------------
            
            /// <summary>
            /// Draws a slider in the inspector.
            /// </summary>
            /// <param name="label">Label of the slider.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The float within the script.</param>
            /// <param name="minimum">Minimum possible range.</param>
            /// <param name="maximum">Maximum possible range.</param>
            /// <param name="labelWidth">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            
            public static void Slider(string label, string tooltip, ref float value, float minimum, float maximum, float labelWidth = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = labelWidth;
                
                value = EditorGUILayout.Slider(new GUIContent(label, tooltip), value, minimum, maximum);
                
                Space(space);
            }
            
            /// <summary>
            /// Draws a slider in the inspector.
            /// </summary>
            /// <param name="label">Label of the slider.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The float within the script.</param>
            /// <param name="minimum">Minimum possible range.</param>
            /// <param name="maximum">Maximum possible range.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            
            public static float Slider(string label, string tooltip, float value, float minimum, float maximum, 
                float width = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = width;
                value = EditorGUILayout.Slider(new GUIContent(label, tooltip), value, minimum, maximum);
                
                Space(space);

                return value;
            }


            /// <summary>
            /// Draws a slider in the inspector.
            /// </summary>
            /// <param name="label">Label of the slider.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The float within the script.</param>
            /// <param name="minimum">Minimum possible range.</param>
            /// <param name="maximum">Maximum possible range.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static int Slider(string label, string tooltip, int value, int minimum, int maximum,
                float width = 50, float space = 0) => Mathf.RoundToInt(Slider(label, tooltip, (float)value, minimum, maximum, width, space));
            
            
            /// <summary>
            /// Draws a slider in the inspector.
            /// </summary>
            /// <param name="label">Label of the slider.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="minValue">The current minimum value within the script</param>
            /// <param name="maxValue">The current maximum value within the script</param>
            /// <param name="minimum">Minimum possible range.</param>
            /// <param name="maximum">Maximum possible range.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static void MinMaxSlider(string label, string tooltip, ref float minValue, ref float maxValue, float minimum, float maximum, float width = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = width;
                EditorGUILayout.MinMaxSlider(new GUIContent(label, tooltip), ref minValue, ref maxValue, minimum, maximum);
                
                Space(space);
            }
            
            // ---------------------------------------------------------------------------------------------------------
            
            #endregion

            
            
            #region Toggles

            /// <summary>
            /// Draws a toggle in the inspector.
            /// </summary>
            /// <param name="label">Label of the toggle.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The bool within the script.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static void Toggle(string label, string tooltip, ref bool value, float width = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = width;
                value = EditorGUILayout.Toggle(new GUIContent(label, tooltip), value);
                
                Space(space);
            }
            
            /// <summary>
            /// Draws a toggle in the inspector.
            /// </summary>
            /// <param name="label">Label of the toggle.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The bool within the script.</param>
            /// <param name="width">Width of the displayed Label.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static bool ContentToggle(string label, string tooltip, ref bool value, float width = 50, float space = 0)
            {
                EditorGUIUtility.labelWidth = width;
                value = EditorGUILayout.Toggle(new GUIContent(label, tooltip), value);
                
                Space(space);

                return value;
            }

            /// <summary>
            /// Draws a toggle in the inspector.
            /// </summary>
            /// <param name="label">Label of the toggle.</param>
            /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
            /// <param name="value">The bool within the script.</param>
            /// <param name="drawToggle">Draws the toggle box on the far right.</param>
            /// <param name="drawLine">Draws a line underneath the foldout.</param>
            /// <param name="space">Amount of space after the element. </param>
            public static bool Foldout(string label, string tooltip, ref bool value, bool drawToggle = false, bool drawLine = false, float space = 0)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    var style = new GUIStyle("ShurikenModuleTitle")
                    {
                        font = new GUIStyle(EditorStyles.label).font,
                        border = new RectOffset(15, 7, 4, 4),
                        fixedHeight = 22,
                        contentOffset = new Vector2(20f, -2f),
                        fontSize = 12,
                        normal = {background = Texture2D.blackTexture}
                    };

                    var width = Screen.width - Screen.width / 2.0f - 25;
                    var rect = GUILayoutUtility.GetRect(width, 22f, style);
                    rect.width -= 85;
                    GUI.Box(rect, new GUIContent(label, tooltip), style);
     
                    var e = Event.current;
     
                    var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
                    switch (e.type)
                    {
                        case EventType.Repaint:
                            EditorStyles.foldout.Draw(toggleRect, false, false, value, false);
                            break;
                        case EventType.MouseDown when rect.Contains(e.mousePosition):
                            value = !value;
                            e.Use();
                            break;
                    }

                    if (drawToggle)
                    {
                        value = GUILayout.Toggle(value, "");            
                        GUILayout.Space(-5);
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                if(value && drawLine) DrawLine(0.5f, 0, 5);
                
                Space(space);

                return value;
            }
            
            #endregion

            
            
            #region Utility

            public static void DrawLine(float thickness = 0.5f, float spaceBefore = 2.5f, float spaceAfter = 2.5f)
            {
                Space(spaceBefore);
                {
                    var rect = EditorGUILayout.GetControlRect(false, thickness );
                    rect.height = thickness;
                    EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
                }
                Space(spaceAfter);
            }

            public static void Space(float value) => EditorGUILayout.Space(value);

            #endregion


            
            #region Styles

            /// <summary>
            /// Initialises the only the box style.
            /// </summary>
            public static void InitStyles(out GUIStyle boxStyle)
            {
                boxStyle = new GUIStyle("box")
                {
                    padding = new RectOffset(5, 5, 5, 2), 
                };
            }
            
            /// <summary>
            /// Initialises both the box style and the foldout style.
            /// </summary>
            public static void InitStyles(out GUIStyle boxStyle, out GUIStyle foldoutStyle)
            {
                boxStyle = new GUIStyle("box")
                {
                    padding = new RectOffset(5, 5, 5, 2),
                };
                
                foldoutStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"))
                {
                    padding = new RectOffset(5, 5, 5, 2) 
                };
            }

            #endregion


        #endregion
        
    }
}