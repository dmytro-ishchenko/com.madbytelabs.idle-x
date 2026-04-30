using System.Collections.Generic;

namespace Data.Model
{
    internal class UserPlaceHolderData
    {
        private readonly Dictionary<string, PlaceholderModel> m_placeHolderMap = new();
        public IReadOnlyDictionary<string, PlaceholderModel> PlaceHolderDataMap => m_placeHolderMap;

        internal void AddPlaceHolder(string id, PlaceholderModel placeHolderModel)
        {
            m_placeHolderMap.Add(id, placeHolderModel);
        }

        internal void UpdatePlaceHolderStatus(string id, PlaceholderModel placeHolderModel)
        {
            m_placeHolderMap[id] = placeHolderModel;
        }

        public PlaceholderModel GetPlaceHolderStatus(string id)
        {
            return m_placeHolderMap[id];
        }
    }
}