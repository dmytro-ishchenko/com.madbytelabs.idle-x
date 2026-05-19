using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Sound
{
    public class SoundTemplate : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<SoundType, AudioClip> m_sounds;
        public AudioClip this[SoundType soundType] => m_sounds[soundType];
    }
}