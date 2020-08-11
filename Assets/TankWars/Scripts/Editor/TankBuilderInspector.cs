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
        private SerializedProperty _sortingLayerId;

        private bool _eraseAll;
        private bool _addCategory;

        private string _categoryName;
        private string _folderLocation;
        private string _newCategoryName;
        private string _newFolderLocation;
        
        private Color _guiColorBackup;
        
        private const string Path = "TankWars/Sprites/";
        private string[] _accessoryStyles;
        
        private Texture2D _plusTexture;
        private Texture2D _minusTexture;
        private Texture2D _editTexture;
        
        private TankBuilder tankBuilder => target as TankBuilder;

        private void OnEnable()
        {
            // Save the original background color.
            _guiColorBackup = GUI.backgroundColor;
            
            // Setup the SerializedProperties
            _sortingLayerId = serializedObject.FindProperty("sortingLayerID");
            
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
            
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(target, "TankBuilder");
            
            DrawSpawnElements();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            
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

            EditorTools.SortingLayerField("", "", _sortingLayerId, EditorStyles.popup, EditorStyles.label);
            EditorGUILayout.EndHorizontal();

            EditorTools.DrawLine(0.5f, 10f, 5);
            
            GUI.backgroundColor = Color.grey;

            if (!_addCategory)
            {
                if (!_eraseAll)
                {
                    if (EditorTools.Button("Erase All", "Erases everything without spawning a new tank."))
                        _eraseAll = true;
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
                    
                    if (EditorTools.Button("No", "")) _eraseAll = false;
                    
                    GUILayout.EndHorizontal(); 
                    GUILayout.Space(15);
                    
                    GUILayout.EndVertical(); 
                }
                
                GUI.backgroundColor = _guiColorBackup;
                
                GUILayout.Space(5);
            }

            if (!_eraseAll)
            {
                if (!_addCategory)
                {
                    if (EditorTools.Button("Add Category", "Add a new category."))
                        _addCategory = true;
                }
                else if(_addCategory)
                {
                    GUI.backgroundColor = Color.grey;
                    GUILayout.BeginVertical("box");

                    EditorTools.Label("Add Category:", "", 100);
                    
                    EditorGUIUtility.labelWidth = 100;
                    _categoryName = EditorGUILayout.TextField(new GUIContent("Name:", "Name of the category to add."),
                        _categoryName);
                    
                    EditorGUIUtility.labelWidth = 100;
                    _folderLocation = EditorGUILayout.TextField(
                        new GUIContent("Folder Location:", "Location of the folder in resources."),
                        _folderLocation);
                    
                    GUILayout.Space(5);
                    
                    GUILayout.BeginHorizontal();
                    if (EditorTools.Button("Add", "Adds the category."))
                    {
                        tankBuilder.AddCategory(_categoryName, _folderLocation);
                        _addCategory = false;
                        _categoryName = "";
                        _folderLocation = "";
                        GUI.FocusControl(null);
                    }

                    if (EditorTools.Button("Back", "Goes back to main."))
                    {
                        _addCategory = false;
                        _categoryName = "";
                        _folderLocation = "";
                        GUI.FocusControl(null);
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.EndVertical(); 
                }
                
                GUILayout.Space(5);
            }

            if (_eraseAll || _addCategory) return;
            
            if (EditorTools.Button("Add Movement System", "Adds and sets up all the necessary " +
                                                          "components for the tank to move."))
                tankBuilder.AddMovementSystem();
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
            GUILayout.BeginHorizontal();

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

                GUILayout.EndHorizontal();

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

                    
                    
                    GUILayout.BeginHorizontal();
                    {
                        EditorTools.Label("Base:", "Changes the values for the overall cannon rotor.", 100);

                        GUILayout.FlexibleSpace();

                        if (EditorTools.Button("Toggle", "Toggles the current cannon."))
                            Selection.activeGameObject = tankBuilder.cannonRotor.gameObject;
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(3);

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
                        GUILayout.BeginHorizontal();
                        {
                            EditorTools.Label("Main Cannon:", "Changes the values for the main cannon.", 100);

                            GUILayout.FlexibleSpace();

                            if (EditorTools.Button("Toggle", "Toggles the current cannon."))
                                Selection.activeGameObject = tankBuilder.cannonHolder[0].gameObject;
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.Space(3);

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
                        GUILayout.BeginHorizontal();
                        {
                            EditorTools.Label("Left Cannon:", "Changes the values for the left cannon.", 100);

                            GUILayout.FlexibleSpace();

                            if (EditorTools.Button("Toggle", "Toggles the current cannon."))
                                Selection.activeGameObject = tankBuilder.cannonHolder[0].gameObject;
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.Space(3);

                        EditorGUI.indentLevel++;

                        tankBuilder.leftCannonPosition = EditorTools.Slider("Position",
                            "Position of the cannon on the x axis.",
                            tankBuilder.leftCannonPosition, -2.5f, 0.0f, 100.0f);

                        tankBuilder.leftCannonSize = EditorTools.Slider("Size", "Size of the tanks cannon.",
                            tankBuilder.leftCannonSize, 0.5f, 1.0f, 100.0f);

                        EditorGUI.indentLevel--;

                        EditorTools.DrawLine(0.5f, 5f, 5);

                        GUILayout.BeginHorizontal();
                        {
                            EditorTools.Label("Right Cannon:", "Changes the values for the right cannon.", 100);

                            GUILayout.FlexibleSpace();

                            if (EditorTools.Button("Toggle", "Toggles the current cannon."))
                                Selection.activeGameObject = tankBuilder.cannonHolder[1].gameObject;
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.Space(3);

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

                        GUILayout.BeginHorizontal();
                        {
                            EditorTools.Label("Base", "Changes the color of the cannons base.", 100);

                            tankBuilder.cannonBaseColor = EditorGUILayout.ColorField(tankBuilder.cannonBaseColor);
                            tankBuilder.cannonBaseSidesColor =
                                EditorGUILayout.ColorField(tankBuilder.cannonBaseSidesColor);

                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.Space(5);

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
                            _newCategoryName = category.categoryName;
                            _newFolderLocation = category.categoryFolder;   
                        }
                    }

                    if (EditorTools.TexturedButton(_plusTexture, "Creates a completely new accessory.", 20f))
                        tankBuilder.SpawnAccessory(index, category.categoryName, category.categoryFolder);

                    if (EditorTools.TexturedButton(_minusTexture, "Removes this category.", 20f))
                    {
                        tankBuilder.RemoveCategory(index);
                        break;
                    }

                    GUILayout.EndHorizontal();

                    EditorTools.DrawLine(0.5f, 2.5f, 5);
                    
                    if (category.editCategory)
                    {
                        GUI.backgroundColor = Color.grey;
                        GUILayout.BeginVertical("box");
                        {
                            EditorTools.Label("Edit Category:", "", 100);

                            EditorGUIUtility.labelWidth = 100;
                            _newCategoryName = EditorGUILayout.TextField(new GUIContent("Name:", 
                            "Name of the category."), _newCategoryName);
                            
                            _newFolderLocation = EditorGUILayout.TextField(new GUIContent("Folder Location:", 
                            "Location of the folder in resources."), _newFolderLocation);
                        
                            GUILayout.Space(5);
                        
                            GUILayout.BeginHorizontal();
                            {
                                if (EditorTools.Button("Apply", "Applies the changes."))
                                {
                                    foreach (var accessory in category.accessories)
                                        accessory.parentName = _newCategoryName;
                                    
                                    var transform = tankBuilder.transform;
                                        
                                    for (var i = 0; i < 2; i++)
                                    {
                                        var parent = transform.Find(i == 0 ? "Hull" : "Cannon");
                                        if (parent == null) continue;
                        
                                        var categoryTransform = parent.Find(category.categoryName);
                                        if(categoryTransform == null) continue;
                                            
                                        categoryTransform.name = _newCategoryName;
                                    }
                                    
                                    category.categoryName = _newCategoryName;
                                    category.categoryFolder = _newFolderLocation;
                                    
                                    category.editCategory = false;
                                    _newCategoryName = "";
                                    _newFolderLocation = "";
                                    GUI.FocusControl(null);
                                    
                                    // Debug.Log("Tank Builder: New category name and folder has been applied.");
                                }

                                if (EditorTools.Button("Back", "Goes back to main."))
                                {
                                    category.editCategory = false;
                                    _newCategoryName = "";
                                    _newFolderLocation = "";
                                    GUI.FocusControl(null);
                                }
                            }
                            GUILayout.EndHorizontal();
                    
                        }
                        GUILayout.EndVertical(); 
                        
                        EditorTools.DrawLine(0.5f, 2.5f, 5);
                    }

                    if (tankBuilder.hull == null) return;
                    
                    InitAccessoryStyle(category.categoryFolder, ref _accessoryStyles);
                    
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
                        GUI.backgroundColor = _guiColorBackup;
                        
                        if (accessory.editName)
                        {
                            GUI.backgroundColor = Color.grey;
                            
                            EditorGUIUtility.labelWidth = 50;
                            accessory.newName = EditorGUILayout.TextField(new GUIContent("Name", 
                                "Automatically renames the game object."), accessory.newName);
                            
                            GUI.backgroundColor = _guiColorBackup;
                                
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
                            "Changes the style of the accessory."), accessory.Style, _accessoryStyles);

                        
                        
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
