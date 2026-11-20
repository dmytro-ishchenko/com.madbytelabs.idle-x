using UnityEngine;

namespace GeNa.Core
{
    public static class GeNaRuntimeEvents
    {
        //Disabled Domain Reload Warning
        //Reason: The warning in UDR0005 seems a bit weird in this place - it complains about a non static delegate being subscribed to
        //a static event, but this is merely the definition of the static event here.
#pragma warning disable UDR0005
        public static System.Action onBeforeSceneLoad;
        public static System.Action onAfterSceneLoad;
        public static System.Action onRuntimeLoad;
#pragma warning restore UDR0005

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            onBeforeSceneLoad?.Invoke();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoadRuntimeMethod()
        {
            onAfterSceneLoad?.Invoke();
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeLoadMethod()
        {
            onRuntimeLoad?.Invoke();
            Initialize();
        }

#if UNITY_2023_1_OR_NEWER
        public static Object[] FindObjectsByType(System.Type type)
        {
#if UNITY_6000_4_OR_NEWER
            return Object.FindObjectsByType(type);
#else
            return Object.FindObjectsByType(type, FindObjectsSortMode.None);
#endif
            
        }
#endif

        public static void Initialize()
        {
#if UNITY_2023_1_OR_NEWER
            GeNaEvents.findObjectOfType = Object.FindAnyObjectByType;
            GeNaEvents.findObjectsOfType = FindObjectsByType;
#else
            GeNaEvents.findObjectOfType = Object.FindObjectOfType;
            GeNaEvents.findObjectsOfType = Object.FindObjectsOfType;
#endif
        }
    }
}