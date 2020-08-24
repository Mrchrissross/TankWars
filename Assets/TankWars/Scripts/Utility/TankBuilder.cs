using System;
using System.Collections.Generic;
using TankWars.Controllers;
using UnityEngine;

namespace TankWars.Utility
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
        }
        
        [Serializable]
        public class Accessory
        {
            #region Fields

            public string newName;
            public string projectFolder;
            public string parentName;
            
            public Transform transform;
            public SpriteRenderer accessorySprite;
            
            // Unity Editor Fields
            public int currentTab = 0;
            public bool expand;
            public bool editName;

            #endregion

            
            
            #region Properties

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
                    
                    if (transform.parent.name == (CurrentParent == 0 ? "Cannon" : "Hull")) return;

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
                get => position;
                set
                {
                    if (value == position) return;
                    
                    position.x = Mathf.Clamp(value.x, -10.0f, 10.0f);
                    position.y = Mathf.Clamp(value.y, -13.0f, 13.0f);
                    transform.localPosition = position;
                }
            }
            [SerializeField] private Vector2 position = Vector2.zero;
            
            public float Rotation
            {
                get => rotation;
                set
                {
                    if (value == rotation) return;
                    
                    rotation = Mathf.Clamp(value, -180.0f, 180.0f);
                    transform.localEulerAngles = transform.localEulerAngles.WithZ(rotation);
                }
            }
            [SerializeField] private float rotation = 0;
            
            public Vector2 Scale
            {
                get => scale;
                set
                {
                    if (value == scale) return;
                    
                    scale.x = Mathf.Clamp(value.x, -2.0f, 2.0f);
                    scale.y = Mathf.Clamp(value.y, -2.0f, 2.0f);
                    transform.localScale = scale;
                }
            }
            [SerializeField] private Vector2 scale = Vector2.one;
            
            public Color Color
            {
                get => color;
                set
                {
                    if (value == color) return;

                    color = value;
                    accessorySprite.color = value;
                }
            }
            [SerializeField] private Color color = Color.white;

            public int SortingOrder
            {
                get => sortingOrder;
                set
                {
                    if (value == sortingOrder) return;

                    sortingOrder = value;
                    accessorySprite.sortingOrder = sortingOrder;
                }
            }
            [SerializeField] private int sortingOrder = 0;

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
                Rotation = copy.Rotation;
                Scale = copy.Scale;

                // Copy the sprite renderer.
                var copySprite = copy.accessorySprite;
                accessorySprite = transform.gameObject.AddComponent<SpriteRenderer>();
                accessorySprite.sprite = copySprite.sprite;
                accessorySprite.sortingLayerID = copySprite.sortingLayerID;            
                SortingOrder = copySprite.sortingOrder;
                Color = copySprite.color;
                
                Style = copy.Style;
            }
            
            #endregion

        }
        
        #endregion
        
        
        
        
        #region Properties

        public enum CannonType
        {
            Single,
            Double
        }
        
        public CannonType cannonType
        {
            get => _cannonType;
            set
            {
                if (value == _cannonType) return;
                
                _cannonType = value;
                SpawnCannon();

                if (weaponController == null) return;
                
                weaponController.firePoints.Clear();
                
                foreach (var firePoint in cannonFirePoint)
                    weaponController.firePoints.Add(firePoint);
            }
        }
        [SerializeField] private CannonType _cannonType = CannonType.Single;
        
        
        
        #region Hull Colors

            public Color hullColor
            {
                get => _hullColor;
                set
                {
                    if (value == _hullColor) return;

                    _hullColor = value;
                    hullSprite.color = value;
                }
            }
            [SerializeField] private Color _hullColor = Color.white;
            
            public Color hullAdditionalColor
            {
                get => _hullAdditionalColor;
                set
                {
                    if (value == _hullAdditionalColor) return;

                    _hullAdditionalColor = value;
                    hullAdditionalColorSprite.color = value;
                }
            }
            [SerializeField] private Color _hullAdditionalColor = Color.white;
            
            public Color hullShadowsColor
            {
                get => _hullShadowsColor;
                set
                {
                    if (value == _hullShadowsColor) return;

                    _hullShadowsColor = value;
                    hullShadowsSprite.color = value;
                }
            }
            [SerializeField] private Color _hullShadowsColor = Color.white;

        #endregion

        

        #region Cannon Colors
        
            public Color cannonBaseColor
            {
                get => _cannonBaseColor;
                set
                {
                    if (value == _cannonBaseColor) return;

                    _cannonBaseColor = value;
                    cannonBaseSprite.color = value;
                }
            }
            [SerializeField] private Color _cannonBaseColor = Color.white;
            
            public Color cannonBaseSidesColor
            {
                get => _cannonBaseSidesColor;
                set
                {
                    if (value == _cannonBaseSidesColor) return;

                    _cannonBaseSidesColor = value;
                    cannonBaseSidesSprite.color = value;
                }
            }
            [SerializeField] private Color _cannonBaseSidesColor = Color.white;
            
            public Color cannonColor
            {
                get => _cannonColor;
                set
                {
                    if (value == _cannonColor) return;

                    _cannonColor = value;
                    cannonSprite[0].color = value;
                }
            }
            [SerializeField] private Color _cannonColor = Color.white;
            
            public Color leftCannonColor
            {
                get => _leftCannonColor;
                set
                {
                    if (value == _leftCannonColor) return;

                    _leftCannonColor = value;
                    cannonSprite[0].color = value;
                }
            }
            [SerializeField] private Color _leftCannonColor = Color.white;
            
            public Color rightCannonColor
            {
                get => _rightCannonColor;
                set
                {
                    if (value == _rightCannonColor) return;

                    _rightCannonColor = value;
                    cannonSprite[1].color = value;
                }
            }
            [SerializeField] private Color _rightCannonColor = Color.white;
            
        #endregion

        
        
        #region Cannon Positions

            public Vector2 cannonRotorPosition
            {
                get => _cannonRotorPosition;
                set
                {
                    if (value == _cannonRotorPosition) return;
                    
                    _cannonRotorPosition.x = Mathf.Clamp(value.x, -3.0f, 3.0f);
                    _cannonRotorPosition.y = Mathf.Clamp(value.y, -7.0f, 5.0f);
                    cannonRotor.localPosition = _cannonRotorPosition;
                }
            }
            [SerializeField] private Vector2 _cannonRotorPosition = Vector2.zero;
        
            public float singleCannonPosition
            {
                get => _singleCannonPosition;
                set
                {
                    if (value == _singleCannonPosition || cannonHolder[0] == null) return;
                    
                    _singleCannonPosition = Mathf.Clamp(value, -2.5f, 2.5f);
                    cannonHolder[0].localPosition = cannonHolder[0].localPosition.WithX(_singleCannonPosition);
                }
            }
            [SerializeField] private float _singleCannonPosition = 0;
            
            public float leftCannonPosition
            {
                get => _leftCannonPosition;
                set
                {
                    if (value == _leftCannonPosition || cannonHolder[0] == null) return;
                    
                    _leftCannonPosition = Mathf.Clamp(value, -2.5f, 0);
                    cannonHolder[0].localPosition = cannonHolder[0].localPosition.WithX(_leftCannonPosition);
                }
            }
            [SerializeField] private float _leftCannonPosition = -1.0f;
            
            public float rightCannonPosition
            {
                get => _rightCannonPosition;
                set
                {
                    if (value == _rightCannonPosition || cannonHolder[1] == null) return;
                    
                    _rightCannonPosition = Mathf.Clamp(value, 0, 2.5f);
                    cannonHolder[1].localPosition = cannonHolder[1].localPosition.WithX(_rightCannonPosition);
                }
            }
            [SerializeField] private float _rightCannonPosition = 1.0f;
        
        #endregion

        
        
        #region Cannon Sizes

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
            
            public float singleCannonSize
            {
                get => _singleCannonSize;
                set
                {
                    if (value == _singleCannonSize || cannonHolder[0] == null) return;
                    
                    _singleCannonSize = Mathf.Clamp(value, 0.5f, 1.0f);
                    cannonHolder[0].localScale = Vector3.one * _singleCannonSize;
                }
            }
            [SerializeField] private float _singleCannonSize = 1.0f;
            
            public float leftCannonSize
            {
                get => _leftCannonSize;
                set
                {
                    if (value == _leftCannonSize || cannonHolder[0] == null) return;
                    
                    _leftCannonSize = Mathf.Clamp(value, 0.5f, 1.0f);
                    cannonHolder[0].localScale = Vector3.one * _leftCannonSize;
                }
            }
            [SerializeField] private float _leftCannonSize = 1.0f;
            
            public float rightCannonSize
            {
                get => _rightCannonSize;
                set
                {
                    if (value == _rightCannonSize || cannonHolder[1] == null) return;
                    
                    _rightCannonSize = Mathf.Clamp(value, 0.5f, 1.0f);
                    cannonHolder[1].localScale = Vector3.one * _rightCannonSize;
                }
            }
            [SerializeField] private float _rightCannonSize = 1.0f;
        
        #endregion

        #endregion

        
        
        
        #region Fields

        public bool hideSection;
        
        // Constant variable leading to the tank sprites.
        private const string Path = "TankWars/Sprites/";
        
        // Current tab that the cannon is current on. eg. position, size.        
        public int cannonCurrentTab = 0;
        
        // The sorting layer ID, used to assign the tank parts to a sorting layer.
        [HideInInspector] public int sortingLayerID = 0;
        
        // Storage of built controllers.
        public TankController tankController;
        public WeaponController weaponController;
        public CameraController cameraController;
        
        // Hull data.
        public Transform hull;
        public SpriteRenderer hullSprite;
        public SpriteRenderer hullAdditionalColorSprite;
        public SpriteRenderer hullShadowsSprite;
        public bool expandHull;
        
        // Cannon data.
        public Transform cannonRotor;
        public Transform cannonBase;
        public List<Transform> cannonHolder = new List<Transform>();
        public List<Transform> cannon = new List<Transform>();
        public SpriteRenderer cannonBaseSprite;
        public SpriteRenderer cannonBaseSidesSprite;
        public List<SpriteRenderer> cannonSprite = new List<SpriteRenderer>();
        public List<Transform> cannonFirePoint = new List<Transform>();
        public bool expandCannon;
        
        // Categories data.
        public List<Category> categories = new List<Category>();
        
        #endregion

        
        

        #region Functions

        public void SetSortingLayer()
        {
            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (var spriteRenderer in spriteRenderers)
                spriteRenderer.sortingLayerID = sortingLayerID;
        }
        
        /// <summary>
        /// Use with caution: Remove absolutely everything from the tank
        /// and deletes is all.
        /// </summary>
        public void EraseAll()
        {
            transform.DestroyChildren();
            
            foreach (var category in categories) category.accessories.Clear();
            categories.Clear();
        }

        /// <summary>
        /// Spawns the tanks hull.
        /// </summary>
        
        public void SpawnHull()
        {
            
            // Create the tanks hull
            // (the game object that holds all elements attached to the hull).
            var hullParent = transform.Find("Hull");
            if (hullParent == null)
            {
                hullParent = new GameObject().transform;
                hullParent.name = "Hull";
                hullParent.parent = transform;
            }
            
            // Reset the hulls position to zero.
            hullParent.localPosition = Vector3.zero;
            hullParent.localRotation = Quaternion.identity;
            hullParent.localScale = Vector2.one;
            
            
            
            // Destroy the original hull.
            if(hull != null) DestroyImmediate(hull.gameObject);
            
            // Create a new hull for the tank as a
            // child of the parent hull (holds all hull elements).
            hull = new GameObject().transform;
            hull.name = "Hull";
            hull.parent = hullParent;
            hull.localPosition = Vector3.zero;
            hull.localRotation = Quaternion.identity;
            hull.localScale = Vector2.one;

            // Add a sprite renderer to the hull game object and set it layer.
            hullSprite = hull.gameObject.AddComponent<SpriteRenderer>();
            hullSprite.sprite = Resources.Load<Sprite>(Path + "Hull/Hull");
            hullSprite.sortingLayerID = sortingLayerID;
            hullSprite.color = hullColor;
            
            // Create a polygon collider.
            hull.gameObject.AddComponent<PolygonCollider2D>();



            // Add a additional coloring object as a child of the (child) hull.
            var hullAdditionalColoring = new GameObject().transform;
            hullAdditionalColoring.name = "Additional Color";
            hullAdditionalColoring.parent = hull;
            hullAdditionalColoring.localPosition = Vector3.zero;
            hullAdditionalColoring.localRotation = Quaternion.identity;
            hullAdditionalColoring.localScale = Vector2.one;
            
            // Again, add a sprite renderer to this object, set its layer/sorting order and the color.
            hullAdditionalColorSprite = hullAdditionalColoring.gameObject.AddComponent<SpriteRenderer>();
            hullAdditionalColorSprite.sprite = Resources.Load<Sprite>(Path + "Hull/Additional Color");
            hullAdditionalColorSprite.sortingLayerID = sortingLayerID;
            hullAdditionalColorSprite.sortingOrder = 1;
            hullAdditionalColorSprite.color = hullAdditionalColor;
            
            
            
            // Add shadows to the tank, following the above process.
            var hullShadows = new GameObject().transform;
            hullShadows.name = "Shadows";
            hullShadows.parent = hull;
            hullShadows.localPosition = Vector3.zero;
            hullShadows.localRotation = Quaternion.identity;
            hullShadows.localScale = Vector2.one;
         
            // Add the shadow sprite renderer.
            hullShadowsSprite = hullShadows.gameObject.AddComponent<SpriteRenderer>();
            hullShadowsSprite.sprite = Resources.Load<Sprite>(Path + "Hull/Shadows");
            hullShadowsSprite.sortingLayerID = sortingLayerID;
            hullShadowsSprite.sortingOrder = 1;
            hullShadowsSprite.color = hullShadowsColor;
        }
        
        /// <summary>
        /// Spawns the tanks cannon.
        /// </summary>
        
        public void SpawnCannon()
        {
            // Clear all cannon data.
            cannonHolder.Clear();
            cannon.Clear();
            cannonSprite.Clear();
            cannonFirePoint.Clear();

            // Create the cannon rotor
            // (the game object that holds all elements attached to the cannon).
            if (cannonRotor == null)
            {
                cannonRotor = new GameObject().transform;
                cannonRotor.name = "Cannon";
                cannonRotor.parent = transform;
                cannonRotor.localPosition = cannonRotorPosition;
                cannonRotor.localRotation = Quaternion.identity;
                cannonRotor.localScale = Vector2.one * cannonRotorSize;
            } 
            
            
            
            // Destroy the original base.
            if(cannonBase != null) DestroyImmediate(cannonBase.gameObject);
            
            // Create a new base for the cannon as a
            // child of the rotor.
            cannonBase = new GameObject().transform;
            cannonBase.name = "Base";
            cannonBase.parent = cannonRotor;
            cannonBase.localPosition = Vector3.zero;
            cannonBase.localRotation = Quaternion.identity;
            cannonBase.localScale = Vector2.one;

            // Add a sprite renderer to the base.
            cannonBaseSprite = cannonBase.gameObject.AddComponent<SpriteRenderer>();
            cannonBaseSprite.sprite = Resources.Load<Sprite>(Path + "Cannon/Base");
            cannonBaseSprite.sortingLayerID = sortingLayerID;
            cannonBaseSprite.sortingOrder = 11;
            cannonBaseSprite.color = cannonBaseColor;
            
            
            
            // Create a child object of the base which acts as the
            // coloring to the bases sides.
            var baseSides = new GameObject().transform;
            baseSides.name = "BaseSides";
            baseSides.parent = cannonBase;
            baseSides.localPosition = Vector3.zero;
            baseSides.localRotation = Quaternion.identity;
            baseSides.localScale = Vector2.one;
            
            // Again add a sprite to this object.
            cannonBaseSidesSprite = baseSides.gameObject.AddComponent<SpriteRenderer>();
            cannonBaseSidesSprite.sprite = Resources.Load<Sprite>(Path + "Cannon/BaseSides");
            cannonBaseSidesSprite.sortingLayerID = sortingLayerID;
            cannonBaseSidesSprite.sortingOrder = 10;
            cannonBaseSidesSprite.color = cannonBaseSidesColor;
            
            

            // Depending on whether the tank is a single or double draw however many cannons.
            var single = cannonType == CannonType.Single;

            for (var i = 0; i < 2; i++)
            {
                if (single && i > 0) break;
                
                // Create a holder for the cannon.
                cannonHolder.Add(new GameObject().transform);
                cannonHolder[i].name = single ? "Cannon" : i == 0 ? "Left Cannon" : "Right Cannon";
                cannonHolder[i].parent = cannonBase;
                cannonHolder[i].localPosition = Vector3.zero;
                cannonHolder[i].localRotation = Quaternion.identity;
                
                // Create the cannon and parent it to the holder.
                cannon.Add(new GameObject().transform);
                cannon[i].name = "CannonSprite";
                cannon[i].parent = cannonHolder[i];
                cannon[i].localPosition = Vector3.zero.WithY(8.195f);
                cannon[i].localRotation = Quaternion.identity;
                
                cannonHolder[i].localPosition = new Vector3(0, 4.2f, 0);
                
                cannonSprite.Add(cannon[i].gameObject.AddComponent<SpriteRenderer>());
                cannonSprite[i].sprite = Resources.Load<Sprite>(Path + "Cannon/Cannon");
                cannonSprite[i].sortingLayerID = sortingLayerID;
                cannonSprite[i].sortingOrder = 12;
                
                // Get or create a polygon collider.
                cannon[i].gameObject.AddComponent<PolygonCollider2D>();
                
                
                
                cannonFirePoint.Add(new GameObject().transform);
                cannonFirePoint[i].name = "FirePoint";
                cannonFirePoint[i].parent = cannonHolder[i];
                cannonFirePoint[i].localPosition = Vector3.zero.WithY(8.195f * 2.0f);
                cannonFirePoint[i].localRotation = Quaternion.identity;
            }

            // Reposition and scale the cannons.
            if (single)
            {
                cannonHolder[0].localPosition = cannonHolder[0].localPosition.WithX(singleCannonPosition);
                cannonHolder[0].localScale = Vector3.one * singleCannonSize;
                cannonSprite[0].color = cannonColor;
            }
            else
            {
                cannonHolder[0].localPosition = cannonHolder[0].localPosition.WithX(leftCannonPosition);
                cannonHolder[0].localScale = Vector3.one * leftCannonSize;
                cannonSprite[0].color = leftCannonColor;
                
                cannonHolder[1].localPosition = cannonHolder[1].localPosition.WithX(rightCannonPosition);
                cannonHolder[1].localScale = Vector3.one * rightCannonSize;
                cannonSprite[1].color = rightCannonColor;
            }
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="nameOfCategory">Name of the category.</param>
        /// <param name="folderLocation">Origin folder - where the sprites are located in the resources.</param>
        
        public void AddCategory(string nameOfCategory, string folderLocation)
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
                
                var parent = grandParent.Find(nameOfCategory);

                if (parent != null) continue;
                
                parent = new GameObject().transform;
                parent.name = nameOfCategory;
                parent.parent = grandParent;
                parent.localPosition = Vector3.zero;
                parent.localRotation = Quaternion.identity;
                parent.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Removes a new category.
        /// </summary>
        /// <param name="index">The index of the category.</param>
        
        public void RemoveCategory(int index)
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
        public void SpawnAccessory(int index, string categoryName, string projectFolder)
        {
            var parentHull = transform.Find("Hull");
            if (parentHull == null) return;
            
            var parent = parentHull.Find(categoryName);
                
            if (parent == null)
            {
                parent = new GameObject().transform;
                parent.name = categoryName;
                parent.parent = parentHull;
                parent.localPosition = Vector3.zero;
                parent.localRotation = Quaternion.identity;
                parent.localScale = Vector3.one;
            }
            
            categories[index].accessories.Add(new Accessory(parent, projectFolder, sortingLayerID));
        }

        /// <summary>
        /// Creates an exact copy of the input accessory.
        /// </summary>
        /// <param name="categoryIndex">The index of the category to add the new accessory to.</param>
        /// <param name="accessory">The accessory to be copied.</param>
        
        public void CopyAccessory(int categoryIndex, Accessory accessory) => 
            categories[categoryIndex].accessories.Add(new Accessory(accessory));

        /// <summary>
        /// Adds all movement components to the tanks game object.
        /// </summary>
        
        public void AddMovementSystem()
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
            }
            
            // Rename the tank.
            transform.name = "Player Tank";
            
            print("Tank Builder: Movement system added.");
        }
        
        /// <summary>
        /// Adds all movement components to the tanks game object.
        /// </summary>
        
        public void AddWeaponSystem()
        {
            if (hull == null || cannonRotor == null) return;
            
            weaponController = GetComponent<WeaponController>();
            if(weaponController != null) DestroyImmediate(weaponController);

            weaponController = gameObject.AddComponent<WeaponController>();
            
            foreach (var firePoint in cannonFirePoint)
                weaponController.firePoints.Add(firePoint);
            
            
            
            print("Tank Builder: Weapon system added.");
        }
        
        /// <summary>
        /// Adds all camera system to the hierarchy.
        /// </summary>
        
        public void AddCameraSystem()
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
            
            if(cameraController.CameraTransform != null) print("Tank Builder: Camera system added.");
        }
        
        #endregion
        
    }
}
