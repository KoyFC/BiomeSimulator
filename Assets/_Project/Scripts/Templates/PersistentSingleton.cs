// *************************************************************** //
// Script done by Jorge Kojtych
// Singleton class that persists across scenes.
// *************************************************************** //

using UnityEngine;

/// <summary>
/// Singleton class for MonoBehaviour that persists across scenes, 
/// but doesn't instantiate itself if it doesn't exist.
/// </summary>
public abstract class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    public static bool InstanceExists => Instance != null;

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Multiple instances of singleton {typeof(T).Name} detected. Destroying duplicate.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
