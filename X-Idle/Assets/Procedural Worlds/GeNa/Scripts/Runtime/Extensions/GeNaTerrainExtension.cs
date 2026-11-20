using System;
using UnityEngine;
namespace GeNa.Core
{
    /// <summary>
    /// Spline Extension for performing Terrain operations (note: only works on Terrains)
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Terrain", menuName = "Procedural Worlds/GeNa/Extensions/Terrain", order = 0)]
    public class GeNaTerrainExtension : GeNaSplineExtension
    {
        #region Variables
        [SerializeField] protected float m_width = 5f;
        [SerializeField] protected float m_heightOffset = 0f;
        [SerializeField] protected float m_noiseStrength = 1.0f;
        [SerializeField] protected bool m_roadLike = false;
        [SerializeField] protected bool m_hidePreview;
        [SerializeField] protected float m_shoulder = 1.5f;
        [SerializeField] protected Fractal m_maskFractal = new Fractal();
        [SerializeField] protected AnimationCurve m_shoulderFalloff = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));
        [SerializeField] protected TerrainModifier m_terrainModifier = new TerrainModifier();
        [NonSerialized] protected TerrainTools m_terrainTools;
        [NonSerialized] private TerrainEntity m_terrainEntity;
        [NonSerialized] private bool m_isDirty = true;
        [NonSerialized] private Terrain m_previewTerrain;
        #endregion
        #region Properties
        public bool ShowPreview
        {
            get => !m_hidePreview;
            set
            {
                bool hide = !value;
                if (m_hidePreview == hide)
                    return;
                m_hidePreview = hide;
                if (m_hidePreview)
                    DisposePreview();
                else
                    MarkPreviewDirty();
            }
        }
        public EffectType EffectType
        {
            get => m_terrainModifier.EffectType;
            set
            {
                if (m_terrainModifier.EffectType == value)
                    return;
                m_terrainModifier.EffectType = value;
                MarkPreviewDirty();
            }
        }
        public float Strength
        {
            get => m_terrainModifier.Strength;
            set
            {
                if (Mathf.Approximately(m_terrainModifier.Strength, value))
                    return;
                m_terrainModifier.Strength = value;
                MarkPreviewDirty();
            }
        }
        public int TextureProtoIndex
        {
            get => m_terrainModifier.TextureProtoIndex;
            set
            {
                if (m_terrainModifier.TextureProtoIndex == value)
                    return;
                m_terrainModifier.TextureProtoIndex = value;
                MarkPreviewDirty();
            }
        }
        public int DetailProtoIndex
        {
            get => m_terrainModifier.DetailProtoIndex;
            set
            {
                if (m_terrainModifier.DetailProtoIndex == value)
                    return;
                m_terrainModifier.DetailProtoIndex = value;
                MarkPreviewDirty();
            }
        }
        public float Width
        {
            get => m_width;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (Mathf.Approximately(m_width, clamped))
                    return;
                m_width = clamped;
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
        public float NoiseStrength
        {
            get => m_noiseStrength;
            set
            {
                if (Mathf.Approximately(m_noiseStrength, value))
                    return;
                m_noiseStrength = value;
                MarkPreviewDirty();
            }
        }
        public float Shoulder
        {
            get => m_shoulder;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (Mathf.Approximately(m_shoulder, clamped))
                    return;
                m_shoulder = clamped;
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
        public TerrainTools GetTerrainTools()
        {
            if (m_terrainTools == null)
            {
                GeNaManager geNaManager = GeNaManager.GetInstance();
                if (geNaManager != null)
                    m_terrainTools = geNaManager.TerrainTools;
            }
            return m_terrainTools;
        }
        private void ApplyToolSettings(TerrainTools tools)
        {
            tools.RoadLike = RoadLike;
            tools.Width = Width;
            tools.Shoulder = Shoulder;
            tools.Strength = Strength;
            tools.HeightOffset = HeightOffset;
            tools.ShoulderFalloff = ShoulderFalloff;
            tools.MaskFractal = MaskFractal;
        }
        protected override void OnSceneGUI()
        {
            Visualize();
        }
        public void Visualize()
        {
            if (!m_isSelected || !ShowPreview)
                return;
            TerrainTools tools = GetTerrainTools();
            if (tools == null)
                return;
            ApplyToolSettings(tools);
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
        private void Modify(bool recordUndo = true)
        {
            TerrainTools tools = GetTerrainTools();
            if (tools == null)
                return;
            ApplyToolSettings(tools);
            tools.ApplySplineToTerrains(m_terrainModifier, Spline, recordUndo, EffectType.ToString());
        }
        public void Clear()
        {
            Modify();
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
        protected override void OnDelete()
        {
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
