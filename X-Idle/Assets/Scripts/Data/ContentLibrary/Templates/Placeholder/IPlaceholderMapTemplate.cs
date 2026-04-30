using Data.Enum;

namespace Data.ContentLibrary.Templates.Placeholder
{
    public interface IPlaceholderMapTemplate
    {
        bool TryGetRequirementsByStatus(PlaceHolderType type, out PlaceholderStatusRequirementModel requirementModel);
    }
}