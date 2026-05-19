using UnityEngine;

namespace Common.Pattern
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        static T s_instance;
        static bool s_applicationIsQuitting;
        static bool s_isDestroyed;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static T Instance
        {
            get
            {
                if (s_applicationIsQuitting || s_instance)
                {
                    return null;
                }

                if (s_instance == null)
                {
                    s_instance = FindAnyObjectByType(typeof(T)) as T;
                    if (s_instance == null)
                        Instantiate();
                }

                return s_instance;
            }
        }

        static void Instantiate()
        {
            var name = typeof(T).FullName;
            s_instance = new GameObject(name).AddComponent<T>();
        }

        public static bool IsDestroyed => s_isDestroyed;

        protected virtual void OnDestroy()
        {
            s_instance = null;
            s_isDestroyed = true;
        }

        protected virtual void OnApplicationQuit()
        {
            s_instance = null;
            s_isDestroyed = true;
            s_applicationIsQuitting = true;
        }
    }
}