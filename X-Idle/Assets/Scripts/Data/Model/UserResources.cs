using System;
using System.Collections.Generic;
using Data.Enum;

namespace Data.Model
{
    public class UserResources
    {
        internal event Action<UserResources> OnUserResourcesChanged;

        Dictionary<GameResourceType, float> m_resources = new Dictionary<GameResourceType, float>();

        public float GetGameResourceValue(GameResourceType type)
        {
            return m_resources.GetValueOrDefault(type, 0f);
        }

        public void SetGameResource(GameResourceType type, float value)
        {
            if (!m_resources.TryGetValue(type, out _))
                m_resources.Add(type, value);
            else
                m_resources[type] = value;
            OnUserResourcesChanged?.Invoke(this);
        }
    }
}