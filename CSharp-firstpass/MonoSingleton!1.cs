using System;
using UnityEngine;

[AutoSingleton(true)]
public class MonoSingleton<T> : MonoBehaviour where T: Component
{
    private static bool _destroyed;
    private static T _instance;

    protected virtual void Awake()
    {
        if ((MonoSingleton<T>._instance != null) && (MonoSingleton<T>._instance.gameObject != base.gameObject))
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(base.gameObject);
            }
        }
        else if (MonoSingleton<T>._instance == null)
        {
            MonoSingleton<T>._instance = base.GetComponent<T>();
        }
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        this.Init();
    }

    public static void ClearDestroy()
    {
        MonoSingleton<T>.DestroyInstance();
        MonoSingleton<T>._destroyed = false;
    }

    public static void DestroyInstance()
    {
        if (MonoSingleton<T>._instance != null)
        {
            UnityEngine.Object.Destroy(MonoSingleton<T>._instance.gameObject);
        }
        MonoSingleton<T>._destroyed = true;
        MonoSingleton<T>._instance = null;
    }

    public virtual void DestroySelf()
    {
        MonoSingleton<T>._instance = null;
        UnityEngine.Object.Destroy(base.gameObject);
    }

    public static T GetInstance()
    {
        if ((MonoSingleton<T>._instance == null) && !MonoSingleton<T>._destroyed)
        {
            System.Type type = typeof(T);
            MonoSingleton<T>._instance = (T) UnityEngine.Object.FindObjectOfType(type);
            if (MonoSingleton<T>._instance == null)
            {
                object[] customAttributes = type.GetCustomAttributes(typeof(AutoSingletonAttribute), true);
                if ((customAttributes.Length > 0) && !((AutoSingletonAttribute) customAttributes[0]).bAutoCreate)
                {
                    return null;
                }
                GameObject obj2 = new GameObject(typeof(T).Name);
                MonoSingleton<T>._instance = obj2.AddComponent<T>();
                GameObject obj3 = GameObject.Find("BootObj");
                if (obj3 != null)
                {
                    obj2.transform.SetParent(obj3.transform);
                }
            }
        }
        return MonoSingleton<T>._instance;
    }

    public static bool HasInstance()
    {
        return (MonoSingleton<T>._instance != null);
    }

    protected virtual void Init()
    {
    }

    protected virtual void OnDestroy()
    {
        if ((MonoSingleton<T>._instance != null) && (MonoSingleton<T>._instance.gameObject == base.gameObject))
        {
            MonoSingleton<T>._instance = null;
        }
    }

    public static T instance
    {
        get
        {
            return MonoSingleton<T>.GetInstance();
        }
    }
}

