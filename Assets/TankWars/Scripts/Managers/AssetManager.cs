using System.Collections.Generic;
using UnityEngine;

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

        #endregion

        
        
        
        #region Functions

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
            
            foreach (var asset in assets)
            {
                if (asset.name != assetName) continue;
                
                var newAsset = Instantiate(asset.prefab, position, rotation);
                
                var sizeX = Random.Range(asset.scaleX.x, asset.scaleX.y);
                var sizeY = Random.Range(asset.scaleY.x, asset.scaleY.y);
                
                newAsset.transform.localScale = new Vector2(sizeX, sizeY);

                if (!asset.infiniteLife) Destroy(newAsset, asset.lifeDuration);
                
                return newAsset;
            }

            Debug.LogWarning("Asset Manager: Asset not found in list." + assetName);
            return null;
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
        
        #endregion
        
    }
}
