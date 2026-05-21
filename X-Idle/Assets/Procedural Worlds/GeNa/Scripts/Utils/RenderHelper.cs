using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GeNa.Scripts.Utils
{

    public class RenderHelper
    {
        public static void RenderFlowIntoTexture(Camera captureCam, RenderTexture rtFlow, List<Transform> meshTransforms, Material flowMaterial)
        {
#if UNITY_6000_0_OR_NEWER
            CommandBuffer cmd = new CommandBuffer { name = "CustomObjectRenderer" };
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            int tempRT = Shader.PropertyToID("_TempRT");

            cmd.Clear();

            cmd.GetTemporaryRT(tempRT, rtFlow.descriptor);

            cmd.SetRenderTarget(tempRT);

            cmd.ClearRenderTarget(true, true, Color.clear);

            cmd.SetViewMatrix(captureCam.worldToCameraMatrix);
            cmd.SetProjectionMatrix(captureCam.projectionMatrix);

            // Draw all renderers directly since they all use the same shader
            for (int i = 0; i < meshTransforms.Count; i++)
            {
                Renderer renderer = meshTransforms[i].GetComponent<Renderer>();
            
                if (renderer != null && renderer is MeshRenderer meshRenderer)
                {
                    var meshFilter = meshRenderer.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        renderer.GetPropertyBlock(propertyBlock);
                        cmd.DrawMesh(meshFilter.sharedMesh, renderer.localToWorldMatrix,
                                flowMaterial, 0, 0, propertyBlock);
                    }
                }
            }
            cmd.CopyTexture(tempRT, rtFlow);
            cmd.ReleaseTemporaryRT(tempRT);

            Graphics.ExecuteCommandBuffer(cmd);

             if (cmd != null)
            {
                cmd.Release();
                cmd = null;
            }
#else

            //Capture Flow into Render Texture
            captureCam.targetTexture = rtFlow;
            //RenderTexture.active = rtFlow;
            //Graphics.Blit(captureCam.targetTexture, rtFlow);
            captureCam.Render();
            //RenderTexture.active = rtFlow;
#endif

        }
    }
}
