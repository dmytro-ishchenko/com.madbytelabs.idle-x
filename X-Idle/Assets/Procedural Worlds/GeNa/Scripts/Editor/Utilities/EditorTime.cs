using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeNa.Core
{
    [InitializeOnLoad]
    public static class EditorTime
    {
#pragma warning disable UDR0001
        public static float deltaTime = 0f;
        public static float elapsedTime = 0f;
        public static float lastTimeSinceStartup = 0f;
        static EditorTime()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }
#pragma warning restore UDR0001
        private static void Update()
        {
            SetEditorDeltaTime();
        }
        private static void SetEditorDeltaTime()
        {
            if (lastTimeSinceStartup == 0f)
            {
                lastTimeSinceStartup = (float)EditorApplication.timeSinceStartup;
            }
            deltaTime = (float)EditorApplication.timeSinceStartup - lastTimeSinceStartup;
            lastTimeSinceStartup = (float)EditorApplication.timeSinceStartup;
            elapsedTime += deltaTime;
        }
    }
}
