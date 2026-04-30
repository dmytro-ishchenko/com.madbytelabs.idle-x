using Data.Enum;

namespace Data.Model
{
    public class BuildingContextModel
    {
        public BuildingContextModel(BuildingType buildingType, string name, int level, int requireLevel)
        {
            BuildingType = buildingType;
            Name = name;
            Level = level;
            RequireLevel = requireLevel;
        }

        public BuildingType BuildingType { get; }
        public string Name { get; }
        public int Level { get; }
        public int RequireLevel { get; }
    }
}