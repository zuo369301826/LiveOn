using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new()
{
    private static T _instance;

    private static object _lock = new object();

    public static T instance
    {
        get
        {
            lock(_lock)
            {
                if(_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }

    void OnDestroy()
    {
        lock (_lock)
        {
            if (_instance != null)
            {
                _instance = default(T);
            }
        }
    }
}

public class SingletonUI<T> : MonoBehaviour where T: MonoBehaviour
{//切换场景不需要销毁的界面继承这个单例类
    private static T _instance;

    private static object _lock = new object();

    private static bool applicationIsQuitting = false;

    private static bool ClearMode = false;

    public static T instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }

    public void OnDestroy()
    {
        if (ClearMode == false)
            applicationIsQuitting = true;
        ClearMode = false;
    }

    public static void DestorySingleton()
    {
        lock(_lock)
        {
            if(_instance != null)
            {
                ClearMode = true;
                GameObject.Destroy(_instance.gameObject);
                _instance = null;
            }
        }
    }
}

public class SingletonUIInOneScene<T> : MonoBehaviour where T : MonoBehaviour
{//切换场景需要销毁的界面继承这个单例类
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if(_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if(_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().ToString() + "(singletonTemp) " + typeof(T).ToString();
                    }
                }
                return _instance;
            }
        }
    }

    void OnDestroy()
    {
        lock(_lock)
        {
            if(_instance != null)
            {
                GameObject.Destroy(_instance.gameObject);
                _instance = null;
            }
        }
    }
}