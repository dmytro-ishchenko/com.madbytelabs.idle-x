//Copyright(c)2020 Procedural Worlds Pty Limited
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeNa.Core
{
    [CustomEditor(typeof(GeNaRoadEndpointFadeExtension))]
    public class GeNaRoadEndpointFadeExtensionEditor : GeNaSplineExtensionEditor
    {
        private GeNaRoadEndpointFadeExtension m_extension;
        private bool m_isAsset;

        protected void OnEnable()
        {
            if (m_editorUtils == null)
                m_editorUtils = PWApp.GetEditorUtils(this, "GeNaSplineExtensionEditor");
            m_extension = target as GeNaRoadEndpointFadeExtension;
            if (m_extension == null)
                return;
            m_isAsset = AssetDatabase.Contains(m_extension);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (m_extension == null)
                m_extension = target as GeNaRoadEndpointFadeExtension;
            if (m_extension == null)
                return;

            EditorGUI.BeginChangeCheck();
            {
                m_editorUtils.Heading("Endpoint Fade Settings");
                m_editorUtils.InlineHelp("Endpoint Fade Settings", HelpEnabled);
                EditorGUI.indentLevel++;
                m_extension.StartHiddenLength = Mathf.Max(0f, m_editorUtils.FloatField("Start Hidden Length", m_extension.StartHiddenLength, HelpEnabled));
                m_extension.StartFadeLength = Mathf.Max(0.1f, m_editorUtils.FloatField("Start Fade Length", m_extension.StartFadeLength, HelpEnabled));
                m_extension.EndHiddenLength = Mathf.Max(0f, m_editorUtils.FloatField("End Hidden Length", m_extension.EndHiddenLength, HelpEnabled));
                m_extension.EndFadeLength = Mathf.Max(0.1f, m_editorUtils.FloatField("End Fade Length", m_extension.EndFadeLength, HelpEnabled));
                m_extension.EndpointDetail = m_editorUtils.IntSlider("Endpoint Detail", m_extension.EndpointDetail, 1, 4, HelpEnabled);
                m_extension.SurfaceBreakup = m_editorUtils.Slider("Surface Breakup", m_extension.SurfaceBreakup, 0f, GeNaRoadEndpointFadeProcessor.MaxSurfaceBreakup, HelpEnabled);
                EditorGUILayout.BeginHorizontal();
                m_extension.Seed = m_editorUtils.IntField("Seed", m_extension.Seed);
                if (GUILayout.Button("Randomize", GUILayout.Width(90f)))
                    m_extension.RandomizeSeed();
                EditorGUILayout.EndHorizontal();
                m_editorUtils.InlineHelp("Seed", HelpEnabled);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
                m_editorUtils.Heading("Road Segments");
                m_editorUtils.InlineHelp("Road Segments", HelpEnabled);

                if (m_isAsset)
                {
                    EditorGUILayout.HelpBox("Attach this extension to a spline that has a Roads extension. Road segments will appear here after the road meshes are generated.", MessageType.Info);
                }
                else
                {
                    GeNaSpline spline = m_extension.Spline;
                    GeNaRoadExtension roadExtension = spline != null ? spline.GetExtension<GeNaRoadExtension>() : null;
                    if (roadExtension == null)
                    {
                        EditorGUILayout.HelpBox("This spline has no Roads extension. Add Roads first so endpoint fade can target generated road segments.", MessageType.Warning);
                    }
                    else if (GeNaUtility.GetActivePipeline() == Constants.RenderPipeline.BuiltIn)
                    {
                        EditorGUILayout.HelpBox("Endpoint fade is supported in URP and HDRP only.", MessageType.Warning);
                    }
                    else
                    {
                        m_extension.RefreshSegmentList();
                        List<GeNaRoad> roads = GeNaRoadEndpointFadeExtension.FindRoadSegments(spline);
                        if (roads.Count == 0)
                        {
                            EditorGUILayout.HelpBox("No road segments were found. Draw the spline and keep the Roads extension active so meshes can be generated.", MessageType.Info);
                        }
                        else
                        {
                            List<GeNaRoadEndpointFadeSegment> segments = m_extension.Segments;
                            for (int i = 0; i < segments.Count; i++)
                            {
                                GeNaRoadEndpointFadeSegment segment = segments[i];
                                if (segment == null)
                                    continue;
                                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                EditorGUILayout.LabelField(segment.m_segmentName, EditorStyles.boldLabel);
                                segment.m_blendStart = EditorGUILayout.Toggle("Blend Start", segment.m_blendStart);
                                segment.m_blendEnd = EditorGUILayout.Toggle("Blend End", segment.m_blendEnd);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.HelpBox("Unity's selection outline ignores alpha clipping and can make a faded endpoint look solid. Deselect the spline to judge the blend.", MessageType.Info);
                        }
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_extension);
                if (!m_isAsset)
                    m_extension.ApplyToRoadMeshes();
            }
        }
    }
}
