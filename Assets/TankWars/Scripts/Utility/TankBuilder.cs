using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public class Vent
        {
            #region Fields

            public Transform vent;
            public SpriteRenderer ventSprite;
            
            // Unity Editor Fields
            public int currentTab = 0;
            public bool expand;
            public bool editName;

            #endregion

            
            
            #region Properties

            public int currentParent
            {
                get => _currentParent;
                set
                {
                    if (_currentParent == value) return;
                    _currentParent = value;
                    
                    if (vent.parent.name == (currentParent == 0 ? "Cannon" : "Hull")) return;

                    var grParent = vent.parent.parent.parent.Find(currentParent == 0 ? "Hull" : "Cannon");
                    var parent = grParent.Find("Vents");
                
                    if (parent == null)
                    {
                        parent = new GameObject().transform;
                        parent.name = "Vents";
                        parent.parent = grParent;
                        parent.localPosition = Vector3.zero;
                        parent.localRotation = Quaternion.identity;
                    }
                
                    vent.parent = grParent.Find("Vents");
                }
            }
            [SerializeField] private int _currentParent = 0;
            
            public string ventName
            {
                get => _ventName;
                set
                {
                    if (value == _ventName) return;

                    _ventName = value;
                    vent.name = _ventName;
                }
            }
            [SerializeField] private string _ventName = "Vent";
            
            public Vector2 ventPosition
            {
                get => _ventPosition;
                set
                {
                    if (value == _ventPosition) return;
                    
                    _ventPosition.x = Mathf.Clamp(value.x, -10.0f, 10.0f);
                    _ventPosition.y = Mathf.Clamp(value.y, -13.0f, 11.4f);
                    vent.localPosition = _ventPosition;
                }
            }
            [SerializeField] private Vector2 _ventPosition = Vector2.zero;
            
            public float ventRotation
            {
                get => _ventRotation;
                set
                {
                    if (value == _ventRotation) return;
                    
                    _ventRotation = Mathf.Clamp(value, -180.0f, 180.0f);
                    vent.localEulerAngles = vent.localEulerAngles.WithZ(_ventRotation);
                }
            }
            [SerializeField] private float _ventRotation = 0;
            
            public Vector2 ventScale
            {
                get => _ventScale;
                set
                {
                    if (value == _ventScale) return;
                    
                    _ventScale.x = Mathf.Clamp(value.x, 0.0f, 2.0f);
                    _ventScale.y = Mathf.Clamp(value.y, 0.0f, 2.0f);
                    vent.localScale = _ventScale;
                }
            }
            [SerializeField] private Vector2 _ventScale = Vector2.one;
            
            public Color ventColor
            {
                get => _ventColor;
                set
                {
                    if (value == _ventColor) return;

                    _ventColor = value;
                    ventSprite.color = value;
                }
            }
            [SerializeField] private Color _ventColor = Color.white;

            public int sortingOrder
            {
                get => _sortingOrder;
                set
                {
                    if (value == _sortingOrder) return;

                    _sortingOrder = value;
                    ventSprite.sortingOrder = _sortingOrder;
                }
            }
            [SerializeField] private int _sortingOrder = 0;

            #endregion

            
            
            #region Functions

            public Vent(Transform parent, int sortingLayerID)
            {
                // Vent
                vent = new GameObject().transform;
                vent.name = ventName;
                vent.parent = parent;
                vent.localPosition = Vector3.zero;
                vent.localRotation = Quaternion.identity;

                ventSprite = vent.gameObject.AddComponent<SpriteRenderer>();
                ventSprite.sprite = Resources.Load<Sprite>(path + "Vent");
                ventSprite.sortingLayerID = sortingLayerID;            
                ventSprite.sortingOrder = sortingOrder;
                ventSprite.color = ventColor;
            }
            
            public Vent(Vent copy)
            {
                // Vent
                vent = new GameObject().transform;
                ventName = copy.ventName + " copy";
                vent.parent = copy.vent.parent;
                currentParent = copy.currentParent;
                ventPosition = copy.vent.localPosition;
                ventRotation = copy.ventRotation;
                ventScale = copy.ventScale;

                // Copy the sprite renderer.
                var copySprite = copy.ventSprite;
                ventSprite = vent.gameObject.AddComponent<SpriteRenderer>();
                ventSprite.sprite = copySprite.sprite;
                ventSprite.sortingLayerID = copySprite.sortingLayerID;            
                sortingOrder = copySprite.sortingOrder;
                ventColor = copySprite.color;
            }
            
            #endregion

        }
        
        
        [Serializable]
        public class Compartment
        {
            #region Fields

            public Transform compartment;
            public SpriteRenderer compartmentSprite;
            
            // Unity Editor Fields
            public int currentTab = 0;
            public bool expand;
            public bool editName;

            #endregion

            
            
            #region Properties

            public int compartmentStyle
            {
                get => _compartmentStyle;
                set
                {
                    if (_compartmentStyle == value) return;
                    _compartmentStyle = value;
                    
                    compartmentSprite.sprite = Resources.LoadAll<Sprite>(path + "Compartments")[_compartmentStyle];
                }
            }
            [SerializeField] private int _compartmentStyle = 0;
            
            public int currentParent
            {
                get => _currentParent;
                set
                {
                    if (_currentParent == value) return;
                    _currentParent = value;
                    
                    if (compartment.parent.name == (currentParent == 0 ? "Cannon" : "Hull")) return;

                    var grParent = compartment.parent.parent.parent.Find(currentParent == 0 ? "Hull" : "Cannon");
                    var parent = grParent.Find("Compartments");
                
                    if (parent == null)
                    {
                        parent = new GameObject().transform;
                        parent.name = "Compartments";
                        parent.parent = grParent;
                        parent.localPosition = Vector3.zero;
                        parent.localRotation = Quaternion.identity;
                    }
                
                    compartment.parent = grParent.Find("Compartments");
                }
            }
            [SerializeField] private int _currentParent = 0;
            
            public string compartmentName
            {
                get => _compartmentName;
                set
                {
                    if (value == _compartmentName) return;

                    _compartmentName = value;
                    compartment.name = _compartmentName;
                }
            }
            [SerializeField] private string _compartmentName = "Compartment";
            
            public Vector2 compartmentPosition
            {
                get => _compartmentPosition;
                set
                {
                    if (value == _compartmentPosition) return;
                    
                    _compartmentPosition.x = Mathf.Clamp(value.x, -10.0f, 10.0f);
                    _compartmentPosition.y = Mathf.Clamp(value.y, -13.0f, 11.4f);
                    compartment.localPosition = _compartmentPosition;
                }
            }
            [SerializeField] private Vector2 _compartmentPosition = Vector2.zero;
            
            public float compartmentRotation
            {
                get => _compartmentRotation;
                set
                {
                    if (value == _compartmentRotation) return;
                    
                    _compartmentRotation = Mathf.Clamp(value, -180.0f, 180.0f);
                    compartment.localEulerAngles = compartment.localEulerAngles.WithZ(_compartmentRotation);
                }
            }
            [SerializeField] private float _compartmentRotation = 0;
            
            public Vector2 compartmentScale
            {
                get => _compartmentScale;
                set
                {
                    if (value == _compartmentScale) return;
                    
                    _compartmentScale.x = Mathf.Clamp(value.x, -2.0f, 2.0f);
                    _compartmentScale.y = Mathf.Clamp(value.y, -2.0f, 2.0f);
                    compartment.localScale = _compartmentScale;
                }
            }
            [SerializeField] private Vector2 _compartmentScale = Vector2.one;
            
            public Color compartmentColor
            {
                get => _compartmentColor;
                set
                {
                    if (value == _compartmentColor) return;

                    _compartmentColor = value;
                    compartmentSprite.color = value;
                }
            }
            [SerializeField] private Color _compartmentColor = Color.white;

            public int sortingOrder
            {
                get => _sortingOrder;
                set
                {
                    if (value == _sortingOrder) return;

                    _sortingOrder = value;
                    compartmentSprite.sortingOrder = _sortingOrder;
                }
            }
            [SerializeField] private int _sortingOrder = 0;

            #endregion

            
            
            #region Functions

            public Compartment(Transform parent, int sortingLayerID)
            {
                // compartment
                compartment = new GameObject().transform;
                compartment.name = compartmentName;
                compartment.parent = parent;
                compartment.localPosition = Vector3.zero;
                compartment.localRotation = Quaternion.identity;

                compartmentSprite = compartment.gameObject.AddComponent<SpriteRenderer>();
                compartmentSprite.sprite = Resources.LoadAll<Sprite>(path + "Compartments")[0];
                compartmentSprite.sortingLayerID = sortingLayerID;            
                compartmentSprite.sortingOrder = sortingOrder;
                compartmentSprite.color = compartmentColor;
            }
            
            public Compartment(Compartment copy)
            {
                // compartment
                compartment = new GameObject().transform;
                compartmentName = copy.compartmentName + " copy";
                compartment.parent = copy.compartment.parent;
                currentParent = copy.currentParent;
                compartmentPosition = copy.compartment.localPosition;
                compartmentRotation = copy.compartmentRotation;
                compartmentScale = copy.compartmentScale;

                // Copy the sprite renderer.
                var copySprite = copy.compartmentSprite;
                compartmentSprite = compartment.gameObject.AddComponent<SpriteRenderer>();
                compartmentSprite.sprite = copySprite.sprite;
                compartmentSprite.sortingLayerID = copySprite.sortingLayerID;            
                sortingOrder = copySprite.sortingOrder;
                compartmentColor = copySprite.color;
                
                compartmentStyle = copy.compartmentStyle;
            }
            
            #endregion

        }

        
        
        [Serializable]
        public class Box
        {
            #region Fields

            public Transform box;
            public SpriteRenderer boxSprite;
            
            // Unity Editor Fields
            public int currentTab = 0;
            public bool expand;
            public bool editName;

            #endregion

            
            
            #region Properties

            public int boxStyle
            {
                get => _boxStyle;
                set
                {
                    if (_boxStyle == value) return;
                    _boxStyle = value;
                    
                    boxSprite.sprite = Resources.LoadAll<Sprite>(path + "Boxes")[_boxStyle];
                }
            }
            [SerializeField] private int _boxStyle = 0;
            
            public int currentParent
            {
                get => _currentParent;
                set
                {
                    if (_currentParent == value) return;
                    _currentParent = value;
                    
                    if (box.parent.name == (currentParent == 0 ? "Cannon" : "Hull")) return;

                    var grParent = box.parent.parent.parent.Find(currentParent == 0 ? "Hull" : "Cannon");
                    var parent = grParent.Find("Boxes");
                
                    if (parent == null)
                    {
                        parent = new GameObject().transform;
                        parent.name = "Boxes";
                        parent.parent = grParent;
                        parent.localPosition = Vector3.zero;
                        parent.localRotation = Quaternion.identity;
                    }
                
                    box.parent = grParent.Find("Boxes");
                }
            }
            [SerializeField] private int _currentParent = 0;
            
            public string boxName
            {
                get => _boxName;
                set
                {
                    if (value == _boxName) return;

                    _boxName = value;
                    box.name = _boxName;
                }
            }
            [SerializeField] private string _boxName = "Box";
            
            public Vector2 boxPosition
            {
                get => _boxPosition;
                set
                {
                    if (value == _boxPosition) return;
                    
                    _boxPosition.x = Mathf.Clamp(value.x, -10.0f, 10.0f);
                    _boxPosition.y = Mathf.Clamp(value.y, -13.0f, 11.4f);
                    box.localPosition = _boxPosition;
                }
            }
            [SerializeField] private Vector2 _boxPosition = Vector2.zero;
            
            public float boxRotation
            {
                get => _boxRotation;
                set
                {
                    if (value == _boxRotation) return;
                    
                    _boxRotation = Mathf.Clamp(value, -180.0f, 180.0f);
                    box.localEulerAngles = box.localEulerAngles.WithZ(_boxRotation);
                }
            }
            [SerializeField] private float _boxRotation = 0;
            
            public Vector2 boxScale
            {
                get => _boxScale;
                set
                {
                    if (value == _boxScale) return;
                    
                    _boxScale.x = Mathf.Clamp(value.x, -2.0f, 2.0f);
                    _boxScale.y = Mathf.Clamp(value.y, -2.0f, 2.0f);
                    box.localScale = _boxScale;
                }
            }
            [SerializeField] private Vector2 _boxScale = Vector2.one;
            
            public Color boxColor
            {
                get => _boxColor;
                set
                {
                    if (value == _boxColor) return;

                    _boxColor = value;
                    boxSprite.color = value;
                }
            }
            [SerializeField] private Color _boxColor = Color.white;

            public int sortingOrder
            {
                get => _sortingOrder;
                set
                {
                    if (value == _sortingOrder) return;

                    _sortingOrder = value;
                    boxSprite.sortingOrder = _sortingOrder;
                }
            }
            [SerializeField] private int _sortingOrder = 0;

            #endregion

            
            
            #region Functions

            public Box(Transform parent, int sortingLayerID)
            {
                // box
                box = new GameObject().transform;
                box.name = boxName;
                box.parent = parent;
                box.localPosition = Vector3.zero;
                box.localRotation = Quaternion.identity;

                boxSprite = box.gameObject.AddComponent<SpriteRenderer>();
                boxSprite.sprite = Resources.LoadAll<Sprite>(path + "Boxes")[0];
                boxSprite.sortingLayerID = sortingLayerID;            
                boxSprite.sortingOrder = sortingOrder;
                boxSprite.color = boxColor;
            }
            
            public Box(Box copy)
            {
                // box
                box = new GameObject().transform;
                boxName = copy.boxName + " copy";
                box.parent = copy.box.parent;
                currentParent = copy.currentParent;
                boxPosition = copy.box.localPosition;
                boxRotation = copy.boxRotation;
                boxScale = copy.boxScale;

                // Copy the sprite renderer.
                var copySprite = copy.boxSprite;
                boxSprite = box.gameObject.AddComponent<SpriteRenderer>();
                boxSprite.sprite = copySprite.sprite;
                boxSprite.sortingLayerID = copySprite.sortingLayerID;            
                sortingOrder = copySprite.sortingOrder;
                boxColor = copySprite.color;
                
                boxStyle = copy.boxStyle;
            }
            
            #endregion

        }
        
        [Serializable]
        public class Extra
        {
            #region Fields

            public Transform extra;
            public SpriteRenderer extraSprite;
            
            // Unity Editor Fields
            public int currentTab = 0;
            public bool expand;
            public bool editName;

            #endregion

            
            
            #region Properties

            public int extraStyle
            {
                get => _extraStyle;
                set
                {
                    if (_extraStyle == value) return;
                    _extraStyle = value;
                    
                    extraSprite.sprite = Resources.LoadAll<Sprite>(path + "Extras")[_extraStyle];
                }
            }
            [SerializeField] private int _extraStyle = 0;
            
            public int currentParent
            {
                get => _currentParent;
                set
                {
                    if (_currentParent == value) return;
                    _currentParent = value;
                    
                    if (extra.parent.name == (currentParent == 0 ? "Cannon" : "Hull")) return;

                    var grParent = extra.parent.parent.parent.Find(currentParent == 0 ? "Hull" : "Cannon");
                    var parent = grParent.Find("Extras");
                
                    if (parent == null)
                    {
                        parent = new GameObject().transform;
                        parent.name = "Extras";
                        parent.parent = grParent;
                        parent.localPosition = Vector3.zero;
                        parent.localRotation = Quaternion.identity;
                    }
                
                    extra.parent = grParent.Find("Extras");
                }
            }
            [SerializeField] private int _currentParent = 0;
            
            public string extraName
            {
                get => _extraName;
                set
                {
                    if (value == _extraName) return;

                    _extraName = value;
                    extra.name = _extraName;
                }
            }
            [SerializeField] private string _extraName = "Box";
            
            public Vector2 extraPosition
            {
                get => _extraPosition;
                set
                {
                    if (value == _extraPosition) return;
                    
                    _extraPosition.x = Mathf.Clamp(value.x, -10.0f, 10.0f);
                    _extraPosition.y = Mathf.Clamp(value.y, -13.0f, 11.4f);
                    extra.localPosition = _extraPosition;
                }
            }
            [SerializeField] private Vector2 _extraPosition = Vector2.zero;
            
            public float extraRotation
            {
                get => _extraRotation;
                set
                {
                    if (value == _extraRotation) return;
                    
                    _extraRotation = Mathf.Clamp(value, -180.0f, 180.0f);
                    extra.localEulerAngles = extra.localEulerAngles.WithZ(_extraRotation);
                }
            }
            [SerializeField] private float _extraRotation = 0;
            
            public Vector2 extraScale
            {
                get => _extraScale;
                set
                {
                    if (value == _extraScale) return;
                    
                    _extraScale.x = Mathf.Clamp(value.x, -2.0f, 2.0f);
                    _extraScale.y = Mathf.Clamp(value.y, -2.0f, 2.0f);
                    extra.localScale = _extraScale;
                }
            }
            [SerializeField] private Vector2 _extraScale = Vector2.one;
            
            public Color extraColor
            {
                get => _extraColor;
                set
                {
                    if (value == _extraColor) return;

                    _extraColor = value;
                    extraSprite.color = value;
                }
            }
            [SerializeField] private Color _extraColor = Color.white;

            public int sortingOrder
            {
                get => _sortingOrder;
                set
                {
                    if (value == _sortingOrder) return;

                    _sortingOrder = value;
                    extraSprite.sortingOrder = _sortingOrder;
                }
            }
            [SerializeField] private int _sortingOrder = 0;

            #endregion

            
            
            #region Functions

            public Extra(Transform parent, int sortingLayerID)
            {
                // extra
                extra = new GameObject().transform;
                extra.name = extraName;
                extra.parent = parent;
                extra.localPosition = Vector3.zero;
                extra.localRotation = Quaternion.identity;

                extraSprite = extra.gameObject.AddComponent<SpriteRenderer>();
                extraSprite.sprite = Resources.LoadAll<Sprite>(path + "Extras")[0];
                extraSprite.sortingLayerID = sortingLayerID;            
                extraSprite.sortingOrder = sortingOrder;
                extraSprite.color = extraColor;
            }
            
            public Extra(Extra copy)
            {
                // extra
                extra = new GameObject().transform;
                extraName = copy.extraName + " copy";
                extra.parent = copy.extra.parent;
                currentParent = copy.currentParent;
                extraPosition = copy.extra.localPosition;
                extraRotation = copy.extraRotation;
                extraScale = copy.extraScale;

                // Copy the sprite renderer.
                var copySprite = copy.extraSprite;
                extraSprite = extra.gameObject.AddComponent<SpriteRenderer>();
                extraSprite.sprite = copySprite.sprite;
                extraSprite.sortingLayerID = copySprite.sortingLayerID;            
                sortingOrder = copySprite.sortingOrder;
                extraColor = copySprite.color;
                
                extraStyle = copy.extraStyle;
            }
            
            #endregion

        }

        [Serializable]
        public class Category
        {
            public string categoryName;
            public string categoryFolder;
            public bool expandCategory;
            public List<Accessory> accessories;
        }
        
        [Serializable]
        public class Accessory
        {
            #region Fields

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
                    
                    accessorySprite.sprite = Resources.LoadAll<Sprite>(path + projectFolder)[style];
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
                    position.y = Mathf.Clamp(value.y, -13.0f, 11.4f);
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

                // Sprite
                accessorySprite = transform.gameObject.AddComponent<SpriteRenderer>();
                accessorySprite.sprite = Resources.LoadAll<Sprite>(path + projectFolder)[0];
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

        /// <summary>
        /// The sorting layer ID, used to assign the tank parts to a sorting layer.
        /// </summary>

        public bool expandHull;
        public bool expandCannon;
        public bool expandVents;
        public bool expandCompartments;
        public bool expandBoxes;
        public bool expandExtras;

        public int cannonCurrentTab = 0;
        
        [HideInInspector] public int sortingLayerID = 0;
        private const string path = "TankWars/Sprites/";
        
        public Transform hull;
        public SpriteRenderer hullSprite;
        public SpriteRenderer hullAdditionalColorSprite;
        public SpriteRenderer hullShadowsSprite;
        
        public Transform cannonRotor;
        public Transform cannonBase;
        public List<Transform> cannonHolder = new List<Transform>();
        public List<Transform> cannon = new List<Transform>();
        public SpriteRenderer cannonBaseSprite;
        public SpriteRenderer cannonBaseSidesSprite;
        public List<SpriteRenderer> cannonSprite = new List<SpriteRenderer>();
        
        public List<Vent> vents = new List<Vent>();
        public List<Compartment> compartments = new List<Compartment>();
        public List<Box> boxes = new List<Box>();
        public List<Extra> extras = new List<Extra>();

        public List<Category> categories = new List<Category>();
        
        #endregion

        

        #region Functions

        public void SpawnTank()
        {
            SpawnHull();
            SpawnCannon();
        }
        
        public void RemoveAllCategories()
        {
            print(categories.Count);

            foreach (var category in categories) category.accessories.Clear();
            categories.Clear();
        }
        
        public void EraseAll()
        {
            transform.DestroyChildren();
            
            foreach (var category in categories) category.accessories.Clear();
            categories.Clear();
        }

        public void SpawnHull()
        {
            var hullParent = transform.Find("Hull");
            if (hullParent == null)
            {
                hullParent = new GameObject().transform;
                hullParent.name = "Hull";
                hullParent.parent = transform;
            }
            
            hullParent.localPosition = Vector3.zero;
            hullParent.localRotation = Quaternion.identity;
            
            // Hull
            hull = new GameObject().transform;
            hull.name = "Hull";
            hull.parent = hullParent;
            hull.localPosition = Vector3.zero;
            hull.localRotation = Quaternion.identity;

            hullSprite = hull.gameObject.AddComponent<SpriteRenderer>();
            hullSprite.sprite = Resources.Load<Sprite>(path + "Hull/Hull");
            hullSprite.sortingLayerID = sortingLayerID;
            hullSprite.color = hullColor;
            
            
            
            // Additional Coloring
            var hullAdditionalColoring = new GameObject().transform;
            hullAdditionalColoring.name = "Additional Color";
            hullAdditionalColoring.parent = hull;
            hullAdditionalColoring.localPosition = Vector3.zero;
            hullAdditionalColoring.localRotation = Quaternion.identity;
            
            hullAdditionalColorSprite = hullAdditionalColoring.gameObject.AddComponent<SpriteRenderer>();
            hullAdditionalColorSprite.sprite = Resources.Load<Sprite>(path + "Hull/Additional Color");
            hullAdditionalColorSprite.sortingLayerID = sortingLayerID;
            hullAdditionalColorSprite.sortingOrder = 1;
            hullAdditionalColorSprite.color = hullAdditionalColor;
            
            
            
            // Shadows Coloring
            var hullShadows = new GameObject().transform;
            hullShadows.name = "Shadows";
            hullShadows.parent = hull;
            hullShadows.localPosition = Vector3.zero;
            hullShadows.localRotation = Quaternion.identity;
            
            hullShadowsSprite = hullShadows.gameObject.AddComponent<SpriteRenderer>();
            hullShadowsSprite.sprite = Resources.Load<Sprite>(path + "Hull/Shadows");
            hullShadowsSprite.sortingLayerID = sortingLayerID;
            hullShadowsSprite.sortingOrder = 1;
            hullShadowsSprite.color = hullShadowsColor;
        }
        
        public void SpawnCannon()
        {
            cannonHolder.Clear();
            cannon.Clear();
            cannonSprite.Clear();

            // Create the cannon rotor.
            if (cannonRotor == null)
            {
                cannonRotor = new GameObject().transform;
                cannonRotor.name = "Cannon";
                cannonRotor.parent = transform;
                cannonRotor.localPosition = cannonRotorPosition;
                cannonRotor.localRotation = Quaternion.identity;
                cannonRotor.localScale = Vector2.one * cannonRotorSize;
            } 
            
            
            // Base.            
            cannonBase = new GameObject().transform;
            cannonBase.name = "Base";
            cannonBase.parent = cannonRotor;
            cannonBase.localPosition = Vector3.zero;
            cannonBase.localRotation = Quaternion.identity;

            cannonBaseSprite = cannonBase.gameObject.AddComponent<SpriteRenderer>();
            cannonBaseSprite.sprite = Resources.Load<Sprite>(path + "Cannon/Base");
            cannonBaseSprite.sortingLayerID = sortingLayerID;
            cannonBaseSprite.sortingOrder = 11;
            cannonBaseSprite.color = cannonBaseColor;
            
            
            
            // Base Sides Coloring.
            var baseSides = new GameObject().transform;
            baseSides.name = "BaseSides";
            baseSides.parent = cannonBase;
            baseSides.localPosition = Vector3.zero;
            baseSides.localRotation = Quaternion.identity;
            
            cannonBaseSidesSprite = baseSides.gameObject.AddComponent<SpriteRenderer>();
            cannonBaseSidesSprite.sprite = Resources.Load<Sprite>(path + "Cannon/BaseSides");
            cannonBaseSidesSprite.sortingLayerID = sortingLayerID;
            cannonBaseSidesSprite.sortingOrder = 10;
            cannonBaseSidesSprite.color = cannonBaseSidesColor;
            
            

            var single = cannonType == CannonType.Single;

            for (var i = 0; i < 2; i++)
            {
                if (single && i > 0) break;
                
                // Create a holder for the cannon.
                cannonHolder.Add(new GameObject().transform);
                cannonHolder[i].name = single ? "Cannon" : i == 0 ? "Left Cannon" : "Right Cannon";
                cannonHolder[i].parent = cannonBase;
                cannonHolder[i].localPosition = new Vector3(0, 0, 0);
                cannonHolder[i].localRotation = Quaternion.identity;
                
                // Create the cannon and parent it to the holder.
                cannon.Add(new GameObject().transform);
                cannon[i].name = "CannonSprite";
                cannon[i].parent = cannonHolder[i];
                cannon[i].localPosition = new Vector3(0, 8.195f, 0);
                cannon[i].localRotation = Quaternion.identity;
                
                cannonHolder[i].localPosition = new Vector3(0, 4.2f, 0);
                
                cannonSprite.Add(cannon[i].gameObject.AddComponent<SpriteRenderer>());
                cannonSprite[i].sprite = Resources.Load<Sprite>(path + "Cannon/Cannon");
                cannonSprite[i].sortingLayerID = sortingLayerID;
                cannonSprite[i].sortingOrder = 12;
            }

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
                expandCategory = false
            };
            categories.Add(category);

            for (var i = 0; i < 2; i++)
            {
                var parentHull = transform.Find(i == 0 ? "Hull" : "Cannon");
                if (parentHull == null) return;
                
                var parent = parentHull.Find(nameOfCategory);

                if (parent != null) continue;
                
                parent = new GameObject().transform;
                parent.name = nameOfCategory;
                parent.parent = parentHull;
                parent.localPosition = Vector3.zero;
                parent.localRotation = Quaternion.identity;
            }
            
            print(categories.Count);
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
        
        
        
        public void CopyVent(Vent vent) => vents.Add(new Vent(vent));
        public void CopyCompartment(Compartment compartment) => compartments.Add(new Compartment(compartment));
        public void CopyBox(Box box) => boxes.Add(new Box(box));
        public void CopyBox(Extra extra) => extras.Add(new Extra(extra));
        
        #endregion
        
    }
}
