using System;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static object _lock = new ();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
                
                return _instance;
            }
        }
    }

    public class LazySingleton<T> where T : class, new()
    {
        private static readonly Lazy<T> _instance = new (() => new T());

        public static T Instance
        {
            get
            {
                T inst = _instance.Value;
                lock (inst)
                {
                    return inst;
                }
            }
        }
    }

    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _destroyed;
        private static T _instance;
        private static object _lock = new();

        public static T Instance
        {
            get
            {
                if (_destroyed)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindAnyObjectByType<T>();
                        if (_instance == null)
                        {
                            GameObject obj = new GameObject(typeof(T).ToString());
                            _instance = obj.AddComponent<T>();
                            DontDestroyOnLoad(obj);
                        }
                    }
                }
                
                return _instance;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _instance = null;
            _destroyed = true;
        }

        protected void OnDestroy()
        {
            _instance = null;
            _destroyed = true;
        }

        protected static bool IsAlive() => !_destroyed;
    }

    public class LazySingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour, new ()
    {
        private static readonly Lazy<T> _instance = new(() =>
        {
            var instance = FindAnyObjectByType<T>();
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(T).ToString());
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }

            return instance;
        });

        public static T Instance
        {
            get
            {
                T inst = _instance.Value;
                lock (inst)
                {
                    return inst;
                }
            }
        }
    }
}