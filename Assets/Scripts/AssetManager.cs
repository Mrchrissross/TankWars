using UnityEngine;

[System.Serializable]
public class Asset
{
    //variables
    public string prefabName;
    public GameObject prefab;

    [Range(0.05f, 2.0f)]
    public float size = 1.0f;
    [Range(0.0f, 0.5f)]
    public float randomness = 0.2f;

    public bool infiniteLife;
    public float lifeDuration = 2.0f;
}

public class AssetManager : MonoBehaviour
{
    public static AssetManager instance;

    [SerializeField]
    Asset[] assets;

    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Spawn the asset
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

                AudioManager.instance.PlaySound(name);

                return asset;
            }
        }
        Debug.LogWarning("AssetManager: Asset not found in list. " + name);
        return null;
    }
}
