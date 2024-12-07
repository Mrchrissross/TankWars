using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace DimensionalDeveloper.TankBuilder.Utility
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Tank Wars/New Weapon", order = 1)]
    public class Weapon : ScriptableObject
    {
        
        
        #region Properties

        #if UNITY_EDITOR
        
        /// <summary>
        /// The name of the weapon.
        /// </summary>
        
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                
                _name = value;
                
                var assetPath =  AssetDatabase.GetAssetPath(GetInstanceID());
                AssetDatabase.RenameAsset(assetPath, value);
                AssetDatabase.SaveAssets();
            }
        } 
        [Tooltip("The name of the weapon.")]
        [Space, SerializeField] private string _name = "New Weapon";

        #endif
        
        /// <summary>
        /// The asset to spawn from asset manager (Ensure this matches the settings in the Asset Manager).
        /// </summary>
            
        public string Asset
        {
            get => asset;
            set => asset = value;
        }
        [Tooltip("The asset to spawn from asset manager (Ensure this matches the settings in the Asset Manager).")]
        [SerializeField] private string asset;
        [HideInInspector] public int assetIndex;
        
        /// <summary>
        /// Layers that the ammo can collide with.
        /// </summary>
            
        public LayerMask CollisionLayer
        {
            get => collisionLayer;
            set => collisionLayer = value;
        }
        [Tooltip("Layers that the ammo can collide with.")]
        [SerializeField] private LayerMask collisionLayer;
        
        /// <summary>
        /// Time between shots fired (in seconds).
        ///
        /// The X value is the set time that the timer will reset to.
        /// The Y value is the current count of the timer (the one being reduced).
        /// </summary>
            
        public float Cooldown
        {
            get => cooldown;
            set => cooldown = Mathf.Max(0.0f, value);
        }
        [Tooltip("Time between shots fired (in seconds)." + 
                 "\n \n The X value is the set time that the timer will reset to." +
                 "\n    The Y value is the current count of the timer (the one being reduced).")]
        [Space, SerializeField] private float cooldown = 0.8f;
        
        /// <summary>
        /// How fast the ammo travels.
        /// </summary>
            
        public float Speed
        {
            get => speed;
            set => speed = Mathf.Max(20.0f, value);
        }
        [Tooltip("How fast the ammo travels.")]
        [SerializeField] private float speed = 40.0f;
        
        /// <summary>
        /// How much damage the ammo does.
        /// </summary>
            
        public float Damage
        {
            get => damage;
            set => damage = Mathf.Max(0.0f, value);
        }
        [Tooltip("How much damage the ammo does.")]
        [SerializeField] private float damage = 20.0f;
        
        /// <summary>
        /// Sound of this specific weapon when the tanks shoots (Ensure this matches the settings in the Audio Manager).
        /// </summary>
            
        public string ShotSound
        {
            get => shotSound;
            set => shotSound = value;
        }
        [Tooltip("Sound of this specific weapon when the tanks shoots (Ensure this matches the settings in the Audio Manager).")]
        [SerializeField] private string shotSound;
        [HideInInspector] public int shotSoundIndex;
        
        /// <summary>
        /// Sound effect when the ammo explodes (Ensure this matches the settings in the Audio Manager).
        /// </summary>
            
        public string ExplosionSound
        {
            get => explosionSound;
            set => explosionSound = value;
        }
        [Tooltip("Sound effect when the ammo explodes (Ensure this matches the settings in the Audio Manager).")]
        [SerializeField] private string explosionSound;
        [HideInInspector] public int explosionSoundIndex;
        
        /// <summary>
        /// The particle system to play when the tanks shoots (Ensure this matches the settings in the Asset Manager).
        /// </summary>
            
        public string MuzzleFlash
        {
            get => muzzleFlash;
            set => muzzleFlash = value;
        }
        [Tooltip("The particle system to play when the tanks shoots (Ensure this matches the settings in the Asset Manager).")]
        [SerializeField] private string muzzleFlash;
        [HideInInspector] public int muzzleFlashIndex;
        
        /// <summary>
        /// The particle system to play when the ammo explodes (Ensure this matches the settings in the Asset Manager).
        /// </summary>
            
        public string Explosion
        {
            get => explosion;
            set => explosion = value;
        }
        [Tooltip("The particle system to play when the ammo explodes (Ensure this matches the settings in the Asset Manager).")]
        [SerializeField] private string explosion;
        [HideInInspector] public int explosionIndex;

        #endregion
        
        
        
        #region Functions

        #if UNITY_EDITOR
        
        public void OnValidate()
        {
            Name = _name;
            Cooldown = cooldown;
            Speed = speed;
            Damage = damage;
            ShotSound = shotSound;
            ExplosionSound = explosionSound;
            MuzzleFlash = muzzleFlash;
            Explosion = explosion;
        }

        #endif

        #endregion
        
        
    }
}
