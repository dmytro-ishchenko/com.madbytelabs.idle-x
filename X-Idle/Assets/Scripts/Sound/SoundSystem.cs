using System.Collections;
using System.Collections.Generic;
using Common.Pattern;
using UnityEngine;

namespace Sound
{
    public class SoundSystem : MonoSingleton<SoundSystem>
    {
        private SoundTemplate m_soundTemplate;
        private AudioSource m_ambientSource;
        private List<AudioSource> m_effectSources = new();
        private float m_ambientVolume = 1;
        private float m_effectVolume = 1;
        private readonly string m_effectVolumeKey = "EffectVolume";
        private readonly string m_ambientVolumeKey = "AmbientVolume";
        public float EffectVolume => m_effectVolume;
        public float AmbientVolume => m_ambientVolume;

        protected override void Awake()
        {
            base.Awake();

            m_soundTemplate = Resources.Load<SoundTemplate>("SoundTemplate");
            m_ambientSource = gameObject.AddComponent<AudioSource>();
            if (PlayerPrefs.HasKey(m_ambientVolumeKey))
                m_ambientVolume = PlayerPrefs.GetFloat(m_ambientVolumeKey);

            if (PlayerPrefs.HasKey(m_effectVolumeKey))
                m_effectVolume = PlayerPrefs.GetFloat(m_effectVolumeKey);
        }

        public void PlaySoundEffect(SoundType type)
        {
            var source = gameObject.AddComponent<AudioSource>();
            m_effectSources.Add(source);
            source.volume = m_effectVolume;

            var clip = m_soundTemplate[type];

            source.PlayOneShot(clip);
            StartCoroutine(RemoveEffectSource(source, clip.length));
        }

        public void PlayAmbientSound(SoundType type)
        {
            if (m_ambientSource.isPlaying)
                m_ambientSource.Stop();

            m_ambientSource.loop = true;
            m_ambientSource.clip = m_soundTemplate[type];
            m_ambientSource.Play();
        }

        public void SetEffectVolume(float volume)
        {
            m_effectVolume = volume;
            PlayerPrefs.SetFloat(m_effectVolumeKey, volume);
            if (m_effectSources.Count > 0)
            {
                foreach (var source in m_effectSources)
                {
                    source.volume = m_effectVolume;
                }
            }
        }

        public void SetAmbientVolume(float volume)
        {
            m_ambientVolume = volume;
            PlayerPrefs.SetFloat(m_ambientVolumeKey, volume);
            m_ambientSource.volume = m_ambientVolume;
        }

        IEnumerator RemoveEffectSource(AudioSource source, float time)
        {
            yield return new WaitForSeconds(time);
            m_effectSources.Remove(source);
            if (source.isPlaying)
                source.Stop();

            Destroy(source);
        }
    }
}