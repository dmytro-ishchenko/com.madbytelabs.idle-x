using Data.ContentLibrary.Templates;
using Data.Model;


namespace Data.Utility
{
    public static class DataUtility
    {
        public static bool CanCreateBuilding(UserBuildingsData buildingsData, BuildingTemplate template)
        {
            var buildRequirements = template.BuildingContext.BuildingRequirements;

            if (buildRequirements is { Count: > 0 })
            {
                foreach (var element in buildRequirements)
                {
                    if (!buildingsData.TryGetBuildingsByType(element.BuildingType, out var requiredBuildings))
                    {
                        return false;
                    }

                    foreach (var model in requiredBuildings)
                    {
                        if (model.Level >= element.Level)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            return true;
        }
    }
}