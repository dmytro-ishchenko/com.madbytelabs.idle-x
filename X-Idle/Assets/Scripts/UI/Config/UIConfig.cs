using UnityEngine;

namespace UI.Config
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "AFTER/UI/Config")]
    internal class UIConfig : ScriptableObject
    {
        private static UIConfig s_instance;

        public static UIConfig Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = Resources.Load<UIConfig>("UIConfig");
                return s_instance;
            }
        }

        [SerializeField] private Color m_positiveEffectColor;
        [SerializeField] private Color m_negativeEffectColor;
        [SerializeField] private Color m_positiveBonusColor;
        [SerializeField] private Color m_condition0;
        [SerializeField] private Color m_condition30;
        [SerializeField] private Color m_condition60;
        [SerializeField] private Color m_condition90;
        [SerializeField] private Color m_condition100;

        public Color PositiveEffectColor => m_positiveEffectColor;
        public Color NegativeEffectColor => m_negativeEffectColor;
        public Color PositiveBonusColor => m_positiveBonusColor;
        public Color Condition0 => m_condition0;
        public Color Condition30 => m_condition30;
        public Color Condition60 => m_condition60;
        public Color Condition90 => m_condition90;
        public Color Condition100 => m_condition100;
    }
}