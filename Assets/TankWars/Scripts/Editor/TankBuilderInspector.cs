using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TankWars.Utility;
using UnityEditor;
using UnityEngine;

namespace TankWars.Editor
{
    [CustomEditor(typeof(TankBuilder)), CanEditMultipleObjects]
    public class TankBuilderInspector : UnityEditor.Editor
    {
        private const float BoxMinWidth = 300f;
        private const float BoxMaxWidth = 1000f;
        
        private GUIStyle _boxStyle;
        private GUIStyle _foldoutStyle;
        private SerializedProperty sortingLayerID;

        private bool eraseAll;
        private bool addCategory;

        private string categoryName;
        private string folderLocation;
        
        private bool newSortingLayer;
        private Color guiColorBackup;
        
        private const string path = "TankWars/Sprites/";
        private string[] compartmentStyles;
        private string[] boxStyles;
        private string[] accessoryStyles;
        
        private Texture2D _plusTexture;
        private Texture2D _minusTexture;
        private Texture2D _editTexture;
        
        private TankBuilder tankBuilder => target as TankBuilder;

        private void OnEnable()
        {
            // Save the original background color.
            guiColorBackup = GUI.backgroundColor;
            
            // Setup the SerializedProperties
            sortingLayerID = serializedObject.FindProperty("sortingLayerID");
            
            // Setup the textures
            const string path = "TankWars/EditorUI/";
            _plusTexture = EditorTools.InitTexture(path + "plus");
            _minusTexture = EditorTools.InitTexture(path + "minus");
            _editTexture = EditorTools.InitTexture(path + "edit");
        }

        private void InitAccessoryStyle(string folderName, ref string[] styles)
        {
            var sprites = Resources.LoadAll<Sprite>(path + folderName);
            styles = new string[sprites.Length];
            for (var i = 0; i < sprites.Length; i++)
                styles[i] = sprites[i].name;
        }
        
        public override void OnInspectorGUI()
        {
            EditorTools.InitStyles(out _boxStyle, out _foldoutStyle);
            
            InitAccessoryStyle("Compartments", ref compartmentStyles);
            InitAccessoryStyle("Boxes", ref boxStyles);

            DrawSpawnElements();
                
            Undo.RecordObject(target, "tankBuilder");
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            
            EditorGUIUtility.labelWidth = 150;
            
            base.OnInspectorGUI();
        }

        private void DrawSpawnElements()
        {
            EditorTools.Header("Tank Creation Tool");

            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                DrawTop();
                
                EditorTools.DrawLine(0.5f, 5f, 5);

                DrawSpawnHull();

                EditorTools.DrawLine(0.5f, 2.5f, 5);

                DrawSpawnCannon();
                
                EditorTools.DrawLine(0.5f, 2.5f, 5);

                DrawCategories();
                
                // DrawVents();
                //
                // EditorTools.DrawLine(0.5f, 2.5f, 5);
                //
                // DrawCompartments();
                //
                // EditorTools.DrawLine(0.5f, 2.5f, 5);
                //
                // DrawBoxes();
                
                // DrawCategory();
                
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
        
        
        
        private void DrawTop()
        {
            EditorGUILayout.BeginHorizontal();
            EditorTools.Label("Sorting Layer:", "The sorting layer to assign to the tank.", 110);

            EditorTools.SortingLayerField("", "", sortingLayerID, EditorStyles.popup, EditorStyles.label);
            EditorGUILayout.EndHorizontal();

            EditorTools.DrawLine(0.5f, 10f, 5);
            
            GUI.backgroundColor = Color.grey;

            if (!addCategory)
            {
                if (!eraseAll)
                {
                    if (EditorTools.Button("Erase All", "Erases everything without spawning a new tank."))
                        eraseAll = true;
                }
                else
                {
                    GUI.backgroundColor = Color.grey;
                    GUILayout.BeginVertical("box");
                    GUI.backgroundColor = new Color(1f, 0f, 0f, 1f);

                    
                    GUILayout.Space(5);
                    EditorTools.Label("Erase All:", "", 100);
                    GUILayout.BeginHorizontal();
                    EditorTools.Label("Are you sure?", "", 100);
                    if (EditorTools.Button("Yes", "Erases everything."))
                        tankBuilder.EraseAll();
                    
                    if (EditorTools.Button("No", "")) eraseAll = false;
                    
                    GUILayout.EndHorizontal(); 
                    GUILayout.Space(15);
                    
                    GUILayout.EndVertical(); 
                }
                
                GUI.backgroundColor = guiColorBackup;
                
                GUILayout.Space(5);
            }

            if (!eraseAll)
            {
                if (!addCategory)
                {
                    if (EditorTools.Button("Add Category", "Add a new category."))
                        addCategory = true;
                }
                else if(addCategory)
                {
                    GUI.backgroundColor = Color.grey;
                    GUILayout.BeginVertical("box");

                    EditorTools.Label("Add Category:", "", 100);
                    
                    EditorGUIUtility.labelWidth = 100;
                    categoryName = EditorGUILayout.TextField(new GUIContent("Name:", "Name of the category to add."),
                        categoryName);
                    
                    EditorGUIUtility.labelWidth = 100;
                    folderLocation = EditorGUILayout.TextField(
                        new GUIContent("Folder Location:", "Location of the folder in resources."),
                        folderLocation);
                    
                    GUILayout.Space(5);
                    
                    GUILayout.BeginHorizontal();
                    if (EditorTools.Button("Add", "Adds the category."))
                    {
                        tankBuilder.AddCategory(categoryName, folderLocation);
                        addCategory = false;
                        categoryName = "";
                        folderLocation = "";
                        GUI.FocusControl(null);
                    }

                    if (EditorTools.Button("Back", "Goes back to main."))
                    {
                        addCategory = false;
                        categoryName = "";
                        folderLocation = "";
                        GUI.FocusControl(null);
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.EndVertical(); 
                }
            }

            GUILayout.Space(15);

            if (EditorTools.Button("Remove Categories", ""))
                tankBuilder.RemoveAllCategories();

        }



        private void DrawSpawnHull()
        {
            GUILayout.BeginHorizontal();

            if (EditorTools.Foldout("Hull", "Spawn, toggles, or destroys the tanks hull.",
                ref tankBuilder.expandHull))
            {

                GUILayout.Space(-100);

                if (EditorTools.TexturedButton(_plusTexture, "Spawns a new hull, destroying the old one.", 20f))
                    tankBuilder.SpawnHull();

                if (tankBuilder.hull != null)
                {
                    if (EditorTools.TexturedButton(_minusTexture, "Destroys the tanks current hull.", 20f))
                        DestroyImmediate(tankBuilder.hull.gameObject);
                }

                GUILayout.EndHorizontal();

                EditorTools.DrawLine(0.5f, 5f, 5);

                if (tankBuilder.hull == null) return;

                GUILayout.BeginVertical("box");

                if (EditorTools.Button("Toggle", "Toggles the current hull."))
                    Selection.activeGameObject = tankBuilder.hull.gameObject;



                EditorTools.DrawLine(0.5f, 5f, 5);



                EditorTools.Label("Colors:", "Changes the various colors on the tanks hull.");
                {
                    EditorGUI.indentLevel++;

                    tankBuilder.hullColor = EditorTools.ColorField("Overall Color",
                        "Changes the color of the tanks hull.",
                        tankBuilder.hullColor, true, 120);
                    tankBuilder.hullAdditionalColor = EditorTools.ColorField("Additional Color",
                        "Changes the additional colors on the tanks hull.", tankBuilder.hullAdditionalColor, true, 120);
                    tankBuilder.hullShadowsColor = EditorTools.ColorField("Shadows Color",
                        "Changes the color of the shadows on the tanks hull.", tankBuilder.hullShadowsColor, true, 120);

                    EditorGUI.indentLevel--;
                }

                GUILayout.EndVertical();
            }
            else GUILayout.EndHorizontal();
        }



        private void DrawSpawnCannon()
        {
            EditorTools.BeginHorizontalGroup();

            if (EditorTools.Foldout("Cannon", "Spawn, toggles, or destroys the tanks Cannon.",
                ref tankBuilder.expandCannon))
            {

                GUILayout.Space(-100);

                if (EditorTools.TexturedButton(_plusTexture, "Spawns a new cannon, destroying the old one.", 20f))
                    tankBuilder.SpawnCannon();

                if (tankBuilder.cannonBase != null)
                {
                    if (EditorTools.TexturedButton(_minusTexture, "Destroys the tanks current cannon.", 20f))
                        DestroyImmediate(tankBuilder.cannonBase.gameObject);
                }

                EditorTools.EndHorizontalGroup();

                EditorTools.DrawLine(0.5f, 5f, 5);

                if (tankBuilder.cannonBase == null) return;

                GUILayout.BeginVertical("box");
                {
                    if (EditorTools.Button("Toggle", "Toggles the current cannon."))
                        Selection.activeGameObject = tankBuilder.cannonBase.gameObject;



                    EditorTools.DrawLine(0.5f, 5f, 5);



                    EditorGUIUtility.labelWidth = 100;
                    tankBuilder.cannonType = (TankBuilder.CannonType) EditorGUILayout.EnumPopup(new GUIContent(
                        "Cannon Type",
                        "The method used to follow the target."), tankBuilder.cannonType);



                    EditorTools.DrawLine(0.5f, 5f, 5);

                    
                    
                    EditorTools.BeginHorizontalGroup();
                    {
                        EditorTools.Label("Base:", "Changes the values for the overall cannon rotor.", 100);

                        GUILayout.FlexibleSpace();

                        if (EditorTools.Button("Toggle", "Toggles the current cannon."))
                            Selection.activeGameObject = tankBuilder.cannonRotor.gameObject;
                    }
                    EditorTools.EndHorizontalGroup(3f);

                    tankBuilder.cannonCurrentTab = GUILayout.Toolbar(tankBuilder.cannonCurrentTab,
                        new string[] {"Position", "Size"});
                    
                    if (tankBuilder.cannonCurrentTab == 0)
                    {
                        EditorTools.Label("Position:", "Changes the position of the base.", 100);

                        EditorGUI.indentLevel++;

                        var position = tankBuilder.cannonRotorPosition;
                        position.x = EditorTools.Slider("X", "Changes the base's position along the x axis.",
                            position.x, -3.0f, 3.0f, 100);
                        position.y = EditorTools.Slider("Y", "Changes the base's position along the y axis.",
                            position.y, -7.0f, 5.0f, 100);
                        tankBuilder.cannonRotorPosition = position;

                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        EditorTools.Label("Local Scale:", "Changes the size of the overall cannon.", 100);

                        EditorGUI.indentLevel++;

                        var size = tankBuilder.cannonRotorSize;
                        size.x = EditorTools.Slider("X", "Changes the base's size along the x axis.",
                            size.x, -2.0f, 2.0f, 100);
                        size.y = EditorTools.Slider("Y", "Changes the base's size along the y axis.",
                            size.y, 0.0f, 2.0f, 100);
                        tankBuilder.cannonRotorSize = size;

                        EditorGUI.indentLevel--;
                    }

                    
                    
                    EditorTools.DrawLine(0.5f, 5f, 5);

                    
                    
                    if (tankBuilder.cannonType == TankBuilder.CannonType.Single)
                    {
                        EditorTools.BeginHorizontalGroup();
                        {
                            EditorTools.Label("Main Cannon:", "Changes the values for the main cannon.", 100);

                            GUILayout.FlexibleSpace();

                            if (EditorTools.Button("Toggle", "Toggles the current cannon."))
                                Selection.activeGameObject = tankBuilder.cannonHolder[0].gameObject;
                        }
                        EditorTools.EndHorizontalGroup(3f);

                        EditorGUI.indentLevel++;

                        tankBuilder.singleCannonPosition = EditorTools.Slider("Position",
                            "Position of the cannon on the x axis.",
                            tankBuilder.singleCannonPosition, -2.5f, 2.5f, 100.0f);

                        tankBuilder.singleCannonSize = EditorTools.Slider("Size", "Size of the tanks cannon.",
                            tankBuilder.singleCannonSize, 0.5f, 1.0f, 100.0f);

                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        EditorTools.BeginHorizontalGroup();
                        {
                            EditorTools.Label("Left Cannon:", "Changes the values for the left cannon.", 100);

                            GUILayout.FlexibleSpace();

                            if (EditorTools.Button("Toggle", "Toggles the current cannon."))
                                Selection.activeGameObject = tankBuilder.cannonHolder[0].gameObject;
                        }
                        EditorTools.EndHorizontalGroup(3f);

                        EditorGUI.indentLevel++;

                        tankBuilder.leftCannonPosition = EditorTools.Slider("Position",
                            "Position of the cannon on the x axis.",
                            tankBuilder.leftCannonPosition, -2.5f, 0.0f, 100.0f);

                        tankBuilder.leftCannonSize = EditorTools.Slider("Size", "Size of the tanks cannon.",
                            tankBuilder.leftCannonSize, 0.5f, 1.0f, 100.0f);

                        EditorGUI.indentLevel--;



                        EditorTools.DrawLine(0.5f, 5f, 5);



                        EditorTools.BeginHorizontalGroup();
                        {
                            EditorTools.Label("Right Cannon:", "Changes the values for the right cannon.", 100);

                            GUILayout.FlexibleSpace();

                            if (EditorTools.Button("Toggle", "Toggles the current cannon."))
                                Selection.activeGameObject = tankBuilder.cannonHolder[1].gameObject;
                        }
                        EditorTools.EndHorizontalGroup(3f);

                        EditorGUI.indentLevel++;

                        tankBuilder.rightCannonPosition = EditorTools.Slider("Position",
                            "Position of the cannon on the x axis.",
                            tankBuilder.rightCannonPosition, 0.0f, 2.5f, 100.0f);

                        tankBuilder.rightCannonSize = EditorTools.Slider("Size", "Size of the tanks cannon.",
                            tankBuilder.rightCannonSize, 0.5f, 1.0f, 100.0f);

                        EditorGUI.indentLevel--;



                        EditorTools.DrawLine(0.5f, 5f, 5);



                        EditorTools.Label("Colors:", "Changes the various colors on the tanks cannon.");
                        {
                            EditorGUI.indentLevel++;

                            EditorTools.BeginHorizontalGroup();
                            {
                                EditorTools.Label("Base", "Changes the color of the cannons base.", 100);

                                tankBuilder.cannonBaseColor = EditorGUILayout.ColorField(tankBuilder.cannonBaseColor);
                                tankBuilder.cannonBaseSidesColor =
                                    EditorGUILayout.ColorField(tankBuilder.cannonBaseSidesColor);

                                GUILayout.FlexibleSpace();
                            }
                            EditorTools.EndHorizontalGroup(5);

                            if (tankBuilder.cannonType == TankBuilder.CannonType.Single)
                            {
                                tankBuilder.cannonColor = EditorTools.ColorField("Cannon",
                                    "Changes the color of the tanks cannon.", tankBuilder.cannonColor, true, 120);
                            }
                            else
                            {
                                tankBuilder.leftCannonColor = EditorTools.ColorField("Left Cannon",
                                    "Changes the color of the tanks left cannon.", tankBuilder.leftCannonColor, true,
                                    120);
                                tankBuilder.rightCannonColor = EditorTools.ColorField("Right Cannon",
                                    "Changes the color of the tanks right cannon.", tankBuilder.rightCannonColor, true,
                                    120);
                            }

                            EditorGUI.indentLevel--;
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            else GUILayout.EndHorizontal();
        }



        private void DrawVents()
        {
            EditorTools.BeginHorizontalGroup();

            if (EditorTools.Foldout("Vents", "Spawn, toggles, or destroys the vents that are present on the tank.",
                ref tankBuilder.expandVents))
            {
                GUILayout.Space(-100);

                // if (EditorTools.TexturedButton(_plusTexture, "Creates a completely new vent.", 20f))
                //     tankBuilder.SpawnAccessory("Vents");

                EditorTools.EndHorizontalGroup();

                EditorTools.DrawLine(0.5f, 2.5f, 5);

                if (tankBuilder.hull == null) return;





                GUILayout.BeginVertical("box");
                {
                    var index = 0;
                    foreach (var vent in tankBuilder.vents)
                    {
                        if (index > 0) EditorTools.DrawLine(0.5f, 5f, 5);

                        GUILayout.BeginHorizontal();

                        if (EditorTools.Foldout(vent.ventName, "", ref vent.expand))
                        {
                            GUILayout.Space(-100);

                            if (EditorTools.TexturedButton(_editTexture, "Rename this object?", 20f))
                                vent.editName = !vent.editName;

                            if (EditorTools.TexturedButton(_plusTexture, "Create a copy of this object?", 20f))
                            {
                                tankBuilder.CopyVent(vent);
                                break;
                            }

                            if (EditorTools.TexturedButton(_minusTexture, "Remove this object?", 20f))
                            {
                                if (vent.vent != null) DestroyImmediate(vent.vent.gameObject);
                                tankBuilder.vents.RemoveAt(index);
                                break;
                            }

                            GUILayout.EndHorizontal();

                            
                            
                            EditorTools.DrawLine(0.5f, 2.5f, 2.5f);
                            
                            
                            
                            GUI.backgroundColor = Color.black;
                            GUILayout.BeginVertical("box");
                            GUI.backgroundColor = guiColorBackup;
                            
                            if (vent.editName)
                            {
                                GUILayout.BeginHorizontal();
                                GUI.backgroundColor = Color.grey;
                                vent.ventName = EditorGUILayout.TextField(new GUIContent("Name", "Automatically renames the game object."), vent.ventName);
                                GUI.backgroundColor = guiColorBackup;
                                if (EditorTools.Button("Done", "Finish renaming the box.")) vent.editName = false;
                                GUILayout.EndHorizontal();
                                
                                EditorTools.DrawLine(0.5f, 2.5f, 5);
                            }


                            if (vent.vent == null)
                            {
                                vent.vent = (Transform) EditorGUILayout.ObjectField(new GUIContent("Missing Transform:",
                                        vent.ventName + "'s transform was not found, please reassign it or delete this item."),
                                    vent.vent, typeof(Transform), true);
                                
                                GUILayout.EndVertical();

                                continue;
                            }

                            if (vent.ventSprite == null) vent.ventSprite = vent.vent.GetComponent<SpriteRenderer>();

                            if (EditorTools.Button("Toggle", "Toggles the current vent."))
                                Selection.activeGameObject = vent.vent.gameObject;







                            GUILayout.Space(5);



                            vent.currentTab = GUILayout.Toolbar(vent.currentTab,
                                new string[] {"Position", "Rotation", "Size"});

                            switch (vent.currentTab)
                            {
                                case 0:
                                {
                                    EditorTools.Label("Position:", "Changes the position of the vent.", 100);

                                    EditorGUI.indentLevel++;

                                    var position = vent.ventPosition;
                                    position.x = EditorTools.Slider("X", "Changes the vents position along the x axis.",
                                        position.x, -10, 10, 100);
                                    position.y = EditorTools.Slider("Y", "Changes the vents position along the y axis.",
                                        position.y, -13, 11.4f, 100);
                                    vent.ventPosition = position;

                                    EditorGUI.indentLevel--;
                                    break;
                                }
                                case 1:
                                {
                                    EditorTools.Label("Rotation:", "Changes the rotation of the vent.", 100);

                                    EditorGUI.indentLevel++;

                                    vent.ventRotation = EditorTools.Slider("Y",
                                        "Changes the vents rotation along the y axis.",
                                        vent.ventRotation, -180.0f, 180.0f, 100);

                                    EditorGUI.indentLevel--;

                                    break;
                                }
                                case 2:
                                {
                                    EditorTools.Label("Local Scale:", "Changes the scale of the vent.", 100);

                                    EditorGUI.indentLevel++;

                                    var scale = vent.ventScale;
                                    scale.x = EditorTools.Slider("X", "Changes the vents position along the x axis.",
                                        scale.x, 0.0f, 2.0f, 100);
                                    scale.y = EditorTools.Slider("Y", "Changes the vents position along the y axis.",
                                        scale.y, 0.0f, 2.0f, 100);
                                    vent.ventScale = scale;

                                    EditorGUI.indentLevel--;
                                    break;
                                }

                                default:
                                    break;
                            }

                            EditorTools.DrawLine(0.5f, 5f, 5);

                            vent.currentParent = EditorGUILayout.Popup(new GUIContent("Parent",
                                    "The current parent of the vent.\n\n" +
                                    "eg. if parent to the cannon, this vent will rotate with it."), vent.currentParent,
                                new string[] {"Hull", "Cannon"});
                            GUILayout.Space(2.5f);

                            vent.ventColor = EditorTools.ColorField("Color",
                                "Changes the color of the vent.", vent.ventColor, true, 100);

                            vent.sortingOrder = EditorTools.IntField("Sorting Order",
                                "Where this accessory standing within the chosen sorting layer.",
                                vent.sortingOrder, 100);

                            GUILayout.EndVertical();
                        }
                        else GUILayout.EndHorizontal();

                        index++;
                    }
                }
                GUILayout.EndVertical();
            }
            else EditorTools.EndHorizontalGroup();
        }
        
        
        
        private void DrawCompartments()
        {
            EditorTools.BeginHorizontalGroup();

            if (EditorTools.Foldout("Compartments", "Spawn, toggles, or destroys the compartments that are present on the tank.",
                ref tankBuilder.expandCompartments))
            {
                GUILayout.Space(-100);

                // if (EditorTools.TexturedButton(_plusTexture, "Creates a completely new compartment.", 20f))
                //     tankBuilder.SpawnAccessory("Compartments");

                EditorTools.EndHorizontalGroup();

                EditorTools.DrawLine(0.5f, 2.5f, 5);

                if (tankBuilder.hull == null) return;





                GUILayout.BeginVertical("box");
                {
                    var index = 0;
                    foreach (var compartment in tankBuilder.compartments)
                    {
                        if (index > 0) EditorTools.DrawLine(0.5f, 5f, 5);

                        GUILayout.BeginHorizontal();

                        if (EditorTools.Foldout(compartment.compartmentName, "", ref compartment.expand))
                        {
                            GUILayout.Space(-100);

                            if (EditorTools.TexturedButton(_editTexture, "Rename the compartment?", 20f))
                                compartment.editName = !compartment.editName;

                            if (EditorTools.TexturedButton(_plusTexture, "Create a copy of this compartment?", 20f))
                            {
                                tankBuilder.CopyCompartment(compartment);
                                break;
                            }

                            if (EditorTools.TexturedButton(_minusTexture, "Remove this compartment?", 20f))
                            {
                                if (compartment.compartment != null) DestroyImmediate(compartment.compartment.gameObject);
                                tankBuilder.compartments.RemoveAt(index);
                                break;
                            }

                            GUILayout.EndHorizontal();

                            
                            
                            EditorTools.DrawLine(0.5f, 2.5f, 2.5f);
                            
                            
                            
                            GUI.backgroundColor = Color.black;
                            GUILayout.BeginVertical("box");
                            GUI.backgroundColor = guiColorBackup;
                            
                            if (compartment.editName)
                            {
                                GUILayout.BeginHorizontal();
                                GUI.backgroundColor = Color.grey;
                                compartment.compartmentName = EditorGUILayout.TextField(new GUIContent("Name", 
                                    "Automatically renames the game object."), compartment.compartmentName);
                                GUI.backgroundColor = guiColorBackup;
                                if (EditorTools.Button("Done", "Finish renaming the box.")) compartment.editName = false;
                                GUILayout.EndHorizontal();
                                
                                EditorTools.DrawLine(0.5f, 2.5f, 5);
                            }

                            
                            
                            if (compartment.compartment == null)
                            {
                                compartment.compartment = (Transform) EditorGUILayout.ObjectField(new GUIContent(
                                        "Missing Transform:", compartment.compartmentName +
                                        "'s transform was not found, please reassign it or delete this item."),
                                    compartment.compartment, typeof(Transform), true);
                                
                                GUILayout.EndVertical();

                                continue;
                            }

                            if (compartment.compartmentSprite == null) compartment.compartmentSprite = compartment.compartment.GetComponent<SpriteRenderer>();

                            if (EditorTools.Button("Toggle", "Toggles the current compartment."))
                                Selection.activeGameObject = compartment.compartment.gameObject;

                            
                            
                            EditorTools.DrawLine(0.5f, 2.5f, 2.5f);

                            

                            EditorGUIUtility.labelWidth = 100;
                            compartment.compartmentStyle = EditorGUILayout.Popup(new GUIContent("Style",
                                "Changes the style of the compartment."), compartment.compartmentStyle, compartmentStyles);
                            
                            
                            
                            EditorTools.DrawLine(0.5f, 2.5f, 5);

                            

                            compartment.currentTab = GUILayout.Toolbar(compartment.currentTab,
                                new string[] {"Position", "Rotation", "Size"});

                            switch (compartment.currentTab)
                            {
                                case 0:
                                {
                                    EditorTools.Label("Position:", "Changes the position of the compartment.", 100);

                                    EditorGUI.indentLevel++;

                                    var position = compartment.compartmentPosition;
                                    position.x = EditorTools.Slider("X", "Changes the compartments position along the x axis.",
                                        position.x, -10, 10, 100);
                                    position.y = EditorTools.Slider("Y", "Changes the compartments position along the y axis.",
                                        position.y, -13, 11.4f, 100);
                                    compartment.compartmentPosition = position;

                                    EditorGUI.indentLevel--;
                                    break;
                                }
                                case 1:
                                {
                                    EditorTools.Label("Rotation:", "Changes the rotation of the compartment.", 100);

                                    EditorGUI.indentLevel++;

                                    compartment.compartmentRotation = EditorTools.Slider("Y",
                                        "Changes the compartments rotation along the y axis.",
                                        compartment.compartmentRotation, -180.0f, 180.0f, 100);

                                    EditorGUI.indentLevel--;

                                    break;
                                }
                                case 2:
                                {
                                    EditorTools.Label("Local Scale:", "Changes the scale of the compartment.", 100);

                                    EditorGUI.indentLevel++;

                                    var scale = compartment.compartmentScale;
                                    scale.x = EditorTools.Slider("X", "Changes the compartments position along the x axis.",
                                        scale.x, -2.0f, 2.0f, 100);
                                    scale.y = EditorTools.Slider("Y", "Changes the compartments position along the y axis.",
                                        scale.y, -2.0f, 2.0f, 100);
                                    compartment.compartmentScale = scale;

                                    EditorGUI.indentLevel--;
                                    break;
                                }

                                default:
                                    break;
                            }

                            EditorTools.DrawLine(0.5f, 5f, 5);

                            compartment.currentParent = EditorGUILayout.Popup(new GUIContent("Parent",
                                    "The current parent of the compartment.\n\n" +
                                    "eg. if parent to the cannon, this compartment will rotate with it."), compartment.currentParent,
                                new string[] {"Hull", "Cannon"});
                            
                            GUILayout.Space(2.5f);
                            
                            compartment.compartmentColor = EditorTools.ColorField("Color",
                                "Changes the color of the compartment.", compartment.compartmentColor, true, 100);

                            compartment.sortingOrder = EditorTools.IntField("Sorting Order",
                                "Where this accessory standing within the chosen sorting layer.",
                                compartment.sortingOrder, 100);

                            GUILayout.EndVertical();
                        }
                        else GUILayout.EndHorizontal();

                        index++;
                    }
                }
                GUILayout.EndVertical();
            }
            else EditorTools.EndHorizontalGroup();
        }
        
        
        
        private void DrawBoxes()
        {
            EditorTools.BeginHorizontalGroup();

            if (EditorTools.Foldout("Boxes", "Spawn, toggles, or destroys the boxes that are present on the tank.",
                ref tankBuilder.expandBoxes))
            {
                GUILayout.Space(-100);

                // if (EditorTools.TexturedButton(_plusTexture, "Creates a completely new box.", 20f))
                //     tankBuilder.SpawnAccessory("Boxes");

                EditorTools.EndHorizontalGroup();

                EditorTools.DrawLine(0.5f, 2.5f, 5);

                if (tankBuilder.hull == null) return;





                GUILayout.BeginVertical("box");
                {
                    var index = 0;
                    foreach (var box in tankBuilder.boxes)
                    {
                        if (index > 0) EditorTools.DrawLine(0.5f, 5f, 5);

                        GUILayout.BeginHorizontal();

                        if (EditorTools.Foldout(box.boxName, "", ref box.expand))
                        {
                            GUILayout.Space(-100);

                            if (EditorTools.TexturedButton(_editTexture, "Rename object?", 20f))
                                box.editName = !box.editName;

                            if (EditorTools.TexturedButton(_plusTexture, "Create a copy of this object?.", 20f))
                            {
                                tankBuilder.CopyBox(box);
                                break;
                            }

                            if (EditorTools.TexturedButton(_minusTexture, "Remove this object?", 20f))
                            {
                                if (box.box != null) DestroyImmediate(box.box.gameObject);
                                tankBuilder.boxes.RemoveAt(index);
                                break;
                            }

                            GUILayout.EndHorizontal();

                            
                            
                            EditorTools.DrawLine(0.5f, 2.5f, 2.5f);
                            
                            
                            
                            GUI.backgroundColor = Color.black;
                            GUILayout.BeginVertical("box");
                            GUI.backgroundColor = guiColorBackup;
                            
                            if (box.editName)
                            {
                                GUILayout.BeginHorizontal();
                                GUI.backgroundColor = Color.grey;
                                box.boxName = EditorGUILayout.TextField(new GUIContent("Name", "Automatically renames the game object."), box.boxName);
                                GUI.backgroundColor = guiColorBackup;
                                if (EditorTools.Button("Done", "Finish renaming the box.")) box.editName = false;
                                GUILayout.EndHorizontal();
                                
                                EditorTools.DrawLine(0.5f, 2.5f, 5);
                            }


                            if (box.box == null)
                            {
                                box.box = (Transform) EditorGUILayout.ObjectField(new GUIContent("Missing Transform:",
                                        box.boxName + "'s transform was not found, please reassign it or delete this item."),
                                    box.box, typeof(Transform), true);
                                
                                GUILayout.EndVertical();

                                continue;
                            }

                            if (box.boxSprite == null) box.boxSprite = box.box.GetComponent<SpriteRenderer>();

                            if (EditorTools.Button("Toggle", "Toggles the current box."))
                                Selection.activeGameObject = box.box.gameObject;

                            
                            
                            EditorTools.DrawLine(0.5f, 2.5f, 2.5f);


                            
                            EditorGUIUtility.labelWidth = 100;
                            box.boxStyle = EditorGUILayout.Popup(new GUIContent("Style",
                                "Changes the style of the box."), box.boxStyle, boxStyles);

                            
                            
                            EditorTools.DrawLine(0.5f, 2.5f, 5);

                            

                            box.currentTab = GUILayout.Toolbar(box.currentTab,
                                new string[] {"Position", "Rotation", "Size"});

                            switch (box.currentTab)
                            {
                                case 0:
                                {
                                    EditorTools.Label("Position:", "Changes the position of the box.", 100);

                                    EditorGUI.indentLevel++;

                                    var position = box.boxPosition;
                                    position.x = EditorTools.Slider("X", "Changes the boxes position along the x axis.",
                                        position.x, -10, 10, 100);
                                    position.y = EditorTools.Slider("Y", "Changes the boxes position along the y axis.",
                                        position.y, -13, 11.4f, 100);
                                    box.boxPosition = position;

                                    EditorGUI.indentLevel--;
                                    break;
                                }
                                case 1:
                                {
                                    EditorTools.Label("Rotation:", "Changes the rotation of the box.", 100);

                                    EditorGUI.indentLevel++;

                                    box.boxRotation = EditorTools.Slider("Y",
                                        "Changes the boxes rotation along the y axis.",
                                        box.boxRotation, -180.0f, 180.0f, 100);

                                    EditorGUI.indentLevel--;

                                    break;
                                }
                                case 2:
                                {
                                    EditorTools.Label("Local Scale:", "Changes the scale of the box.", 100);

                                    EditorGUI.indentLevel++;

                                    var scale = box.boxScale;
                                    scale.x = EditorTools.Slider("X", "Changes the boxes position along the x axis.",
                                        scale.x, -2.0f, 2.0f, 100);
                                    scale.y = EditorTools.Slider("Y", "Changes the boxes position along the y axis.",
                                        scale.y, -2.0f, 2.0f, 100);
                                    box.boxScale = scale;

                                    EditorGUI.indentLevel--;
                                    break;
                                }

                                default:
                                    break;
                            }

                            EditorTools.DrawLine(0.5f, 5f, 5);

                            box.currentParent = EditorGUILayout.Popup(new GUIContent("Parent",
                                    "The current parent of the box.\n\n" +
                                    "eg. if parent to the cannon, this box will rotate with it."), box.currentParent,
                                new string[] {"Hull", "Cannon"});
                            
                            GUILayout.Space(2.5f);
                            
                            box.boxColor = EditorTools.ColorField("Color",
                                "Changes the color of the box.", box.boxColor, true, 100);

                            box.sortingOrder = EditorTools.IntField("Sorting Order",
                                "Where this accessory standing within the chosen sorting layer.",
                                box.sortingOrder, 100);
                            
                            GUILayout.EndVertical();

                        }
                        else GUILayout.EndHorizontal();

                        index++;
                    }
                }
                GUILayout.EndVertical();
            }
            else EditorTools.EndHorizontalGroup();
        }







        private void DrawCategories()
        {
            var index = 0;
            foreach (var category in tankBuilder.categories)
            {
                GUILayout.BeginHorizontal();

                var expandCategory = category.expandCategory;

                if (EditorTools.Foldout(category.categoryName,
                    "Spawn, toggles, or destroys the accessories present within this category.",
                    ref expandCategory))
                {
                    GUILayout.Space(-100);

                    if (EditorTools.TexturedButton(_plusTexture, "Creates a completely new box.", 20f))
                        tankBuilder.SpawnAccessory(index, category.categoryName, category.categoryFolder);

                    if (EditorTools.TexturedButton(_minusTexture, "Removes this category.", 20f))
                    {
                        tankBuilder.RemoveCategory(index);
                        break;
                    }

                    EditorTools.EndHorizontalGroup();

                    EditorTools.DrawLine(0.5f, 2.5f, 5);

                    if (tankBuilder.hull == null) return;
                    
                    InitAccessoryStyle(category.categoryFolder, ref accessoryStyles);
                    
                    DrawAccessories(index, ref category.accessories);
                }
                else GUILayout.EndHorizontal();

                EditorTools.DrawLine(0.5f, 2.5f, 5);
                
                category.expandCategory = expandCategory;
                index++;
            }            
        }
        
        private void DrawAccessories(int index, ref List<TankBuilder.Accessory> accessories)
        {
            GUILayout.BeginVertical("box");
            {
                var i = 0;
                foreach (var accessory in accessories)
                {
                    if (i > 0) EditorTools.DrawLine(0.5f, 5f, 5);

                    GUILayout.BeginHorizontal();

                    if (EditorTools.Foldout(accessory.Name, "", ref accessory.expand))
                    {
                        GUILayout.Space(-100);

                        if (EditorTools.TexturedButton(_editTexture, "Rename object?", 20f))
                            accessory.editName = !accessory.editName;

                        if (EditorTools.TexturedButton(_plusTexture, "Create a copy of this object?.", 20f))
                        {
                            tankBuilder.CopyAccessory(index, accessory);
                            break;
                        }

                        if (EditorTools.TexturedButton(_minusTexture, "Remove this object?", 20f))
                        {
                            if (accessory.transform != null) DestroyImmediate(accessory.transform.gameObject);
                            accessories.RemoveAt(i);
                            break;
                        }

                        GUILayout.EndHorizontal();

                        
                        
                        EditorTools.DrawLine(0.5f, 2.5f, 2.5f);
                        
                        
                        
                        GUI.backgroundColor = Color.black;
                        GUILayout.BeginVertical("box");
                        GUI.backgroundColor = guiColorBackup;
                        
                        if (accessory.editName)
                        {
                            GUILayout.BeginHorizontal();
                            GUI.backgroundColor = Color.grey;
                            EditorGUIUtility.labelWidth = 50;
                            accessory.Name = EditorGUILayout.TextField(new GUIContent("Name", "Automatically renames the game object."), accessory.Name);
                            GUI.backgroundColor = guiColorBackup;
                            if (EditorTools.Button("Done", "Finish renaming the accessory."))
                            {
                                accessory.editName = false;
                                GUI.FocusControl(null);
                            }
                            GUILayout.EndHorizontal();
                            
                            EditorTools.DrawLine(0.5f, 2.5f, 5);
                        }


                        if (accessory.transform == null)
                        {
                            accessory.transform = (Transform) EditorGUILayout.ObjectField(new GUIContent("Missing Transform:",
                                    accessory.Name + "'s transform was not found, please reassign it or delete this item."),
                                accessory.transform, typeof(Transform), true);
                            
                            GUILayout.EndVertical();

                            continue;
                        }

                        if (accessory.accessorySprite == null) accessory.accessorySprite = accessory.transform.GetComponent<SpriteRenderer>();

                        if (EditorTools.Button("Toggle", "Toggles the current accessory."))
                            Selection.activeGameObject = accessory.transform.gameObject;

                        
                        
                        EditorTools.DrawLine(0.5f, 2.5f, 2.5f);


                        
                        EditorGUIUtility.labelWidth = 100;
                        accessory.Style = EditorGUILayout.Popup(new GUIContent("Style",
                            "Changes the style of the accessory."), accessory.Style, accessoryStyles);

                        
                        
                        EditorTools.DrawLine(0.5f, 2.5f, 5);



                        var currentTab = accessory.currentTab;
                        accessory.currentTab = GUILayout.Toolbar(accessory.currentTab,
                            new string[] {"Position", "Rotation", "Size"});
                        
                        if(accessory.currentTab != currentTab) GUI.FocusControl(null);

                        switch (accessory.currentTab)
                        {
                            case 0:
                            {
                                EditorTools.Label("Position:", "Changes the position of the accessory.", 100);

                                EditorGUI.indentLevel++;

                                var position = accessory.Position;
                                position.x = EditorTools.Slider("X", "Changes the accessories position along the x axis.",
                                    position.x, -10, 10, 100);
                                position.y = EditorTools.Slider("Y", "Changes the accessories position along the y axis.",
                                    position.y, -13, 11.4f, 100);
                                accessory.Position = position;

                                EditorGUI.indentLevel--;
                                break;
                            }
                            case 1:
                            {
                                EditorTools.Label("Rotation:", "Changes the rotation of the accessory.", 100);

                                EditorGUI.indentLevel++;

                                accessory.Rotation = EditorTools.Slider("Y",
                                    "Changes the accessories rotation along the y axis.",
                                    accessory.Rotation, -180.0f, 180.0f, 100);

                                EditorGUI.indentLevel--;

                                break;
                            }
                            case 2:
                            {
                                EditorTools.Label("Local Scale:", "Changes the scale of the accessory.", 100);

                                EditorGUI.indentLevel++;

                                var scale = accessory.Scale;
                                scale.x = EditorTools.Slider("X", "Changes the accessories position along the x axis.",
                                    scale.x, -2.0f, 2.0f, 100);
                                scale.y = EditorTools.Slider("Y", "Changes the accessories position along the y axis.",
                                    scale.y, -2.0f, 2.0f, 100);
                                accessory.Scale = scale;

                                EditorGUI.indentLevel--;
                                break;
                            }

                            default:
                                break;
                        }

                        EditorTools.DrawLine(0.5f, 5f, 5);

                        accessory.CurrentParent = EditorGUILayout.Popup(new GUIContent("Parent",
                                "The current parent of the accessory.\n\n" +
                                "eg. if parent to the cannon, this accessory will rotate with it."), accessory.CurrentParent,
                            new string[] {"Hull", "Cannon"});
                        
                        GUILayout.Space(2.5f);
                        
                        accessory.Color = EditorTools.ColorField("Color",
                            "Changes the color of the accessory.", accessory.Color, true, 100);

                        accessory.SortingOrder = EditorTools.IntField("Sorting Order",
                            "Where this accessory standing within the chosen sorting layer.",
                            accessory.SortingOrder, 100);
                        
                        GUILayout.EndVertical();

                    }
                    else GUILayout.EndHorizontal();

                    i++;
                }
            }
            GUILayout.EndVertical();
        }
    }
}
