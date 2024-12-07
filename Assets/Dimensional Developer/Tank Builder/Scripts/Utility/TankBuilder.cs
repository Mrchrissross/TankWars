using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DimensionalDeveloper.TankBuilder.Controllers;
using UnityEditor;

namespace DimensionalDeveloper.TankBuilder.Utility
{
    /// <summary>
    /// A utility script used to quickly and efficiently build tanks from scratch.
    /// </summary>

    public class TankBuilder : MonoBehaviour
    {
        
        
        #region Classes
        
        
        [Serializable]
        public class Category
        {
            public string categoryName;
            public string categoryFolder;
            public bool expandCategory;
            public bool editCategory;
            public List<Accessory> accessories;

            /// <summary>
            /// Creates an exact copy of the input accessory.
            /// </summary>
            /// <param name="accessory">The accessory to be copied.</param>
        
            public void CopyAccessory(Accessory accessory) => 
                accessories.Add(new Accessory(accessory));
        }
        
        
        
        [Serializable]
        public class Accessory
        {
            
            #region Fields

            public string newName;
            public string projectFolder;
            public string parentName;
            
            public Transform transform;
            public Transform firePoint;
            public SpriteRenderer accessorySprite;
            
            // Unity Editor Fields
            public int currentTab = 0;
            public bool expand;
            public bool editName;

            #endregion

            
            
            #region Properties

            public bool IsWeapon => firePoint != null;
            
            public int Style
            {
                get => style;
                set
                {
                    if (style == value) return;
                    style = value;
                    
                    accessorySprite.sprite = Resources.LoadAll<Sprite>(Path + projectFolder)[style];
                }
            }
            [SerializeField] private int style = 0;
            
            public int CurrentParent
            {
                get => currentParent;
                set
                {
                    if (currentParent == value) return;
                    currentParent = value;
                    
                    var grParent = transform.parent.parent.parent.Find(CurrentParent == 0 ? "Hull" : "Cannon");
                    var parent = grParent.Find(parentName);
                
                    if (parent == null)
                    {
                        parent = new GameObject().transform;
                        parent.name = parentName;
                        parent.parent = grParent;
                        parent.localPosition = Vector3.zero;
                        parent.localRotation = Quaternion.identity;
                    }
                
                    transform.parent = grParent.Find(parentName);
                }
            }
            [SerializeField] private int currentParent = 0;
            
            public string Name
            {
                get => name;
                set
                {
                    if (value == name) return;

                    name = value;
                    transform.name = name;
                }
            }
            [SerializeField] private string name = "New Accessory";
            
            public Vector2 Position
            {
                get => transform.localPosition;
                set
                {
                    if (value == Position) return;

                    var position = Position;
                    position.x = Mathf.Clamp(value.x, -10.0f, 10.0f);
                    position.y = Mathf.Clamp(value.y, -15.0f, 15.0f);
                    transform.localPosition = position;
                }
            }
            
            public float Rotation
            {
                get => transform.localEulerAngles.z;
                set
                {
                    if (Math.Abs(value - Rotation) < 0.001) return;

                    var rotation = Mathf.Clamp(value, -180.0f, 180.0f);
                    transform.localEulerAngles = transform.localEulerAngles.WithZ(rotation);
                }
            }
            
            public Vector2 Scale
            {
                get => transform.localScale;
                set
                {
                    if (value == Scale) return;

                    var scale = Scale;
                    scale.x = Mathf.Clamp(value.x, -2.0f, 2.0f);
                    scale.y = Mathf.Clamp(value.y, -2.0f, 2.0f);
                    transform.localScale = scale;
                }
            }
            
            public Color Color
            {
                get => accessorySprite.color;
                set
                {
                    if (value == Color) return;
                    accessorySprite.color = value;
                }
            }

            public int SortingOrder
            {
                get => accessorySprite.sortingOrder;
                set
                {
                    if (value == SortingOrder) return;
                    accessorySprite.sortingOrder = value;
                }
            }

            #endregion

            
            
            #region Functions

            /// <summary>
            /// Creates a completely new accessory.
            /// </summary>
            /// <param name="parent">The parent of the accessory.</param>
            /// <param name="folder">Where the sprite came from within the resources folder.</param>
            /// <param name="sortingLayerID">The sorting layer id, used to ensure the tank is on the right 2D level.</param>
            
            public Accessory(Transform parent, string folder, int sortingLayerID)
            {
                // Variable assignment
                parentName = parent.name;
                projectFolder = folder;
                
                // Accessory
                transform = new GameObject().transform;
                transform.name = Name;
                transform.parent = parent;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;

                // Sprite
                accessorySprite = transform.gameObject.AddComponent<SpriteRenderer>();
                accessorySprite.sprite = Resources.LoadAll<Sprite>(Path + projectFolder)[0];
                accessorySprite.sortingLayerID = sortingLayerID;            
                accessorySprite.sortingOrder = SortingOrder;
                accessorySprite.color = Color;
            }
            
            /// <summary>
            /// Makes a copy of an existing Accessory.
            /// </summary>
            /// <param name="copy">Accessory to copy.</param>
            
            public Accessory(Accessory copy)
            {
                // Variable assignment
                parentName = copy.parentName;
                projectFolder = copy.projectFolder;
                
                // Accessory
                transform = new GameObject().transform;
                Name = copy.Name + " copy";
                transform.parent = copy.transform.parent;
                CurrentParent = copy.CurrentParent;
                Position = copy.transform.localPosition;
                transform.localEulerAngles = transform.localEulerAngles.WithZ(copy.Rotation);
                transform.localScale = copy.Scale;

                // Copy the sprite renderer.
                var copySprite = copy.accessorySprite;
                accessorySprite = transform.gameObject.AddComponent<SpriteRenderer>();
                accessorySprite.sprite = copySprite.sprite;
                accessorySprite.sortingLayerID = copySprite.sortingLayerID;            
                SortingOrder = copySprite.sortingOrder;
                Color = copySprite.color;
                
                Style = copy.Style;
            }

            /// <summary>
            /// Converts the accessory into a weapon by creating a new fire point. 
            /// </summary>
            /// <returns></returns>
            
            public Transform AddWeapon()
            {
                firePoint = new GameObject().transform;
                firePoint.name = "FirePoint";
                firePoint.SetParent(transform);
                firePoint.localPosition = Vector2.zero;
                firePoint.localRotation = Quaternion.identity;
                firePoint.localScale = Vector3.one;

                return firePoint;
            }
            
            #endregion

        }
        
        
        #endregion
        
        
        
        
        
        #region Properties
        
        
        #region Hull Properties

        public Vector2 hullSize
        {
            get => _hullSize;
            set
            {
                if (value == _hullSize) return;
                
                _hullSize.x = Mathf.Clamp(value.x, -2.0f, 2.0f);
                _hullSize.y = Mathf.Clamp(value.y, 0.0f, 2.0f);

                if (hull == null) return;
                
                hull.parent.localScale = _hullSize;
                hull.parent.localScale = _hullSize;
            }
        }
        [SerializeField] private Vector2 _hullSize = Vector2.one;
            
            
        #endregion

        
        
        #region Cannon Properties

        public Vector2 cannonRotorSize
        {
            get => _cannonRotorSize;
            set
            {
                if (value == _cannonRotorSize) return;
                    
                _cannonRotorSize.x = Mathf.Clamp(value.x, -2.0f, 2.0f);
                _cannonRotorSize.y = Mathf.Clamp(value.y, 0.0f, 2.0f);
                cannonRotor.localScale = _cannonRotorSize;
            }
        }
        [SerializeField] private Vector2 _cannonRotorSize = Vector2.one;
        
        
        #endregion
        
        
        #endregion
        

        
        
        
        #region Fields

        // Constant variable leading to the tank sprites.
        private const string Path = "Tank Builder/Sprites/";
        
        // The sorting layer ID, used to assign the tank parts to a sorting layer.
        [HideInInspector] public int sortingLayerID = 0;
        
        // Storage of built controllers.
        public TankController tankController;
        public WeaponController weaponController;
        public CameraController cameraController;
        
        // Hull data.
        public Category hullCategory = new()
        {
            categoryName = "Hull",
            categoryFolder = "Hull"
        };
        public Transform hull;
        
        // Cannon data.
        public Category cannonCategory = new()
        {
            categoryName = "Cannon",
            categoryFolder = "Cannon"
        };
        public Transform cannonRotor;
        
        // Categories data.
        public List<Category> categories = new();
        
        #endregion

        

        

        #region Functions

        /// <summary>
        /// Sets the tanks of overall sorting layer.
        /// </summary>
        
        public virtual void SetSortingLayer()
        {
            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (var spriteRenderer in spriteRenderers)
                spriteRenderer.sortingLayerID = sortingLayerID;
        }
        
        /// <summary>
        /// Use with caution: Remove absolutely everything from the tank
        /// and deletes is all.
        /// </summary>
        public virtual void EraseAll()
        {
            transform.DestroyChildren();
            
            foreach (var category in categories) category.accessories.Clear();
            categories.Clear();

            hullCategory.accessories.Clear();
            cannonCategory.accessories.Clear();
            
            hullCategory = null;
            cannonCategory = null;
        }

        /// <summary>
        /// Spawns the tanks hull.
        /// </summary>
        
        public virtual void SpawnHull()
        {
            if (hull != null) return;
            
            // Create the tanks hull
            // (the game object that holds all elements attached to the hull).
            hull = new GameObject().transform;
            hull.name = "Hull";
            hull.parent = transform;
            
            // Reset the hulls position to zero.
            hull.localPosition = Vector2.zero;
            hull.localRotation = Quaternion.identity;
            hull.localScale = Vector2.one;
        }
        
        /// <summary>
        /// Spawns the tanks cannon.
        /// </summary>
        
        public virtual void SpawnCannon()
        {
            if(cannonRotor != null) return;
            
            cannonRotor = new GameObject().transform;
            cannonRotor.name = "Cannon";
            cannonRotor.parent = transform;
            cannonRotor.localPosition = Vector2.zero;
            cannonRotor.localRotation = Quaternion.identity;
            cannonRotor.localScale = Vector2.one;
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="nameOfCategory">Name of the category.</param>
        /// <param name="folderLocation">Origin folder - where the sprites are located in the resources.</param>
        
        public virtual void AddCategory(string nameOfCategory, string folderLocation)
        {
            if (nameOfCategory == "Hull" || nameOfCategory == "Base"|| nameOfCategory == "Cannon")
            {
                Debug.LogError("Tank Builder: The Categories \"Hull\"," +
                " \"Cannon\", and \"Base\" have been reserved for the main tank elements. " +
                "Please consider another category name.");

                return;
            }
            
            var category = new Category()
            {
                accessories = new List<Accessory>(),
                categoryFolder = folderLocation,
                categoryName = nameOfCategory,
                expandCategory = false,
                editCategory = false
            };
            categories.Add(category);

            for (var i = 0; i < 2; i++)
            {
                var grandParent = transform.Find(i == 0 ? "Hull" : "Cannon");
                if (grandParent == null) continue;
                
                var categoryTransform = grandParent.Find(nameOfCategory);
                if (categoryTransform != null) continue;

                categoryTransform = new GameObject().transform;
                categoryTransform.name = nameOfCategory;
                categoryTransform.parent = grandParent;
                categoryTransform.localPosition = Vector3.zero;
                categoryTransform.localRotation = Quaternion.identity;
                categoryTransform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Removes a new category.
        /// </summary>
        /// <param name="index">The index of the category.</param>
        
        public virtual void RemoveCategory(int index)
        {
            var categoryName = categories[index].categoryName;

            var category = transform.Find("Hull").Find(categoryName);
            if(category != null) DestroyImmediate(category.gameObject);
            category = transform.Find("Cannon").Find(categoryName);
            if(category != null) DestroyImmediate(category.gameObject);
            
            categories[index].accessories.Clear();
            categories.RemoveAt(index);
        }

        /// <summary>
        /// Spawns an accessory.
        /// </summary>
        /// <param name="index">Category index.</param>
        /// <param name="categoryName">Available Types:
        /// Vents, Compartments, Boxes, Extras.</param>
        /// <param name="projectFolder">Where the accessory sprite came from within the resources folder.</param>
        public virtual void SpawnAccessory(Category category)
        {
            var root = transform.Find(category.categoryName == "Cannon" ? "Cannon" : "Hull");
            if (root == null) return;
            
            var parent = root.Find(category.categoryName);
                
            if (parent == null)
            {
                parent = new GameObject().transform;
                parent.name = category.categoryName;
                parent.parent = root;
                parent.localPosition = Vector3.zero;
                parent.localRotation = Quaternion.identity;
                parent.localScale = Vector3.one;
            }
            
            category.accessories.Add(new Accessory(parent, category.categoryFolder, sortingLayerID));
        }

        /// <summary>
        /// Creates an exact copy of the input accessory.
        /// </summary>
        /// <param name="categoryIndex">The index of the category to add the new accessory to.</param>
        /// <param name="accessory">The accessory to be copied.</param>
        
        public virtual void CopyAccessory(int categoryIndex, Accessory accessory) => 
            categories[categoryIndex].accessories.Add(new Accessory(accessory));

        /// <summary>
        /// Returns all fire points created on the tank.
        /// </summary>
        
        public virtual List<Transform> GetFirePoints()
        {
            var firePoints = hullCategory.accessories.Where(accessory => accessory.IsWeapon).Select(accessory => accessory.firePoint).ToList();
            firePoints.AddRange(cannonCategory.accessories.Where(accessory => accessory.IsWeapon).Select(accessory => accessory.firePoint));
            firePoints.AddRange(from accessory in from category in categories from accessory in category.accessories where accessory.IsWeapon select accessory select accessory.firePoint);

            return firePoints;
        }
        
        /// <summary>
        /// Adds all movement components to the tanks game object.
        /// </summary>
        
        public virtual void AddMovementSystem()
        {
            if (hull == null || cannonRotor == null) return;
            
            tankController = GetComponent<TankController>();
            if(tankController != null) DestroyImmediate(tankController);

            tankController = gameObject.AddComponent<TankController>();
            tankController.CannonRotor = cannonRotor;

            // Rigidbody initialisation.
            var rb = GetComponent<Rigidbody2D>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
            
            rb.gravityScale = 0;
            rb.isKinematic = false;

            // Reassign the cameras tank controller.
            if (cameraController != null)
            {
                cameraController.tankController = tankController;
                tankController.camera = cameraController.Camera;
                EditorUtility.SetDirty(cameraController);
            }
            
            // Rename the tank.
            transform.name = "Player Tank";
            
            EditorUtility.SetDirty(tankController);
            print("Tank Builder: Movement system added.");
        }
        
        /// <summary>
        /// Adds all movement components to the tanks game object.
        /// </summary>
        
        public virtual void AddWeaponSystem()
        {
            if (hull == null || cannonRotor == null) return;
            
            weaponController = GetComponent<WeaponController>();
            if(weaponController != null) DestroyImmediate(weaponController);

            weaponController = gameObject.AddComponent<WeaponController>();

            weaponController.CreateWeapons(GetFirePoints());
            
            EditorUtility.SetDirty(weaponController);
            print("Tank Builder: Weapon system added.");
        }
        
        /// <summary>
        /// Adds all camera system to the hierarchy.
        /// </summary>
        
        public virtual void AddCameraSystem()
        {
            var ccGameObject = new GameObject();
            var ccTransform = ccGameObject.transform;
            ccTransform.name = "Camera";
            
            cameraController = ccGameObject.AddComponent<CameraController>();
            cameraController.tankController = tankController;
            cameraController.ResetTarget();
            cameraController.Reset();
            
            var cameraGameObject = new GameObject();
            var cameraTransform = cameraGameObject.transform;
            cameraTransform.name = "Main Camera";
            cameraTransform.parent = ccTransform;
            cameraTransform.localPosition = Vector3.zero.WithZ(-5);
            cameraTransform.localRotation = Quaternion.identity;

            var cam = cameraGameObject.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = cameraController.CameraDistance;
            tankController.camera = cam;
            
            cameraGameObject.AddComponent<AudioListener>();
            
            EditorUtility.SetDirty(tankController);
            EditorUtility.SetDirty(cameraController);
            
            if(cameraController.CameraTransform != null) print("Tank Builder: Camera system added.");
        }
        
        #endregion
        
        
    }
}
