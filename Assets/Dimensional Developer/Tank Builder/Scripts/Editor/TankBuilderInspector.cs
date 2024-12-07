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
        
        
        #region Fields
        
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
        
        private int hullCurrentTab = 0;
        private int cannonCurrentTab = 0;
        
        // Used in the editor to provide an extra support for placing objects.
        private enum HandleType { Move, Rotation, Scale };
        private HandleType gizmoHandleType;
        private Transform accessoryGizmo;
        private Transform firePointGizmo;

        #endregion

        
        
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
            
            Space(5);
        }
        
        
        
        private void DrawTop()
        {
            EditorGUILayout.BeginHorizontal();
            Label("Sorting Layer:", "The sorting layer to assign to the tank.", LabelWidth);
            SortingLayerField("", "", _sortingLayerId, EditorStyles.popup, EditorStyles.label);

            if(Button("Set", "Sets the sorting layer to all sprite renderers among children.", 60))
                TankBuilder.SetSortingLayer();
            
            EditorGUILayout.EndHorizontal();

            DrawLine(0.5f, 8f, 5);
            
            // SetBackgroundColor(Color.grey);

            if (!_addCategory)
            {
                if (!_eraseAll)
                {
                    if (Button("Erase All", "Erases everything without spawning a new tank."))
                        _eraseAll = true;
                }
                else
                {
                    BeginVertical();
                    SetBackgroundColor(new Color(1f, 0f, 0f, 1f));

                    
                    Space(5);
                    Label("Erase All:", "", LabelWidth);
                    GUILayout.BeginHorizontal();
                    Label("Are you sure?", "", LabelWidth);
                    if (Button("Yes", "Erases everything."))
                    {
                        TankBuilder.EraseAll();
                        _eraseAll = false;
                    }
                    
                    if (Button("No", "")) _eraseAll = false;
                    
                    GUILayout.EndHorizontal(); 
                    Space(15);
                    
                    EndVertical(); 
                }
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
                    BeginVertical();

                    Label("Add Category:", "", LabelWidth);
                    
                    SetBackgroundColor(new Color(0f, 0f, 0f, 0.5f));
                    _categoryName = StringField("Name:", "Name of the category to add.", _categoryName, LabelWidth);
                    ResetBackgroundColor();
                        
                    _resourceCategoriesIndex = Popup("Folder Location:",
                        "Location of the folder in resources.", _resourceCategoriesIndex, _resourceCategoriesArray, LabelWidth);
                    _folderLocation = _resourceCategoriesArray[_resourceCategoriesIndex];
                    
                    Space(5);
                    
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
                    
                    EndVertical(); 
                }
            }
            
            if (_eraseAll || _addCategory) return;
            
            DrawLine(0.5f, 5, 5);

            if (TankBuilder.hull == null && TankBuilder.cannonRotor == null) return;

            if (Button("Add Movement System", "Adds and sets up all the necessary " +
                                                          "components for the tank to move."))
                TankBuilder.AddMovementSystem();

            if (TankBuilder.tankController == null) return;
            
            Space(1.5f);
                
            if (Button("Add Weapon System", "Adds and sets up all the necessary " +
                                                        "components for the tank to shoot."))
                TankBuilder.AddWeaponSystem();
            
            Space(1.5f);
                
            if (Button("Add Camera System", "Adds and sets up all the necessary " +
                                                        "components for the camera."))
                TankBuilder.AddCameraSystem();
        }


        
        
        private void DrawSpawnHull()
        {
            GUILayout.BeginHorizontal();

            TankBuilder.hullCategory ??= new Utility.TankBuilder.Category()
            {
                categoryName = "Hull",
                categoryFolder = "Hull",
                expandCategory = false,
                editCategory = false,
                accessories = new List<Utility.TankBuilder.Accessory>()
            };
            
            var category = TankBuilder.hullCategory;
            var expandCategory = category.expandCategory;

            if (Foldout(category.categoryName, "Spawn, toggles, or destroys the tanks hull.",
                    ref expandCategory))
            {
                category.expandCategory = expandCategory;
                
                Space(-100);
                
                if (TexturedButton(plusTexture, "Creates a completely new accessory."))
                {
                    TankBuilder.SpawnHull();
                    TankBuilder.SpawnAccessory(category);
                }

                if (TankBuilder.hull != null)
                {
                    if (TexturedButton(minusTexture, "Warning: Destroys the tanks current hull and everything on it."))
                        DestroyImmediate(TankBuilder.hull.gameObject);
                }

                GUILayout.EndHorizontal();

                
                
                // -------------------------------------------------------------------------------------------------
                DrawLine(0.5f, 5f, 5); // Toggle
                // -------------------------------------------------------------------------------------------------

                

                if (TankBuilder.hull == null)
                {
                    if (!Button("Build", "Builds the current hull.")) 
                        return;
                    
                    TankBuilder.SpawnHull();
                    TankBuilder.SpawnAccessory(category);

                    return;
                }
                
                if (Button("Toggle", "Toggles the current hull."))
                    Selection.activeGameObject = TankBuilder.hull.gameObject;


                
                // -------------------------------------------------------------------------------------------------
                DrawLine(0.5f, 5f, 5); // Position, Rotation, Scale
                // -------------------------------------------------------------------------------------------------


                
                if (accessoryGizmo != TankBuilder.hull) hullCurrentTab = -1;
                hullCurrentTab = GUILayout.Toolbar(hullCurrentTab, new[] {"Position", "Size"});

                if (hullCurrentTab == 0)
                {
                    Label("Position:", "Changes the position of the base.", LabelWidth);
                    
                    accessoryGizmo = TankBuilder.hull;
                    gizmoHandleType = HandleType.Move;

                    EditorGUI.indentLevel++;

                    var position = TankBuilder.hull.localPosition;
                    position.x = Slider("X", "Changes the base's position along the x axis.",
                        position.x, -3.0f, 3.0f, LabelWidth);
                    position.y = Slider("Y", "Changes the base's position along the y axis.",
                        position.y, -7.0f, 5.0f, LabelWidth);
                    TankBuilder.hull.localPosition = position;

                    EditorGUI.indentLevel--;
                }
                else if (hullCurrentTab == 1)
                {
                    Label("Local Scale:", "Changes the size of the overall cannon.", LabelWidth);
                    
                    accessoryGizmo = TankBuilder.hull;
                    gizmoHandleType = HandleType.Scale;

                    EditorGUI.indentLevel++;

                    var size = TankBuilder.hull.localScale;
                    size.x = Slider("X", "Changes the base's size along the x axis.",
                        size.x, -2.0f, 2.0f, LabelWidth);
                    size.y = Slider("Y", "Changes the base's size along the y axis.",
                        size.y, 0.0f, 2.0f, LabelWidth);
                    TankBuilder.hull.localScale = size;

                    EditorGUI.indentLevel--;
                }
                
                
                
                // -------------------------------------------------------------------------------------------------
                DrawLine(0.5f, 5f, 5); // Accessories
                // -------------------------------------------------------------------------------------------------

                

                category.accessories ??= new List<Utility.TankBuilder.Accessory>();
                InitAccessoryStyle(category.categoryFolder, ref _accessoryStyles);
                DrawAccessories(category, ref category.accessories);

            }
            else
            {
                GUILayout.EndHorizontal();

                if (accessoryGizmo == TankBuilder.hull) accessoryGizmo = null;
            }

            category.expandCategory = expandCategory;
        }
        
        
        
        private void DrawSpawnCannon()
        {
            GUILayout.BeginHorizontal();

            TankBuilder.cannonCategory ??= new Utility.TankBuilder.Category()
            {
                categoryName = "Cannon",
                categoryFolder = "Cannon",
                expandCategory = false,
                editCategory = false,
                accessories = new List<Utility.TankBuilder.Accessory>()
            };
            
            var category = TankBuilder.cannonCategory;
            var expandCategory = category.expandCategory;

            if (String.IsNullOrEmpty(category.categoryName))
            {
                category.categoryName = "Cannon";
                category.categoryFolder = "Cannon";
            }

            if (Foldout("Cannon", "Spawn, toggles, or destroys the tanks Cannon.",
                    ref expandCategory))
            {
                category.expandCategory = expandCategory;
                Space(-100);

                if (TexturedButton(plusTexture, "Creates a completely new accessory."))
                {
                    TankBuilder.SpawnCannon();
                    TankBuilder.SpawnAccessory(category);
                }
                

                if (TankBuilder.cannonRotor != null)
                {
                    if (TexturedButton(minusTexture, "Destroys the tanks current cannon."))
                        DestroyImmediate(TankBuilder.cannonRotor.gameObject);
                }

                GUILayout.EndHorizontal();

                
                
                // -------------------------------------------------------------------------------------------------
                DrawLine(0.5f, 5f, 5); // Toggle
                // -------------------------------------------------------------------------------------------------



                if (TankBuilder.cannonRotor == null)
                {
                    if (!Button("Build", "Builds the current cannon.")) 
                        return;
                    
                    TankBuilder.SpawnCannon();
                    TankBuilder.SpawnAccessory(category);

                    return;
                }

                if (Button("Toggle", "Toggles the current cannon."))
                    Selection.activeGameObject = TankBuilder.cannonRotor.gameObject;


                // -------------------------------------------------------------------------------------------------
                DrawLine(0.5f, 5f, 5); // Position, Rotation, Scale
                // -------------------------------------------------------------------------------------------------


                
                if (accessoryGizmo != TankBuilder.cannonRotor) cannonCurrentTab = -1;
                cannonCurrentTab = GUILayout.Toolbar(cannonCurrentTab, new[] {"Position", "Size"});

                if (cannonCurrentTab == 0)
                {
                    Label("Position:", "Changes the position of the cannon.", LabelWidth);
                    
                    accessoryGizmo = TankBuilder.cannonRotor;
                    gizmoHandleType = HandleType.Move;

                    EditorGUI.indentLevel++;

                    var position = TankBuilder.cannonRotor.localPosition;
                    position.x = Slider("X", "Changes the base's position along the x axis.",
                        position.x, -3.0f, 3.0f, LabelWidth);
                    position.y = Slider("Y", "Changes the base's position along the y axis.",
                        position.y, -7.0f, 5.0f, LabelWidth);
                    TankBuilder.cannonRotor.localPosition = position;

                    EditorGUI.indentLevel--;
                }
                else if (cannonCurrentTab == 1)
                {
                    Label("Local Scale:", "Changes the size of the overall cannon.", LabelWidth);
                    
                    accessoryGizmo = TankBuilder.cannonRotor;
                    gizmoHandleType = HandleType.Scale;

                    EditorGUI.indentLevel++;

                    var size = TankBuilder.cannonRotor.localScale;
                    size.x = Slider("X", "Changes the base's size along the x axis.",
                        size.x, -2.0f, 2.0f, LabelWidth);
                    size.y = Slider("Y", "Changes the base's size along the y axis.",
                        size.y, 0.0f, 2.0f, LabelWidth);
                    TankBuilder.cannonRotor.localScale = size;

                    EditorGUI.indentLevel--;
                }

                
                // -------------------------------------------------------------------------------------------------
                DrawLine(0.5f, 5f, 5); // Accessories
                // -------------------------------------------------------------------------------------------------

                
                category.accessories ??= new List<Utility.TankBuilder.Accessory>();
                InitAccessoryStyle(category.categoryFolder, ref _accessoryStyles);
                DrawAccessories(category, ref category.accessories);
            }
            else
            {
                GUILayout.EndHorizontal();

                if (accessoryGizmo == TankBuilder.cannonRotor) accessoryGizmo = null;
            }
            
            category.expandCategory = expandCategory;
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
                    Space(-100);

                    if (!category.editCategory)
                    {
                        if (TexturedButton(editTexture, "Edit Category?"))
                        {
                            category.editCategory = true;
                            _newCategoryName = category.categoryName;

                            for(; _editResourceCategoriesIndex < _resourceCategoriesArray.Length; _editResourceCategoriesIndex++)
                                if (_resourceCategoriesArray[_editResourceCategoriesIndex] == category.categoryFolder)
                                    break;
                            
                            _newFolderLocation = category.categoryFolder;   
                        }
                    }

                    if (TexturedButton(plusTexture, "Creates a completely new accessory."))
                        TankBuilder.SpawnAccessory(category);

                    if (TexturedButton(minusTexture, "Removes this category."))
                    {
                        TankBuilder.RemoveCategory(index);
                        break;
                    }

                    GUILayout.EndHorizontal();

                    
                    
                    DrawLine(0.5f, 2.5f, 5);
                    
                    
                    
                    if (category.editCategory)
                    {
                        BeginVertical(new Color(0,0,0,0.4f));
                        {
                            Label("Edit Category:", "", LabelWidth);
                            
                            SetBackgroundColor(new Color(0f, 0f, 0f, 0.5f));
                            _newCategoryName = StringField("Name:", "Name of the category.", _newCategoryName, LabelWidth);
                            ResetBackgroundColor();

                            _editResourceCategoriesIndex = Popup("Folder Location:",
                                "Location of the folder in resources.", _editResourceCategoriesIndex, _resourceCategoriesArray, LabelWidth);
                            _newFolderLocation = _resourceCategoriesArray[_editResourceCategoriesIndex];
                        
                            Space(5);
                        
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
                        EndVertical(); 
                        
                        DrawLine(0.5f, 2.5f, 5);
                    }

                    if (TankBuilder.hull == null) return;
                    
                    InitAccessoryStyle(category.categoryFolder, ref _accessoryStyles);
                    
                    DrawAccessories(category, ref category.accessories);
                }
                else GUILayout.EndHorizontal();

                if(index != TankBuilder.categories.Count - 1) DrawLine(0.5f, 2.5f, 5);
                else Space(2.5f);
                
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
        
        
        
        private void DrawAccessories(Utility.TankBuilder.Category category, ref List<Utility.TankBuilder.Accessory> accessories)
        {
            // A button that shows when the category has no accessories. 
            if (accessories.Count == 0)
            {
                if (!Button("Add Accessory", "Add a new accessory.")) 
                    return;
                
                TankBuilder.SpawnCannon();
                TankBuilder.SpawnAccessory(category);
                
                return;
            }
            
            BeginVertical();
            {
                var i = 0;
                foreach (var accessory in accessories)
                {
                    if (i > 0) DrawLine(0.5f, 5f, 5);
                    
                    GUILayout.BeginHorizontal();

                    Space(-40);
                    if (Foldout(accessory.Name, "", ref accessory.expand))
                    {
                        Space(-100);

                        if (!accessory.editName)
                        {
                            if (TexturedButton(editTexture, "Rename object?"))
                            {
                                accessory.editName = true;
                                accessory.newName = accessory.Name;
                            }
                        }

                        if (accessory.transform != null)
                        {
                            var hasCollider = accessory.transform.gameObject.GetComponent<PolygonCollider2D>() == null;
                            if (hasCollider)
                            {
                                if (TexturedButton(colliderAddTexture, "Create a collider on this object"))
                                {
                                    accessory.transform.gameObject.AddComponent<PolygonCollider2D>();
                                    break;
                                }
                            }
                            else
                            {
                                if (TexturedButton(colliderRemoveTexture, "Removes the collider from this object"))
                                {
                                    DestroyImmediate(accessory.transform.gameObject.GetComponent<PolygonCollider2D>());
                                    break;
                                }
                            }
                        }
                        
                        if (TexturedButton(plusTexture, "Create a copy of this object?"))
                        {
                            category.CopyAccessory(accessory);
                            break;
                        }

                        if (TexturedButton(minusTexture, "Remove this object?"))
                        {
                            if (accessory.transform != null) DestroyImmediate(accessory.transform.gameObject);
                            accessories.RemoveAt(i);
                            break;
                        }

                        GUILayout.EndHorizontal();

                        
                        //----------------------------------------------------------------------------------------------
                        DrawLine(0.5f, 2.5f, 2.5f); // Edit Item
                        //----------------------------------------------------------------------------------------------
                        
                        
                        
                        BeginVertical();
                        
                        if (accessory.editName)
                        {
                            SetBackgroundColor(new Color(0f, 0f, 0f, 0.35f));
                            
                            ref var newName = ref accessory.newName ; 
                            
                            newName = StringField("Name", "Automatically renames the game object.", 
                                newName, LabelWidth);
                            
                            ResetBackgroundColor();
                                
                            GUILayout.BeginHorizontal();
                            {
                                if (Button("Apply", "Finish renaming the accessory."))
                                {
                                    if (string.IsNullOrEmpty(newName)) newName = _accessoryStyles[accessory.Style];
                                    
                                    accessory.Name = newName;
                                    accessory.editName = false;
                                    GUI.FocusControl(null);
                                }
                                
                                if (Button("Match Style", "Matches the name of the current style."))
                                {
                                    if (string.IsNullOrEmpty(newName)) newName = _accessoryStyles[accessory.Style];
                                    
                                    accessory.Name = accessory.accessorySprite.sprite.name;
                                    accessory.editName = false;
                                    GUI.FocusControl(null);
                                }
                                
                                if (Button("Back", "Goes back to main."))
                                {
                                    accessory.editName = false;
                                    GUI.FocusControl(null);
                                }
                            }
                            GUILayout.EndHorizontal();
                            
                            
                            //----------------------------------------------------------------------------------------------
                            DrawLine(0.5f, 2.5f, 5); // End of edit item.
                            //----------------------------------------------------------------------------------------------
                            
                            
                            
                        }
                        
                        // Missing Transform
                        if (accessory.transform == null)
                        {
                            ObjectField("Missing Transform:", accessory.Name + "'s transform was not found, please reassign it or delete this item.", ref accessory.transform, LabelWidth);
                            EndVertical();

                            continue;
                        }

                        // Missing Renderer
                        if (accessory.accessorySprite == null)
                        {
                            accessory.accessorySprite = accessory.transform.GetComponent<SpriteRenderer>();
                            if (accessory.accessorySprite == null)
                            {
                                ObjectField("Missing Renderer:", accessory.Name + "'s renderer was not found, please reassign it or delete this item.", ref accessory.accessorySprite, LabelWidth);
                            }
                        }

                        if (Button("Toggle", "Toggles the current accessory."))
                            Selection.activeGameObject = accessory.transform.gameObject;

                        
                        
                        //----------------------------------------------------------------------------------------------
                        DrawLine(0.5f, 2.5f, 2.5f); // Style
                        //----------------------------------------------------------------------------------------------


                        
                        accessory.Style = Popup("Style",
                        "Changes the style of the accessory.", accessory.Style, _accessoryStyles, LabelWidth);

                        
                        
                        //----------------------------------------------------------------------------------------------
                        DrawLine(0.5f, 2.5f, 5); // Position, Rotation, Size
                        //----------------------------------------------------------------------------------------------



                        var currentTab = accessory.currentTab;
                        if (accessoryGizmo != accessory.transform) currentTab = -1;
                        
                        accessory.currentTab = TabsToggle(accessory.currentTab, new string[] {"Position", "Rotation", "Size"});
                        
                        if(accessory.currentTab != currentTab) GUI.FocusControl(null);

                        switch (accessory.currentTab)
                        {
                            case 0:
                            {
                                Label("Position:", "Changes the position of the accessory.", LabelWidth);
                                
                                accessoryGizmo = accessory.transform;
                                gizmoHandleType = HandleType.Move;

                                EditorGUI.indentLevel++;

                                var position = accessory.Position;
                                position.x = Slider("X", "Changes the accessories position along the x axis.",
                                    position.x, -10.0f, 10.0f, LabelWidth);
                                position.y = Slider("Y", "Changes the accessories position along the y axis.",
                                    position.y, -15.0f, 15.0f, LabelWidth);
                                accessory.Position = position;

                                EditorGUI.indentLevel--;
                                break;
                            }
                            case 1:
                            {
                                Label("Rotation:", "Changes the rotation of the accessory.", LabelWidth);
                                
                                accessoryGizmo = accessory.transform;
                                gizmoHandleType = HandleType.Rotation;

                                EditorGUI.indentLevel++;

                                accessory.Rotation = Slider("Y",
                                    "Changes the accessories rotation along the y axis.",
                                    accessory.Rotation, -180.0f, 180.0f, LabelWidth);

                                EditorGUI.indentLevel--;

                                break;
                            }
                            case 2:
                            {
                                Label("Local Scale:", "Changes the scale of the accessory.", LabelWidth);
                                
                                accessoryGizmo = accessory.transform;
                                gizmoHandleType = HandleType.Scale;

                                EditorGUI.indentLevel++;

                                var scale = accessory.Scale;
                                scale.x = Slider("X", "Changes the accessories position along the x axis.",
                                    scale.x, -2.0f, 2.0f, LabelWidth);
                                scale.y = Slider("Y", "Changes the accessories position along the y axis.",
                                    scale.y, -2.0f, 2.0f, LabelWidth);
                                accessory.Scale = scale;

                                EditorGUI.indentLevel--;
                                break;
                            }
                        }

                        
                        //----------------------------------------------------------------------------------------------
                        DrawLine(0.5f, 5f, 5); // FirePoint
                        //----------------------------------------------------------------------------------------------

                        
                        
                        var isWeapon = accessory.IsWeapon;

                        if (!isWeapon)
                        {
                            if (Button("Add Fire Point", "Converts this part into a weapon."))
                            {
                                var firePoint = accessory.AddWeapon();
                               if(TankBuilder.weaponController != null) TankBuilder.weaponController.AddWeapon(firePoint);
                            }
                        }
                        else
                        {
                            var firePoint = accessory.firePoint;
                            
                            if (Button("Remove Fire Point", "Converts this part into an accessory."))
                            {
                                if(TankBuilder.weaponController != null) TankBuilder.weaponController.RemoveWeapon(firePoint);
                                DestroyImmediate(firePoint.gameObject);
                            }
                            
                            if (firePointGizmo != firePoint)
                            {
                                if (Button("Show Fire Point Gizmo", "Shows the Fire Points Handle"))
                                    firePointGizmo = firePoint;
                            }
                            else
                            {
                                if (Button("Hide Fire Point Gizmo", "Hides the Fire Points Handle"))
                                    firePointGizmo = null;
                            }
                        }
                        
                        
                        
                        //----------------------------------------------------------------------------------------------
                        DrawLine(0.5f, 5f, 5); // Colors
                        //----------------------------------------------------------------------------------------------

                        
                        
                        if (category.categoryName is not ("Hull" or "Cannon"))
                        {
                            accessory.CurrentParent = Popup("Parent",
                                    "The current parent of the accessory.\n\n" +
                                    "eg. if parent to the cannon, this accessory will rotate with it.", accessory.CurrentParent,
                                new string[] {"Hull", "Cannon"}, LabelWidth);
                        }
                        
                        Space(2.5f);
                        
                        accessory.Color = ColorField("Color",
                            "Changes the color of the accessory.", accessory.Color, true, LabelWidth);

                        accessory.SortingOrder = IntField("Sorting Order",
                            "Where this accessory standing within the chosen sorting layer.",
                            accessory.SortingOrder, LabelWidth);
                        
                        EndVertical();

                    }
                    else
                    {
                        GUILayout.EndHorizontal();

                        if (accessoryGizmo == accessory.transform) accessoryGizmo = null;
                    }

                    i++;
                }
            }
            EndVertical();
        }



        public void OnSceneGUI()
        {
            for (var index = 0; index < 2; index++)
            {
                var isAccessory = index == 0;
                
                var element = isAccessory ? accessoryGizmo : firePointGizmo;
                if (element == null) continue;
                
                // Handle
                const float opacity = 0.75f;
                const float size = 0.15f;
                Handles.color = isAccessory ? new Color(0f, 0f, 1f, opacity) : new Color(1f, 0f, 0f, opacity);
                Handles.SphereHandleCap(0, element.position, Quaternion.identity, size, EventType.Repaint);
                    
                // Perform Changes
                EditorGUI.BeginChangeCheck();

                if (!isAccessory)
                {
                    var newPosition = Handles.PositionHandle(element.position, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck()) 
                    {
                        Undo.RecordObject(element, "Change Fire Point Position");
                        element.position = newPosition;
                        
                        EditorUtility.SetDirty(element);
                    }

                    return;
                }
                
                var vector3 = (gizmoHandleType) switch
                {
                    HandleType.Move => Handles.PositionHandle(element.position, element.rotation),
                    HandleType.Scale => Handles.ScaleHandle(element.localScale, element.position, element.rotation),
                    _ => Vector3.zero
                };

                var rotation = Quaternion.identity;
                if (gizmoHandleType == HandleType.Rotation) rotation = Handles.RotationHandle(element.rotation, element.position);
                
                if (EditorGUI.EndChangeCheck()) 
                {
                    Undo.RecordObject(element, "Change Accessory Position");
                    
                    switch(gizmoHandleType) 
                    {
                        case HandleType.Move:
                            element.position = vector3;
                            break;
                        case HandleType.Rotation:
                            element.rotation = rotation;
                            break;
                        case HandleType.Scale:
                            element.localScale = vector3;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                        
                    EditorUtility.SetDirty(element);
                }
            }
        }
        
    }
}
