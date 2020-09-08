#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace TankWars.Utility
{
    /// <summary>
    /// This script holds all menu item functions, such as creating a tank, camera, etc.
    /// </summary>
    
    public class MenuItems : MonoBehaviour
    {
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
