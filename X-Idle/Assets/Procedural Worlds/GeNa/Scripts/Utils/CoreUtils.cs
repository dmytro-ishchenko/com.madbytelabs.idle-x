using System;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Helper class with static functions to process Unity version specific code.
/// </summary>
public class CoreUtils
{
    public static int GetId(UnityEngine.Object myObject)
    {
#if UNITY_6000_4_OR_NEWER
        return myObject.GetEntityId().GetHashCode();
#else
        return myObject.GetInstanceID();
#endif
    }

    /// <summary>
    /// Helper to deal with different ways to get the assemblies across unity versions
    /// </summary>
    /// <returns></returns>
    public static System.Reflection.Assembly[] GetAssemblies()
    {
#if UNITY_6000_5_OR_NEWER
        return (System.Reflection.Assembly[])UnityEngine.Assemblies.CurrentAssemblies.GetLoadedAssemblies();
#else
            return System.AppDomain.CurrentDomain.GetAssemblies();                 
#endif
    }

    /// <summary>
    /// Helper to deal with different ways to get the RP asset across unity versions
    /// </summary>
    /// <returns></returns>
    public static RenderPipelineAsset GetRenderPipelineAsset()
    {
        //look at the RP asset in quality settings first, that would be the one that overrides the default
        if (QualitySettings.renderPipeline != null)
        {
            return QualitySettings.renderPipeline;
        }
        //Fallback: take the default pipeline asset if none provided in the default quality settings
#if UNITY_6000_0_OR_NEWER
        return GraphicsSettings.defaultRenderPipeline;
#else
        return GraphicsSettings.renderPipelineAsset;
#endif
    }

    public static Vector3 GetLastActiveSceneViewPos()
    {
#if UNITY_EDITOR
        UnityEditor.SceneView sceneView = UnityEditor.SceneView.lastActiveSceneView;
        if (sceneView != null && sceneView.camera != null)
            return sceneView.camera.transform.position;
        else
#endif
            return Vector3.zero;
    }

    public static void DisplayProgressBar(string title, string processingInfo, int currentCounter, int maxCounter)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayProgressBar(
                       title,
                       $"Processing {processingInfo} ({currentCounter}/{maxCounter})",
                       maxCounter > 0 ? (float)currentCounter / maxCounter : 1f);
#endif
    }

    public static void ClearProgressBar()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
    }
}
