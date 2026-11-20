using System;
using UnityEngine;
namespace GeNa.Core
{
    /// <summary>
    /// Spline Extension for performing Terrain Carve operations (note: only works on Terrains)
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Carve", menuName = "Procedural Worlds/GeNa/Extensions/Carve", order = 0)]
    public class GeNaCarveExtension : GeNaSplineExtension
    {
        #region Variables
        // Compute Shader
        [SerializeField] protected float m_width = 5f;
        [SerializeField] protected float m_heightOffset = 0.0f;
        [SerializeField] protected float m_shoulder = 10.0f;
        [SerializeField] protected bool m_roadLike = false;
        [SerializeField] protected bool m_showPreview = true;
        [SerializeField] protected Fractal m_maskFractal = new Fractal
        {
            Enabled = true,
            NoiseFalloff = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f)),
            Strength = 1f,
            Seed = 1,
            Octaves = 8,
            Frequency = 1f,
            Persistence = .5f,
            Lacunarity = 2,
            Amplitude = 1
        };
        [SerializeField] protected AnimationCurve m_shoulderFalloff = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));
        [SerializeField] protected TerrainModifier m_terrainModifier = new TerrainModifier();
        [NonSerialized] protected TerrainTools m_terrainTools;
        [NonSerialized] protected TerrainEntity m_terrainEntity;
        [NonSerialized] private bool m_isDirty = true;
        [NonSerialized] private Terrain m_previewTerrain;
        #endregion
        #region Properties
        public bool ShowPreview
        {
            get => m_showPreview;
            set
            {
                if (m_showPreview == value)
                    return;
                m_showPreview = value;
                if (!m_showPreview)
                    DisposePreview();
                else
                    MarkPreviewDirty();
            }
        }
        public float Width
        {
            get => m_width;
            set
            {
                if (Mathf.Approximately(m_width, value))
                    return;
                m_width = value;
                MarkPreviewDirty();
            }
        }
        public float HeightOffset
        {
            get => m_heightOffset;
            set
            {
                if (Mathf.Approximately(m_heightOffset, value))
                    return;
                m_heightOffset = value;
                MarkPreviewDirty();
            }
        }
        // Smoothness
        public float Shoulder
        {
            get => m_shoulder;
            set
            {
                if (Mathf.Approximately(m_shoulder, value))
                    return;
                m_shoulder = value;
                MarkPreviewDirty();
            }
        }
        public bool RoadLike
        {
            get => m_roadLike;
            set
            {
                if (m_roadLike == value)
                    return;
                m_roadLike = value;
                MarkPreviewDirty();
            }
        }
        public Fractal MaskFractal
        {
            get => m_maskFractal;
            set
            {
                if (m_maskFractal == value)
                    return;
                m_maskFractal = value;
                MarkPreviewDirty();
            }
        }
        public AnimationCurve ShoulderFalloff
        {
            get => m_shoulderFalloff;
            set
            {
                if (m_shoulderFalloff == value)
                    return;
                m_shoulderFalloff = value;
                MarkPreviewDirty();
            }
        }
        #endregion
        #region Methods
        private TerrainTools GetTerrainTools()
        {
            if (m_terrainTools == null)
            {
                GeNaManager geNaManager = GeNaManager.GetInstance();
                if (geNaManager != null)
                {
                    m_terrainTools = geNaManager.TerrainTools;
                }
            }
            return m_terrainTools;
        }
        protected override void OnSceneGUI()
        {
            Visualize();
        }
        protected void UpdateTerrainModifier()
        {
            m_terrainModifier.EffectType = EffectType.Flatten;
        }
        public TerrainEntity GetTerrainEntity()
        {
            TerrainTools tools = GetTerrainTools();
            if (tools == null)
                return null;
            tools.Width = Width;
            tools.Shoulder = Shoulder;
            tools.RoadLike = RoadLike;
            tools.HeightOffset = HeightOffset;
            tools.MaskFractal = MaskFractal;
            tools.ShoulderFalloff = ShoulderFalloff;
            UpdateTerrainModifier();
            return tools.GenerateTerrainEntity(m_terrainModifier, Spline);
        }
        public void Visualize()
        {
            if (!m_isSelected || !m_showPreview)
                return;
            TerrainTools tools = GetTerrainTools();
            if (tools == null)
                return;
            tools.Width = Width;
            tools.Shoulder = Shoulder;
            tools.RoadLike = RoadLike;
            tools.HeightOffset = HeightOffset;
            tools.MaskFractal = MaskFractal;
            tools.ShoulderFalloff = ShoulderFalloff;
            UpdateTerrainModifier();
            Terrain previewTerrain = tools.GetPreviewTerrain(Spline);
            if (m_isDirty || m_previewTerrain != previewTerrain || m_terrainEntity == null)
            {
                DisposePreview();
                m_terrainEntity = tools.GenerateTerrainEntity(m_terrainModifier, Spline, previewTerrain);
                m_previewTerrain = previewTerrain;
                m_isDirty = false;
            }
            if (m_terrainEntity != null)
                tools.Visualize(m_terrainEntity);
        }
        public void Carve()
        {
            Modify();
        }
        private void Modify(bool recordUndo = true)
        {
            TerrainTools tools = GetTerrainTools();
            if (tools == null)
                return;
            tools.Width = Width;
            tools.Shoulder = Shoulder;
            tools.RoadLike = RoadLike;
            tools.HeightOffset = HeightOffset;
            tools.MaskFractal = MaskFractal;
            tools.ShoulderFalloff = ShoulderFalloff;
            UpdateTerrainModifier();
            tools.ApplySplineToTerrains(m_terrainModifier, Spline, recordUndo, "Carve");
        }
        protected override GameObject OnBake(GeNaSpline spline)
        {
            Modify(false);
            return null;
        }
        public override void Execute()
        {
            if (!m_isSelected)
                return;
            Visualize();
        }
        protected override void OnSelect()
        {
            m_isSelected = true;
            MarkPreviewDirty();
            Visualize();
        }
        protected override void OnDeselect()
        {
            m_isSelected = false;
            DisposePreview();
        }
        protected override void OnSplineDirty()
        {
            MarkPreviewDirty();
        }
        public void MarkPreviewDirty()
        {
            m_isDirty = true;
        }
        private void DisposePreview()
        {
            m_previewTerrain = null;
            if (m_terrainEntity != null)
            {
                m_terrainEntity.Dispose();
                m_terrainEntity = null;
            }
        }
        #endregion
    }
}