using System.Collections.Generic;
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
        private string newCategoryName;
        private string newFolderLocation;
        
        private Color guiColorBackup;
        
        private const string Path = "TankWars/Sprites/";
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
            var sprites = Resources.LoadAll<Sprite>(Path + folderName);
            styles = new string[sprites.Length];
            for (var i = 0; i < sprites.Length; i++)
                styles[i] = sprites[i].name;
        }
        
        public override void OnInspectorGUI()
        {
            EditorTools.InitStyles(out _boxStyle);
            
            DrawSpawnElements();
                
            Undo.RecordObject(target, "tankBuilder");
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            
            EditorGUIUtility.labelWidth = 150;
            
            // base.OnInspectorGUI();
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
                    }

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
                GUILayout.EndVertical();
            }
            else GUILayout.EndHorizontal();
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

                    if (!category.editCategory)
                    {
                        if (EditorTools.TexturedButton(_editTexture, "Edit Category?", 20f))
                        {
                            category.editCategory = true;
                            newCategoryName = category.categoryName;
                            newFolderLocation = category.categoryFolder;   
                        }
                    }

                    if (EditorTools.TexturedButton(_plusTexture, "Creates a completely new accessory.", 20f))
                        tankBuilder.SpawnAccessory(index, category.categoryName, category.categoryFolder);

                    if (EditorTools.TexturedButton(_minusTexture, "Removes this category.", 20f))
                    {
                        tankBuilder.RemoveCategory(index);
                        break;
                    }

                    EditorTools.EndHorizontalGroup();

                    EditorTools.DrawLine(0.5f, 2.5f, 5);
                    
                    if (category.editCategory)
                    {
                        GUI.backgroundColor = Color.grey;
                        GUILayout.BeginVertical("box");
                        {
                            EditorTools.Label("Edit Category:", "", 100);

                            EditorGUIUtility.labelWidth = 100;
                            newCategoryName = EditorGUILayout.TextField(new GUIContent("Name:", 
                            "Name of the category."), newCategoryName);
                            
                            newFolderLocation = EditorGUILayout.TextField(new GUIContent("Folder Location:", 
                            "Location of the folder in resources."), newFolderLocation);
                        
                            GUILayout.Space(5);
                        
                            GUILayout.BeginHorizontal();
                            {
                                if (EditorTools.Button("Apply", "Applies the changes."))
                                {
                                    foreach (var accessory in category.accessories)
                                        accessory.parentName = newCategoryName;
                                    
                                    var transform = tankBuilder.transform;
                                        
                                    for (var i = 0; i < 2; i++)
                                    {
                                        var parent = transform.Find(i == 0 ? "Hull" : "Cannon");
                                        if (parent == null) continue;
                        
                                        var categoryTransform = parent.Find(category.categoryName);
                                        if(categoryTransform == null) continue;
                                            
                                        categoryTransform.name = newCategoryName;
                                    }
                                    
                                    category.categoryName = newCategoryName;
                                    category.categoryFolder = newFolderLocation;
                                    
                                    category.editCategory = false;
                                    newCategoryName = "";
                                    newFolderLocation = "";
                                    GUI.FocusControl(null);
                                    
                                    // Debug.Log("Tank Builder: New category name and folder has been applied.");
                                }

                                if (EditorTools.Button("Back", "Goes back to main."))
                                {
                                    category.editCategory = false;
                                    newCategoryName = "";
                                    newFolderLocation = "";
                                    GUI.FocusControl(null);
                                }
                            }
                            GUILayout.EndHorizontal();
                    
                        }
                        GUILayout.EndVertical(); 
                        
                        EditorTools.DrawLine(0.5f, 2.5f, 5);
                    }

                    if (tankBuilder.hull == null) return;
                    
                    InitAccessoryStyle(category.categoryFolder, ref accessoryStyles);
                    
                    DrawAccessories(index, ref category.accessories);
                }
                else GUILayout.EndHorizontal();

                if(index != tankBuilder.categories.Count - 1) EditorTools.DrawLine(0.5f, 2.5f, 5);
                else GUILayout.Space(2.5f);
                
                category.expandCategory = expandCategory;
                index++;
            }            
        }
        
        
        
        private void DrawAccessories(int index, ref List<TankBuilder.Accessory> accessories)
        {
            if (accessories.Count == 0) return;
            
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

                        if (!accessory.editName)
                        {
                            if (EditorTools.TexturedButton(_editTexture, "Rename object?", 20f))
                            {
                                accessory.editName = true;
                                accessory.newName = accessory.Name;
                            }
                        }

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
                            GUI.backgroundColor = Color.grey;
                            
                            EditorGUIUtility.labelWidth = 50;
                            accessory.newName = EditorGUILayout.TextField(new GUIContent("Name", 
                                "Automatically renames the game object."), accessory.newName);
                            
                            GUI.backgroundColor = guiColorBackup;
                                
                            GUILayout.BeginHorizontal();
                            {
                                if (EditorTools.Button("Apply", "Finish renaming the accessory."))
                                {
                                    accessory.Name = accessory.newName;
                                    accessory.editName = false;
                                    GUI.FocusControl(null);
                                    
                                    // Debug.Log("Tank Builder: New accessory name has been applied.");
                                }
                                
                                if (EditorTools.Button("Back", "Goes back to main."))
                                {
                                    accessory.editName = false;
                                    GUI.FocusControl(null);
                                }
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
                                    position.x, -10.0f, 10.0f, 100);
                                position.y = EditorTools.Slider("Y", "Changes the accessories position along the y axis.",
                                    position.y, -13, 13.0f, 100);
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
