using AYellowpaper.SerializedCollections;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Placeholder
{
    public class PlaceholderMapTemplate:ScriptableObject, IPlaceholderMapTemplate
    {
        [SerializeField] SerializedDictionary<PlaceHolderType, PlaceholderStatusRequirementModel>  m_placeholdersMap;

        public bool TryGetRequirementsByStatus(PlaceHolderType type, out PlaceholderStatusRequirementModel requirementModel)
        {
            return m_placeholdersMap.TryGetValue(type, out requirementModel);
        }
    }
}