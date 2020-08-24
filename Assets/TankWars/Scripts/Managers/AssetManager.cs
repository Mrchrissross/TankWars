using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankWars.Managers
{

    [System.Serializable]
    public class Asset
    {
        
        #region Fields
        
        // Unity Inspector.
        [HideInInspector] public bool hideSection;
        
        // Asset Info.
        public string name;
        public GameObject prefab;

        // Size and randomness controller.
        public Vector2 scaleX = new Vector2(0.8f, 1.2f);
        public Vector2 scaleY = new Vector2(0.8f, 1.2f);

        // Life span once instantiated.
        public bool infiniteLife;
        public float lifeDuration = 2.0f;

        // Pooling variables.
        public Transform source;
        public Dictionary<GameObject, bool> objectPool = new Dictionary<GameObject, bool>();

        #endregion

        
        
        
        #region Functions

        /// <summary>
        /// Assigns the transform to hold all elements in the pool.
        /// </summary>
        /// <param name="transform">The transform to hold pool.</param>
        
        public void SetTransform(Transform transform) => source = transform;

        /// <summary>
        /// Creates a new asset.
        /// </summary>
        
        public Asset() => name = "New Asset";

        /// <summary>
        /// Create a copy of an existing asset.
        /// </summary>
        
        public Asset(Asset copy)
        {
            name = copy.name;
            prefab = copy.prefab;
            scaleX = copy.scaleX;
            scaleY = copy.scaleY;
            infiniteLife = copy.infiniteLife;
            lifeDuration = copy.lifeDuration;
        }
        
        #endregion

    }

    
    
    
    
    public class AssetManager : Singleton<AssetManager>
    {
        #region Fields

        // Asset array allows to choose how many assets we want to display,
        // also disable warning saying it is unused.
        #pragma warning disable 0649
        [SerializeField] public List<Asset> assets = new List<Asset>();
        #pragma warning restore 0649
        
        #endregion



        #region Functions

        public Asset GetAsset(string assetName)
        {
            if (assetName == "None") return null;

            foreach (var asset in assets.Where(asset => asset.name == assetName))
                return asset;
            
            Debug.LogWarning("Asset Manager: Asset not found in list." + assetName);
            return null;
        }
        
        /// <summary>
        /// Spawns an asset.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="position">Asset position once spawned.</param>
        /// <param name="rotation">Asset rotation once spawned.</param>
        /// <returns>Returns the asset for future reference.</returns>
        
        public GameObject SpawnObject(string assetName, Vector3 position, Quaternion rotation)
        {
            if (assetName == "None") return null;

            var asset = GetAsset(assetName);

            if (asset == null)
            {
                Debug.LogWarning("Asset Manager: Asset not found in list." + assetName);
                return null;
            }

            GameObject objectToUse = null;

            foreach (var element in asset.objectPool)
            {
                if (element.Value) continue;

                objectToUse = element.Key;
                objectToUse.SetActive(true);
                break;
            }

            if (objectToUse == null)
            {
                objectToUse = Instantiate(asset.prefab);
                asset.objectPool.Add(objectToUse, true);
            }
            else asset.objectPool[objectToUse] = true;
            
            var newAssetTransform = objectToUse.transform;
            
            var sizeX = Random.Range(asset.scaleX.x, asset.scaleX.y);
            var sizeY = Random.Range(asset.scaleY.x, asset.scaleY.y);
            
            newAssetTransform.parent = asset.source;
            newAssetTransform.position = position;
            newAssetTransform.rotation = rotation;
            newAssetTransform.localScale = new Vector2(sizeX, sizeY);

            if (!asset.infiniteLife) Deactivate(asset, objectToUse, asset.lifeDuration);
            
            return objectToUse;
        }
        
        /// <summary>
        /// Creates a new asset.
        /// </summary>

        public void AddAsset()
        {
            var asset = new Asset();
            assets.Add(asset);
        } 
        
        /// <summary>
        /// Creates a copy of specified asset.
        /// </summary>
        /// <param name="asset">Asset to copy.</param>
        
        public void CopyAsset(Asset asset) => assets.Add(new Asset(asset));
        
        /// <summary>
        /// Creates the pool transform (parent) for all assets in this category.
        /// </summary>
        /// <param name="index">Index within the list of assets.</param>
        /// <param name="asset"></param>
        
        public void CreatePool(int index, Asset asset)
        {
            var goName = "Asset " + index + ": " + asset.name;

            if (asset.source != null)
            {
                asset.source.name = goName;
                return;
            }
            
            var go = new GameObject(goName);
            go.transform.SetParent(transform);
            asset.SetTransform(go.transform);
        }

        /// <summary>
        /// Deactivate a game object after a certain amount of time.
        /// </summary>
        /// <param name="asset">Asset to deactivate.</param>
        /// <param name="element">The game object within the assets pool to be deactivated.</param>
        /// <param name="seconds">Seconds until deactivation.</param>
        private void Deactivate(Asset asset, GameObject element, float seconds) => StartCoroutine(IDeactivate(asset, element, seconds));
        
        /// <summary>
        /// Coroutine to deactivate a game object after a number of seconds.
        /// </summary>
        
        private IEnumerator IDeactivate (Asset asset, GameObject element, float seconds)
        {
            var timer = 0.0f;
            while(timer < seconds) 
            {
                // Pauses the coroutine - replace paused with pause condition.
                // while(paused) yield return null;
 
                timer += Time.deltaTime;
                yield return null;
            }
            
            asset.objectPool[element] = false;
            element.SetActive(false);
        }
        
        #endregion

        
        
        #region MonoBehaviour

        /// <summary>
        /// Initialises all pools.
        /// </summary>
        
        private void Start()
        {
            for (var index = 0; index < assets.Count; index++) 
                CreatePool(index, assets[index]);
        }

        #endregion
        
    }
}
