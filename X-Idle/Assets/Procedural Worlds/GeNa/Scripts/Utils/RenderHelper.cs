using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GeNa.Scripts.Utils
{

    public class RenderHelper
    {
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] quadMatrix, int count, MaterialPropertyBlock mpv, ShadowCastingMode shadowCastingMode, bool receiveShadows, int layer, Camera drawCamera, LightProbeUsage lpu)
        {
#if UNITY_6000_5_OR_NEWER
            Graphics.DrawMeshInstanced(mesh, submeshIndex, material, quadMatrix, count, mpv, shadowCastingMode, receiveShadows, layer, drawCamera);
#else
            Graphics.DrawMeshInstanced(mesh, submeshIndex, material, quadMatrix, count, mpv, shadowCastingMode, receiveShadows, layer, drawCamera, lpu, null);
#endif
        }

        public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer computeBuffer, int argsOffset, MaterialPropertyBlock quadMaterialPropertyBlock, ShadowCastingMode scm, bool receiveShadows, int layer, Camera camera, LightProbeUsage lpu, object value)
        {
#if UNITY_6000_5_OR_NEWER
            Graphics.DrawMeshInstancedIndirect(mesh,submeshIndex,material,bounds,computeBuffer,argsOffset,quadMaterialPropertyBlock,scm,receiveShadows,layer,camera);
#else
            Graphics.DrawMeshInstancedIndirect(mesh,submeshIndex,material,bounds,computeBuffer,argsOffset,quadMaterialPropertyBlock,scm,receiveShadows,layer,camera, lpu);
#endif

        }

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
