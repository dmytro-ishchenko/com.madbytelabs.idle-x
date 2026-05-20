using System;
using Sound;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Event;

namespace UI.Popup.Settings
{
    internal class SettingsPopup : BasePopup
    {
        [SerializeField] private SoundElement m_soundEffect;
        [SerializeField] private SoundElement m_soundMusic;
        [SerializeField] private Button m_resetButton;

        protected new void Awake()
        {
            m_resetButton.onClick.AddListener(() =>
            {
                Node.TriggerEvent(new ShowResetProgressEventArgs());
                Close();
            });
            base.Awake();
        }

        private void OnEnable()
        {
            m_soundEffect.VolumeSlider.value = SoundSystem.Instance.EffectVolume;
            m_soundEffect.SoundValueLabel.text = $"{(int)(m_soundEffect.VolumeSlider.value * 100)}%";
            m_soundMusic.VolumeSlider.value = SoundSystem.Instance.AmbientVolume;
            m_soundMusic.SoundValueLabel.text = $"{(int)(m_soundMusic.VolumeSlider.value * 100)}%";

            m_soundEffect.VolumeSlider.onValueChanged.AddListener((volume) =>
            {
                SoundSystem.Instance.SetEffectVolume(volume);
                m_soundEffect.SoundValueLabel.text = $"{(int)(volume * 100)}%";
            });
            m_soundMusic.VolumeSlider.onValueChanged.AddListener((volume) =>
            {
                SoundSystem.Instance.SetAmbientVolume(volume);
                m_soundMusic.SoundValueLabel.text = $"{(int)(volume * 100)}%";
            });
        }

        private void OnDisable()
        {
            m_soundEffect.VolumeSlider.onValueChanged.RemoveAllListeners();
            m_soundMusic.VolumeSlider.onValueChanged.RemoveAllListeners();
        }

        [Serializable]
        class SoundElement
        {
            public Slider VolumeSlider;
            public TMP_Text SoundValueLabel;
        }
    }
}