using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    /// <summary>
    /// The instance.
    /// </summary>
    
    private static T instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    
    public static T Instance
    {
        get
        {
            if (instance != null) return instance;
            
            instance = FindObjectOfType<T>();
            if (instance != null) return instance;

            var obj = new GameObject {name = typeof(T).Name};
            instance = obj.AddComponent<T>();
            return instance;
        }
    }

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}