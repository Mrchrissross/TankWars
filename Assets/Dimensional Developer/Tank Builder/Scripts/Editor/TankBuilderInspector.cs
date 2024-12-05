using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DimensionalDeveloper.TankBuilder.Editor
{
    [CustomEditor(typeof(Utility.TankBuilder)), CanEditMultipleObjects]
    public class TankBuilderInspector : EditorTemplate
    {
        protected override string ScriptName => "Tank Builder";
        protected override bool EnableBaseGUI => false;
        private Utility.TankBuilder TankBuilder => target as Utility.TankBuilder;
        
        private SerializedProperty _sortingLayerId;

        private bool _eraseAll;
        private bool _addCategory;

        private string _categoryName;
        private string _folderLocation;
        private string _newCategoryName;
        private string _newFolderLocation;
        
        private string[] _accessoryStyles;
        
        private HashSet<string> _resourceCategories;
        private string[] _resourceCategoriesArray;
        private int _resourceCategoriesIndex;
        private int _editResourceCategoriesIndex;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _sortingLayerId = serializedObject.FindProperty("sortingLayerID");
            
            var dir = new DirectoryInfo("Assets/Dimensional Developer/Resources/Tank Builder/Sprites");
            var info = dir.GetFiles();
            info.Select(f => f.FullName).ToArray();
            _resourceCategories = new HashSet<string>();

            foreach (var f in info)
            {
                var fileName = f.Name;
                
                var charIndex = fileName.IndexOf(".", StringComparison.Ordinal);
                if (charIndex > 0) fileName = fileName[..charIndex];

                _resourceCategories.Add(fileName);
            }
            
            _resourceCategoriesArray = new string[_resourceCategories.Count];

            var index = 0;
            foreach (var category in _resourceCategories)
            {
                _resourceCategoriesArray[index] = category;
                index++;
            }        
        }
        
        
        
        protected override void DrawSections()
        {
            base.DrawSections();
            
            if (DrawHeader(this, 0, "Tank Creation Tool"))
                return;
            
            EditorGUILayout.BeginVertical(_boxStyle, GUILayout.MinWidth(BoxMinWidth), GUILayout.MaxWidth(BoxMaxWidth));
            {
                DrawTop();
                
                DrawLine(0.5f, 5f, 5);

                DrawSpawnHull();

                DrawLine(0.5f, 2.5f, 5);

                DrawSpawnCannon();
                
                DrawLine(0.5f, 2.5f, 5);

                DrawCategories();
                
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }
        
        
        
        private void DrawTop()
        {
            EditorGUILayout.BeginHorizontal();
            Label("Sorting Layer:", "The sorting layer to assign to the tank.", 110);
            SortingLayerField("", "", _sortingLayerId, EditorStyles.popup, EditorStyles.label);

            if(Button("Set", "Sets the sorting layer to all sprite renderers among children.", 60))
                TankBuilder.SetSortingLayer();
            
            EditorGUILayout.EndHorizontal();

            DrawLine(0.5f, 8f, 5);
            
            GUI.backgroundColor = Color.grey;

            if (!_addCategory)
            {
                if (!_eraseAll)
                {
                    if (Button("Erase All", "Erases everything without spawning a new tank."))
                        _eraseAll = true;
                }
                else
                {
                    GUI.backgroundColor = Color.grey;
                    GUILayout.BeginVertical("box");
                    GUI.backgroundColor = new Color(1f, 0f, 0f, 1f);

                    
                    GUILayout.Space(5);
                    Label("Erase All:", "", 100);
                    GUILayout.BeginHorizontal();
                    Label("Are you sure?", "", 100);
                    if (Button("Yes", "Erases everything."))
                    {
                        TankBuilder.EraseAll();
                        _eraseAll = false;
                    }
                    
                    if (Button("No", "")) _eraseAll = false;
                    
                    GUILayout.EndHorizontal(); 
                    GUILayout.Space(15);
                    
                    GUILayout.EndVertical(); 
                }
                
                GUI.backgroundColor = guiColorBackup;
            }

            if (!_eraseAll)
            {
                if (!_addCategory)
                {
                    if (Button("Add Category", "Add a new category."))
                    {
                        _addCategory = true;
                        _resourceCategoriesIndex = 0;
                    }
                }
                else if(_addCategory)
                {
                    GUI.backgroundColor = Color.grey;
                    GUILayout.BeginVertical("box");

                    Label("Add Category:", "", 100);
                    
                    EditorGUIUtility.labelWidth = 100;
                    _categoryName = EditorGUILayout.TextField(new GUIContent("Name:", "Name of the category to add."),
                        _categoryName);
                    
                    EditorGUIUtility.labelWidth = 100;
                    _resourceCategoriesIndex = EditorGUILayout.Popup(new GUIContent("Folder Location:",
                        "Location of the folder in resources."), _resourceCategoriesIndex, _resourceCategoriesArray);
                    _folderLocation = _resourceCategoriesArray[_resourceCategoriesIndex];
                    
                    GUILayout.Space(5);
                    
                    GUILayout.BeginHorizontal();
                    if (Button("Add", "Adds the category."))
                    {
                        if (string.IsNullOrEmpty(_categoryName)) _categoryName = _folderLocation;
                        
                        TankBuilder.AddCategory(_categoryName, _folderLocation);
                        _resourceCategoriesIndex = 0;
                        _addCategory = false;
                        _categoryName = "";
                        _folderLocation = "";
                        GUI.FocusControl(null);
                    }

                    if (Button("Back", "Goes back to main."))
                    {
                        _resourceCategoriesIndex = 0;
                        _addCategory = false;
                        _categoryName = "";
                        _folderLocation = "";
                        GUI.FocusControl(null);
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.EndVertical(); 
                }
            }
            
            if (_eraseAll || _addCategory) return;
            
            DrawLine(0.5f, 5, 5);

            if (TankBuilder.hull == null && TankBuilder.cannonRotor == null) return;

            if (Button("Add Movement System", "Adds and sets up all the necessary " +
                                                          "components for the tank to move."))
                TankBuilder.AddMovementSystem();

            if (TankBuilder.tankController == null) return;
            
            GUILayout.Space(1.5f);
                
            if (Button("Add Weapon System", "Adds and sets up all the necessary " +
                                                        "components for the tank to shoot."))
                TankBuilder.AddWeaponSystem();
            
            GUILayout.Space(1.5f);
                
            if (Button("Add Camera System", "Adds and sets up all the necessary " +
                                                        "components for the camera."))
                TankBuilder.AddCameraSystem();
        }



        private void DrawSpawnHull()
        {
            GUILayout.BeginHorizontal();

            if (Foldout("Hull", "Spawn, toggles, or destroys the tanks hull.",
                ref TankBuilder.expandHull))
            {
                GUILayout.Space(-100);

                if (TexturedButton(plusTexture, "Spawns a new hull, destroying the old one.", 20f))
                    TankBuilder.SpawnHull();

                if (TankBuilder.hull != null)
                {
                    if (TexturedButton(minusTexture, "Destroys the tanks current hull.", 20f))
                        DestroyImmediate(TankBuilder.hull.gameObject);
                }

                GUILayout.EndHorizontal();

                DrawLine(0.5f, 5f, 5);

                if (TankBuilder.hull == null) return;

                GUI.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
                GUILayout.BeginVertical("box");
                GUI.backgroundColor = guiColorBackup;

                if (Button("Toggle", "Toggles the current hull."))
                    Selection.activeGameObject = TankBuilder.hull.gameObject;



                DrawLine(0.5f, 5f, 5);



                Label("Colors:", "Changes the various colors on the tanks hull.");
                {
                    EditorGUI.indentLevel++;

                    TankBuilder.hullColor = ColorField("Overall Color",
                        "Changes the color of the tanks hull.",
                        TankBuilder.hullColor, true, 120);
                    TankBuilder.hullAdditionalColor = ColorField("Additional Color",
                        "Changes the additional colors on the tanks hull.", TankBuilder.hullAdditionalColor, true, 120);
                    TankBuilder.hullShadowsColor = ColorField("Shadows Color",
                        "Changes the color of the shadows on the tanks hull.", TankBuilder.hullShadowsColor, true, 120);

                    EditorGUI.indentLevel--;
                }

                GUILayout.EndVertical();
            }
            else GUILayout.EndHorizontal();
        }



        private void DrawSpawnCannon()
        {
            GUILayout.BeginHorizontal();

            if (Foldout("Cannon", "Spawn, toggles, or destroys the tanks Cannon.",
                ref TankBuilder.expandCannon))
            {

                GUILayout.Space(-100);

                if (TexturedButton(plusTexture, "Spawns a new cannon, destroying the old one.", 20f))
                    TankBuilder.SpawnCannon();

                if (TankBuilder.cannonBase != null)
                {
                    if (TexturedButton(minusTexture, "Destroys the tanks current cannon.", 20f))
                        DestroyImmediate(TankBuilder.cannonBase.gameObject);
                }

                GUILayout.EndHorizontal();

                DrawLine(0.5f, 5f, 5);

                if (TankBuilder.cannonBase == null) return;

                GUI.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
                GUILayout.BeginVertical("box");
                GUI.backgroundColor = guiColorBackup;
                {
                    if (Button("Toggle", "Toggles the current cannon."))
                        Selection.activeGameObject = TankBuilder.cannonBase.gameObject;



                    DrawLine(0.5f, 5f, 5);



                    EditorGUIUtility.labelWidth = 100;
                    TankBuilder.cannonType = (Utility.TankBuilder.CannonType) EditorGUILayout.EnumPopup(new GUIContent(
                        "Cannon Type",
                        "The method used to follow the target."), TankBuilder.cannonType);



                    DrawLine(0.5f, 5f, 5);

                    
                    
                    GUILayout.BeginHorizontal();
                    {
                        Label("Base:", "Changes the values for the overall cannon rotor.", 100);

                        GUILayout.FlexibleSpace();

                        if (Button("Toggle", "Toggles the current cannon."))
                            Selection.activeGameObject = TankBuilder.cannonRotor.gameObject;
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(3);

                    TankBuilder.cannonCurrentTab = GUILayout.Toolbar(TankBuilder.cannonCurrentTab,
                        new string[] {"Position", "Size"});
                    
                    if (TankBuilder.cannonCurrentTab == 0)
                    {
                        Label("Position:", "Changes the position of the base.", 100);

                        EditorGUI.indentLevel++;

                        var position = TankBuilder.cannonRotorPosition;
                        position.x = Slider("X", "Changes the base's position along the x axis.",
                            position.x, -3.0f, 3.0f, 100);
                        position.y = Slider("Y", "Changes the base's position along the y axis.",
                            position.y, -7.0f, 5.0f, 100);
                        TankBuilder.cannonRotorPosition = position;

                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        Label("Local Scale:", "Changes the size of the overall cannon.", 100);

                        EditorGUI.indentLevel++;

                        var size = TankBuilder.cannonRotorSize;
                        size.x = Slider("X", "Changes the base's size along the x axis.",
                            size.x, -2.0f, 2.0f, 100);
                        size.y = Slider("Y", "Changes the base's size along the y axis.",
                            size.y, 0.0f, 2.0f, 100);
                        TankBuilder.cannonRotorSize = size;

                        EditorGUI.indentLevel--;
                    }

                    
                    
                    DrawLine(0.5f, 5f, 5);

                    
                    
                    if (TankBuilder.cannonType == Utility.TankBuilder.CannonType.Single)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            Label("Main Cannon:", "Changes the values for the main cannon.", 100);

                            GUILayout.FlexibleSpace();

                            if (Button("Toggle", "Toggles the current cannon."))
                                Selection.activeGameObject = TankBuilder.cannonHolder[0].gameObject;
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.Space(3);

                        EditorGUI.indentLevel++;

                        TankBuilder.singleCannonPosition = Slider("Position",
                            "Position of the cannon on the x axis.",
                            TankBuilder.singleCannonPosition, -2.5f, 2.5f, 100.0f);

                        TankBuilder.singleCannonSize = Slider("Size", "Size of the tanks cannon.",
                            TankBuilder.singleCannonSize, 0.5f, 1.0f, 100.0f);

                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        {
                            Label("Left Cannon:", "Changes the values for the left cannon.", 100);

                            GUILayout.FlexibleSpace();

                            if (Button("Toggle", "Toggles the current cannon."))
                                Selection.activeGameObject = TankBuilder.cannonHolder[0].gameObject;
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.Space(3);

                        EditorGUI.indentLevel++;

                        TankBuilder.leftCannonPosition = Slider("Position",
                            "Position of the cannon on the x axis.",
                            TankBuilder.leftCannonPosition, -2.5f, 0.0f, 100.0f);

                        TankBuilder.leftCannonSize = Slider("Size", "Size of the tanks cannon.",
                            TankBuilder.leftCannonSize, 0.5f, 1.0f, 100.0f);

                        EditorGUI.indentLevel--;

                        DrawLine(0.5f, 5f, 5);

                        GUILayout.BeginHorizontal();
                        {
                            Label("Right Cannon:", "Changes the values for the right cannon.", 100);

                            GUILayout.FlexibleSpace();

                            if (Button("Toggle", "Toggles the current cannon."))
                                Selection.activeGameObject = TankBuilder.cannonHolder[1].gameObject;
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.Space(3);

                        EditorGUI.indentLevel++;

                        TankBuilder.rightCannonPosition = Slider("Position",
                            "Position of the cannon on the x axis.",
                            TankBuilder.rightCannonPosition, 0.0f, 2.5f, 100.0f);

                        TankBuilder.rightCannonSize = Slider("Size", "Size of the tanks cannon.",
                            TankBuilder.rightCannonSize, 0.5f, 1.0f, 100.0f);

                        EditorGUI.indentLevel--;
                    }

                    DrawLine(0.5f, 5f, 5);
                    
                    Label("Colors:", "Changes the various colors on the tanks cannon.");
                    {
                        EditorGUI.indentLevel++;

                        GUILayout.BeginHorizontal();
                        {
                            Label("Base", "Changes the color of the cannons base.", 100);

                            TankBuilder.cannonBaseColor = EditorGUILayout.ColorField(TankBuilder.cannonBaseColor);
                            TankBuilder.cannonBaseSidesColor =
                                EditorGUILayout.ColorField(TankBuilder.cannonBaseSidesColor);

                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.Space(5);

                        if (TankBuilder.cannonType == Utility.TankBuilder.CannonType.Single)
                        {
                            TankBuilder.cannonColor = ColorField("Cannon",
                                "Changes the color of the tanks cannon.", TankBuilder.cannonColor, true, 120);
                        }
                        else
                        {
                            TankBuilder.leftCannonColor = ColorField("Left Cannon",
                                "Changes the color of the tanks left cannon.", TankBuilder.leftCannonColor, true,
                                120);
                            TankBuilder.rightCannonColor = ColorField("Right Cannon",
                                "Changes the color of the tanks right cannon.", TankBuilder.rightCannonColor, true,
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
            foreach (var category in TankBuilder.categories)
            {
                GUILayout.BeginHorizontal();

                var expandCategory = category.expandCategory;

                if (Foldout(category.categoryName,
                    "Spawn, toggles, or destroys the accessories present within this category.",
                    ref expandCategory))
                {
                    GUILayout.Space(-100);

                    if (!category.editCategory)
                    {
                        if (TexturedButton(editTexture, "Edit Category?", 20f))
                        {
                            category.editCategory = true;
                            _newCategoryName = category.categoryName;

                            for(; _editResourceCategoriesIndex < _resourceCategoriesArray.Length; _editResourceCategoriesIndex++)
                                if (_resourceCategoriesArray[_editResourceCategoriesIndex] == category.categoryFolder)
                                    break;
                            
                            _newFolderLocation = category.categoryFolder;   
                        }
                    }

                    if (TexturedButton(plusTexture, "Creates a completely new accessory.", 20f))
                        TankBuilder.SpawnAccessory(index, category.categoryName, category.categoryFolder);

                    if (TexturedButton(minusTexture, "Removes this category.", 20f))
                    {
                        TankBuilder.RemoveCategory(index);
                        break;
                    }

                    GUILayout.EndHorizontal();

                    DrawLine(0.5f, 2.5f, 5);
                    
                    if (category.editCategory)
                    {
                        GUI.backgroundColor = new Color(0f, 0f, 0f, 0.4f);
                        GUILayout.BeginVertical("box");
                        GUI.backgroundColor = guiColorBackup;
                        {
                            Label("Edit Category:", "", 100);

                            EditorGUIUtility.labelWidth = 100;
                            _newCategoryName = EditorGUILayout.TextField(new GUIContent("Name:", 
                            "Name of the category."), _newCategoryName);

                            EditorGUIUtility.labelWidth = 100;
                            _editResourceCategoriesIndex = EditorGUILayout.Popup(new GUIContent("Folder Location:",
                                "Location of the folder in resources."), _editResourceCategoriesIndex, _resourceCategoriesArray);
                            _newFolderLocation = _resourceCategoriesArray[_editResourceCategoriesIndex];
                        
                            GUILayout.Space(5);
                        
                            GUILayout.BeginHorizontal();
                            {
                                if (Button("Apply", "Applies the changes."))
                                {
                                    if (string.IsNullOrEmpty(_newCategoryName)) _newCategoryName = _newFolderLocation;
                                    
                                    foreach (var accessory in category.accessories)
                                        accessory.parentName = _newCategoryName;
                                    
                                    var transform = TankBuilder.transform;
                                        
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
                                    
                                    _editResourceCategoriesIndex = 0;
                                    category.editCategory = false;
                                    _newCategoryName = "";
                                    _newFolderLocation = "";
                                    GUI.FocusControl(null);
                                    
                                    // Debug.Log("Tank Builder: New category name and folder has been applied.");
                                }

                                if (Button("Back", "Goes back to main."))
                                {
                                    _editResourceCategoriesIndex = 0;
                                    category.editCategory = false;
                                    _newCategoryName = "";
                                    _newFolderLocation = "";
                                    GUI.FocusControl(null);
                                }
                            }
                            GUILayout.EndHorizontal();
                    
                        }
                        GUILayout.EndVertical(); 
                        
                        DrawLine(0.5f, 2.5f, 5);
                    }

                    if (TankBuilder.hull == null) return;
                    
                    InitAccessoryStyle(category.categoryFolder, ref _accessoryStyles);
                    
                    DrawAccessories(index, ref category.accessories);
                }
                else GUILayout.EndHorizontal();

                if(index != TankBuilder.categories.Count - 1) DrawLine(0.5f, 2.5f, 5);
                else GUILayout.Space(2.5f);
                
                category.expandCategory = expandCategory;
                index++;
            }            
        }
        
        
        
        private void InitAccessoryStyle(string folderName, ref string[] styles)
        {
            const string path = "Tank Builder/Sprites/";
            var sprites = Resources.LoadAll<Sprite>(path + folderName);
            
            styles = new string[sprites.Length];
            
            for (var i = 0; i < sprites.Length; i++)
                styles[i] = sprites[i].name;
        }
        
        
        
        private void DrawAccessories(int index, ref List<Utility.TankBuilder.Accessory> accessories)
        {
            if (accessories.Count == 0) return;
            
            GUI.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = guiColorBackup;
            {
                var i = 0;
                foreach (var accessory in accessories)
                {
                    if (i > 0) DrawLine(0.5f, 5f, 5);
                    
                    GUILayout.BeginHorizontal();

                    Space(-40);
                    if (Foldout(accessory.Name, "", ref accessory.expand))
                    {
                        GUILayout.Space(-100);

                        if (!accessory.editName)
                        {
                            if (TexturedButton(editTexture, "Rename object?", 20f))
                            {
                                accessory.editName = true;
                                accessory.newName = accessory.Name;
                            }
                        }

                        if (TexturedButton(plusTexture, "Create a copy of this object?", 20f))
                        {
                            TankBuilder.CopyAccessory(index, accessory);
                            break;
                        }

                        if (TexturedButton(minusTexture, "Remove this object?", 20f))
                        {
                            if (accessory.transform != null) DestroyImmediate(accessory.transform.gameObject);
                            accessories.RemoveAt(i);
                            break;
                        }

                        GUILayout.EndHorizontal();

                        
                        
                        DrawLine(0.5f, 2.5f, 2.5f);
                        
                        
                        
                        GUI.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
                        GUILayout.BeginVertical("box");
                        GUI.backgroundColor = guiColorBackup;
                        
                        if (accessory.editName)
                        {
                            GUI.backgroundColor = new Color(0f, 0f, 0f, 0.35f);
                            
                            ref var newName = ref accessory.newName ; 
                            
                            EditorGUIUtility.labelWidth = 50;
                            newName = EditorGUILayout.TextField(new GUIContent("Name", 
                                "Automatically renames the game object."), newName);
                            
                            GUI.backgroundColor = guiColorBackup;
                                
                            GUILayout.BeginHorizontal();
                            {
                                if (Button("Apply", "Finish renaming the accessory."))
                                {
                                    if (string.IsNullOrEmpty(newName)) newName = _accessoryStyles[accessory.Style];
                                    
                                    accessory.Name = newName;
                                    accessory.editName = false;
                                    GUI.FocusControl(null);
                                    
                                    // Debug.Log("Tank Builder: New accessory name has been applied.");
                                }
                                
                                if (Button("Back", "Goes back to main."))
                                {
                                    accessory.editName = false;
                                    GUI.FocusControl(null);
                                }
                            }
                            GUILayout.EndHorizontal();
                            
                            DrawLine(0.5f, 2.5f, 5);
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

                        if (Button("Toggle", "Toggles the current accessory."))
                            Selection.activeGameObject = accessory.transform.gameObject;

                        
                        
                        DrawLine(0.5f, 2.5f, 2.5f);


                        
                        EditorGUIUtility.labelWidth = 100;
                        accessory.Style = EditorGUILayout.Popup(new GUIContent("Style",
                        "Changes the style of the accessory."), accessory.Style, _accessoryStyles);

                        
                        
                        DrawLine(0.5f, 2.5f, 5);



                        var currentTab = accessory.currentTab;
                        accessory.currentTab = GUILayout.Toolbar(accessory.currentTab,
                            new string[] {"Position", "Rotation", "Size"});
                        
                        if(accessory.currentTab != currentTab) GUI.FocusControl(null);

                        switch (accessory.currentTab)
                        {
                            case 0:
                            {
                                Label("Position:", "Changes the position of the accessory.", 100);

                                EditorGUI.indentLevel++;

                                var position = accessory.Position;
                                position.x = Slider("X", "Changes the accessories position along the x axis.",
                                    position.x, -10.0f, 10.0f, 100);
                                position.y = Slider("Y", "Changes the accessories position along the y axis.",
                                    position.y, -15.0f, 15.0f, 100);
                                accessory.Position = position;

                                EditorGUI.indentLevel--;
                                break;
                            }
                            case 1:
                            {
                                Label("Rotation:", "Changes the rotation of the accessory.", 100);

                                EditorGUI.indentLevel++;

                                accessory.Rotation = Slider("Y",
                                    "Changes the accessories rotation along the y axis.",
                                    accessory.Rotation, -180.0f, 180.0f, 100);

                                EditorGUI.indentLevel--;

                                break;
                            }
                            case 2:
                            {
                                Label("Local Scale:", "Changes the scale of the accessory.", 100);

                                EditorGUI.indentLevel++;

                                var scale = accessory.Scale;
                                scale.x = Slider("X", "Changes the accessories position along the x axis.",
                                    scale.x, -2.0f, 2.0f, 100);
                                scale.y = Slider("Y", "Changes the accessories position along the y axis.",
                                    scale.y, -2.0f, 2.0f, 100);
                                accessory.Scale = scale;

                                EditorGUI.indentLevel--;
                                break;
                            }

                            default:
                                break;
                        }

                        DrawLine(0.5f, 5f, 5);

                        accessory.CurrentParent = EditorGUILayout.Popup(new GUIContent("Parent",
                                "The current parent of the accessory.\n\n" +
                                "eg. if parent to the cannon, this accessory will rotate with it."), accessory.CurrentParent,
                            new string[] {"Hull", "Cannon"});
                        
                        GUILayout.Space(2.5f);
                        
                        accessory.Color = ColorField("Color",
                            "Changes the color of the accessory.", accessory.Color, true, 100);

                        accessory.SortingOrder = IntField("Sorting Order",
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
