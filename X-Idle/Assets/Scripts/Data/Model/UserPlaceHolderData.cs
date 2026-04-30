using System;
using System.Collections.Generic;

namespace Data.Model
{
    internal class UserPlaceHolderData
    {
        private readonly Dictionary<string, PlaceholderModel> m_placeHolderMap = new();
        public IReadOnlyDictionary<string, PlaceholderModel> PlaceHolderDataMap => m_placeHolderMap;

        public event Action<PlaceholderModel> OnPlaceHolderStatusChanged;

        internal void AddPlaceHolder(string id, PlaceholderModel placeHolderModel)
        {
            m_placeHolderMap.Add(id, placeHolderModel);
        }

        internal void UpdatePlaceHolderStatus(string id, PlaceholderModel placeHolderModel)
        {
            m_placeHolderMap[id] = placeHolderModel;
            OnPlaceHolderStatusChanged?.Invoke(placeHolderModel);
        }
    }
}