using System;
using System.Collections.Generic;
using Data.Enum;

namespace Data.Model
{
    public class UserResources
    {
        internal event Action<UserResources> OnUserResourcesChanged;

        private readonly Dictionary<GameResourceType, float> m_resources = new Dictionary<GameResourceType, float>();

        public float GetGameResourceValue(GameResourceType type)
        {
            return m_resources.GetValueOrDefault(type, 0f);
        }

        public void SetGameResource(GameResourceType type, float value)
        {
            if (value < 0)
                value = 0;

            if (!m_resources.TryGetValue(type, out _))
                m_resources.Add(type, value);
            else
                m_resources[type] = value;
        }

        public void InvokeUpdateResources()
        {
            OnUserResourcesChanged?.Invoke(this);
        }
    }
}