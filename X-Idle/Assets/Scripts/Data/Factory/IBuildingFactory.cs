using Data.Model;

namespace Data.Factory
{
    internal interface IBuildingFactory
    {
        
        BuildingModel CreateBuilding(string templateId);
    }
}