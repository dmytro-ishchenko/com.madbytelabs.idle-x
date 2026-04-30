using Data.Enum;

namespace Data.Model
{
    public class ResourceContextModel
    {
        public ResourceContextModel(GameResourceType resourceType, string name, float count, float requireAmount)
        {
            Name = name;
            Count = count;
            RequireAmount = requireAmount;
            ResourceType = resourceType;
        }

        public GameResourceType ResourceType { get; }
        public string Name { get; }

        public float Count { get; }

        public float RequireAmount { get; }
    }
}