using UnityEditor;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEditorInternal;
using UnityEngine.Rendering.PostProcessing;
#endif
namespace GeNa.Core
{
    [InitializeOnLoad]
    public class PostProcessingBugFix
    {
#pragma warning disable UDR0001
        private static bool m_isDirty = true;
#pragma warning restore UDR0001
        static PostProcessingBugFix()
        {
#pragma warning disable UDR0001
            EditorApplication.update += Update;
#pragma warning restore UDR0001
        }
        private static void Update()
        {
            if (m_isDirty)
            {
#if UNITY_POST_PROCESSING_STACK_V2
#if UNITY_2023_1_OR_NEWER
                PostProcessVolume postProcessVolume = UnityEngine.Object.FindAnyObjectByType<PostProcessVolume>();
#else
                PostProcessVolume postProcessVolume = UnityEngine.Object.FindObjectOfType<PostProcessVolume>();
#endif
                InternalEditorUtility.SetIsInspectorExpanded(postProcessVolume, true);
                InternalEditorUtility.SetIsInspectorExpanded(postProcessVolume, false);
#endif
                m_isDirty = false;
            }
        }
    }
}