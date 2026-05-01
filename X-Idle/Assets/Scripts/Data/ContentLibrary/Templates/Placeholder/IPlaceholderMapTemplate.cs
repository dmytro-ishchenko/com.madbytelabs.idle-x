using Data.Enum;

namespace Data.ContentLibrary.Templates.Placeholder
{
    public interface IPlaceholderMapTemplate
    {
        bool TryGetRequirementsByType(PlaceHolderType type, out PlaceholderStatusRequirementModel requirementModel);
    }
}