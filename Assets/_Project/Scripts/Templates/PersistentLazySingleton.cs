// *************************************************************** //
// Script done by Jorge Kojtych
// Singleton class that persists across scenes and instantiates itself if it doesn't exist when first accessed.
// *************************************************************** //

using UnityEngine;

/// <summary>
/// Singleton class for MonoBehaviour that persists across scenes, 
/// and instantiates itself if it doesn't exist when first accessed.
/// </summary>
public abstract class PersistentLazySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s_Instance;
    public static T Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindFirstObjectByType<T>();
                if (s_Instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    s_Instance = singletonObject.AddComponent<T>();
                }
            }
            return s_Instance;
        }
    }

    public static bool InstanceExists => s_Instance != null;

    protected virtual void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            Debug.LogWarning($"Multiple instances of singleton {typeof(T).Name} detected. Destroying duplicate.", this);
            Destroy(gameObject);
            return;
        }

        s_Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (s_Instance == this)
        {
            s_Instance = null;
        }
    }
}
