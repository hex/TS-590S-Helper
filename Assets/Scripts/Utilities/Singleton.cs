using UnityEngine;

/// <summary>
/// Generic Implementation of a Singleton MonoBehaviour.
/// </summary>
/// <typeparam name="T"></typeparam>
/// 
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region  Properties

    /// <summary>
    /// Returns the instance of this singleton.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T) FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                }
            }

            return instance;
        }
    }

    #endregion

    protected static T instance;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}