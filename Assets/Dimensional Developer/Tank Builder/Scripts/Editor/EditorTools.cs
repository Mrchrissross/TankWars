using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace DimensionalDeveloper.TankBuilder.Editor
{
    /// <summary>
    /// Tools used to assist in the creation of editor scripts. Redistribution is not allowed.
    /// </summary>
    
    public static class EditorTools
    {
        #region Textures

        private const string Path = "Editor UI/";
        public static Texture2D plusTexture;
        public static Texture2D minusTexture;
        public static Texture2D editTexture;
        public static Texture2D cameraTexture;
        public static Texture2D tankTexture;
        public static Texture2D eyeOpenTexture;
        public static Texture2D eyeClosedTexture;
        
        public static Color guiColorBackup;
        
        /// <summary>
        /// Initialises a texture.
        /// </summary>
        /// <param name="path">The path to the texture.</param>
        
        public static Texture2D InitTexture(string path) => Resources.Load<Texture2D>(path);

        /// <summary>
        /// Initialises all textures.
        /// </summary>
        
        public static void InitTextures()
        {
            cameraTexture = InitTexture(Path + "camera");
            editTexture = InitTexture(Path + "edit");
            eyeClosedTexture = InitTexture(Path + "eye_closed");
            eyeOpenTexture = InitTexture(Path + "eye_open");
            minusTexture = InitTexture(Path + "minus");
            plusTexture = InitTexture(Path + "plus");
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
        
        public static bool DrawHeader(string header, ref bool hideToggle, string tooltip = "", float space = 5, 
            GUIStyle style = null)
        {
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
            }
            GUILayout.EndHorizontal();
            
            DrawLine();
            GUILayout.Space(space);

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
        /// Draws a transform field.
        /// </summary>
        /// <param name="label">Label of the slider.</param>
        /// <param name="tooltip">The tooltip to display when hovering over the label.</param>
        /// <param name="value">The transform within the script.</param>
        /// <param name="labelWidth">Width of the displayed Label.</param>
        /// <param name="space">Amount of space after the element. </param>
        public static Transform TransformField(string label, string tooltip, Transform value, float labelWidth = 50, float space = 0)
        {
            EditorGUIUtility.labelWidth = labelWidth;
            value = (Transform) EditorGUILayout.ObjectField(new GUIContent(label, tooltip), value, 
                typeof(Transform), true);
            
            if(space > 0) Space(space);

            return value;
        }

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
        public static void SortingLayerField(string label, string tooltip, SerializedProperty layerID, GUIStyle boxStyle, GUIStyle labelStyle)
        {
            var methodInfo = typeof(EditorGUILayout).GetMethod("SortingLayerField", 
                BindingFlags.Static | BindingFlags.NonPublic, null, new[] 
                    { typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle), typeof(GUIStyle) }, null);

            if (methodInfo == null) return;
            
            var parameters = new object[] { new GUIContent(label, tooltip), layerID, boxStyle, labelStyle };
            methodInfo.Invoke(null, parameters);
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

        
        
        #region Util

        public static void DrawLine(float thickness = 1, float spaceBefore = 0, float spaceAfter = 0)
        {
            Space(spaceBefore);
            {
                var rect = EditorGUILayout.GetControlRect(false, thickness );
                rect.height = thickness;
                EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
            }
            Space(spaceAfter);
        }

        private static void Space(float value) => EditorGUILayout.Space(value);

        #endregion


        
        #region Styles

        /// <summary>
        /// Initialises the only the box style.
        /// </summary>
        public static void InitStyles(out GUIStyle boxStyle)
        {
            boxStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(5, 5, 5, 2), 
                normal = {background = Texture2D.whiteTexture}
            };
        }
        
        /// <summary>
        /// Initialises both the box style and the foldout style.
        /// </summary>
        public static void InitStyles(out GUIStyle boxStyle, out GUIStyle foldoutStyle)
        {
            boxStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(5, 5, 5, 2), 
                normal = {background = Texture2D.whiteTexture}
            };
            
            foldoutStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"))
            {
                padding = new RectOffset(5, 5, 5, 2) 
            };
        }

        #endregion

    }
}