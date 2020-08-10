using UnityEngine;

namespace TankWars.Managers
{

    [System.Serializable]
    public class Asset
    {
        
        #region Fields
        
        // Asset Info.
        public string prefabName;
        public GameObject prefab;

        // Size and randomness controller.
        [Range(0.05f, 2.0f)] public float size = 1.0f;
        [Range(0.0f, 0.5f)] public float randomness = 0.2f;

        // Life span once instantiated.
        public bool infiniteLife;
        public float lifeDuration = 2.0f;
        
        #endregion
        
    }

    
    
    
    
    public class AssetManager : Singleton<AssetManager>
    {
        #region Fields

        // Asset array allows to choose how many assets we want to display,
        // also disable warning saying it is unused.
        #pragma warning disable 0649
        [SerializeField] private Asset[] assets;
        #pragma warning restore 0649
        
        #endregion



        #region Functions

        /// <summary>
        /// Spawns an asset.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="position">Asset position once spawned.</param>
        /// <param name="rotation">Asset rotation once spawned.</param>
        /// <param name="hasSound">Does asset play a sound when spawned?</param>
        /// <returns>Returns the asset for future reference.</returns>
        public GameObject SpawnObject(string assetName, Vector3 position, Quaternion rotation, bool hasSound = false)
        {
            foreach (var asset in assets)
            {
                if (asset.prefabName != assetName) continue;
                
                var newAsset = Instantiate(asset.prefab, position, rotation);
                var size = Random.Range(asset.size - asset.randomness, asset.size + asset.randomness);
                newAsset.transform.localScale = new Vector3(size, size, size);

                if (!asset.infiniteLife) Destroy(newAsset, asset.lifeDuration);
                if(hasSound) AudioManager.Instance.PlaySound(assetName);

                return newAsset;
            }

            Debug.LogWarning("Asset Manager: Asset not found in list." + assetName);
            return null;
        }
        
        #endregion
        
    }
}
