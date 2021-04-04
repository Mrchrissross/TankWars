#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using DimensionalDeveloper.TankBuilder.Managers;

namespace DimensionalDeveloper.TankBuilder.Utility
{
    /// <summary>
    /// This script holds all menu item functions, such as creating a tank, camera, etc.
    /// </summary>
    
    public class MenuItems : MonoBehaviour
    {
        /// <summary>
        /// Creates an audio manager.
        /// </summary>

        [MenuItem("GameObject/Tank Wars/Create Audio Manager", false, -10)]
        public static void CreateAudioManager()
        {
            var audioManager = GameObject.Find("Audio Manager");
            
            if (audioManager == null)
            {
                // Cache the object.
                audioManager = new GameObject();
                var audioManagerTransform = audioManager.transform;

                audioManager.isStatic = true;
                audioManager.name = "Audio Manager";
                
                audioManagerTransform.position = Vector3.zero;
                audioManagerTransform.rotation = Quaternion.identity;
                audioManagerTransform.localScale = Vector3.one;

                audioManager.AddComponent<AudioManager>();
            }
            
            // Select the object.
            Selection.activeGameObject = audioManager;
            
            Debug.Log("Audio Manager: An audio manager already exists in the hierarchy.");
        }
        
        /// <summary>
        /// Creates an asset manager.
        /// </summary>

        [MenuItem("GameObject/Tank Wars/Create Asset Manager", false, -10)]
        public static void CreateAssetManager()
        {
            var assetManager = GameObject.Find("Asset Manager");
            
            if (assetManager == null)
            {
                // Cache the object.
                assetManager = new GameObject();
                var assetManagerTransform = assetManager.transform;

                assetManager.isStatic = true;
                assetManager.name = "Asset Manager";
                
                assetManagerTransform.position = Vector3.zero;
                assetManagerTransform.rotation = Quaternion.identity;
                assetManagerTransform.localScale = Vector3.one;

                assetManager.AddComponent<AssetManager>();
            }
            
            // Select the object.
            Selection.activeGameObject = assetManager;
            
            Debug.Log("Asset Manager: An asset manager already exists in the hierarchy.");
        }

        /// <summary>
        /// Builds a tank.
        /// </summary>
        
        [MenuItem("GameObject/Tank Wars/Create Tank", false, -10)]
        public static void CreateTank()
        {
            // Cache the object.
            var tank = new GameObject();
            var tankTransform = tank.transform;

            // Select the object.
            Selection.activeGameObject = tank;
            
            // Manually assign variables.
            tank.name = "New Tank";
            tankTransform.position = Vector3.zero;
            tankTransform.rotation = Quaternion.identity;
            tankTransform.localScale = Vector3.one * 0.25f;
            
            // Add the tank builder component.
            var tankBuilder = tank.AddComponent<TankBuilder>();
            
            // Add the hull and cannon.
            tankBuilder.SpawnHull();
            tankBuilder.SpawnCannon();
        }
    }
}

#endif
