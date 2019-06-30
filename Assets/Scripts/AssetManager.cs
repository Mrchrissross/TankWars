using UnityEngine;

[System.Serializable]
public class Asset
{
    // variables
    public string prefabName;
    public GameObject prefab;

    // size and randomness controller
    [Range(0.05f, 2.0f)]
    public float size = 1.0f;
    [Range(0.0f, 0.5f), Tooltip("Randomness in size")]
    public float randomness = 0.2f;

    // life
    public bool infiniteLife;
    public float lifeDuration = 2.0f;
}

public class AssetManager : Singleton<AssetManager>
{
    //Asset array allows to choose how many assets we want to display.
    [SerializeField]
    Asset[] assets;

    /// <summary>
    /// Spawns the asset.
    /// </summary>
    /// <param name="name">Name of the asset.</param>
    /// <param name="position">Position to spawn the assets.</param>
    /// <param name="rotation">Rotation of the asset when spawned.</param>
    /// <returns>Returns the assets for future reference.</returns>
    public GameObject SpawnObject(string name, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < assets.Length; i++)
        {
            if (assets[i].prefabName == name)
            {
                GameObject asset = Instantiate(assets[i].prefab, position, rotation);
                float size = Random.Range(assets[i].size - assets[i].randomness, assets[i].size + assets[i].randomness);
                asset.transform.localScale = new Vector3(size, size, size);

                if(!assets[i].infiniteLife)
                Destroy(asset, assets[i].lifeDuration);

                AudioManager.Instance.PlaySound(name);

                return asset;
            }
        }
        Debug.LogWarning("AssetManager: Asset not found in list. " + name);
        return null;
    }
}
