using System.Collections.Generic;
using System.Linq;
using Data.ContentLibrary.Templates;
using Data.Enum;

namespace Data.Model
{
    public class UserBuildingsData
    {
        private readonly Dictionary<string, BuildingModel> m_buildingsMap = new();
        private readonly Dictionary<BuildingType, List<BuildingModel>> m_buildingsMapByType = new();
        private readonly Dictionary<GameResourceType, List<BuildingModel>> m_buildingsMapByResourcesType = new();
        private readonly Dictionary<GameResourceType, List<BuildingModel>> m_buildingsMapByUsResourcesType = new();
        public IReadOnlyDictionary<string, BuildingModel> BuildingsMap => m_buildingsMap;


        public bool TryGetBuildingModel(string id, out BuildingModel buildingModel)
        {
            return m_buildingsMap.TryGetValue(id, out buildingModel);
        }

        public void AddStartBuildingModel(string id, BuildingModel buildingModel)
        {
            if (m_buildingsMap.TryAdd(id, buildingModel))
            {
                AddBuildingToHelpMaps(buildingModel);
            }
        }

        public void CreateBuilding(string id, BuildingTemplate buildingTemplate)
        {
            if (m_buildingsMap.TryGetValue(id, out var buildingModel))
            {
                RemoveBuildingFromHelpMaps(buildingModel);

                buildingModel.SetTemplate(buildingTemplate);

                AddBuildingToHelpMaps(buildingModel);
            }
        }

        void AddBuildingToHelpMaps(BuildingModel buildingModel)
        {
            if (!m_buildingsMapByType.TryGetValue(buildingModel.Template.BuildingContext.BuildingType, out List<BuildingModel> buildingModels))
            {
                buildingModels = new List<BuildingModel>();
                m_buildingsMapByType.Add(buildingModel.Template.BuildingContext.BuildingType, buildingModels);
            }

            buildingModels.Add(buildingModel);

            if (buildingModel.Template.BuildingContext.BuildingProduction is { ResourcesTemplate: not null })
            {
                if (!m_buildingsMapByResourcesType.TryGetValue(buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.GameResourceType, out List<BuildingModel> byResourceModels))
                {
                    byResourceModels = new List<BuildingModel>();
                    m_buildingsMapByResourcesType.Add(buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.GameResourceType, byResourceModels);
                }

                byResourceModels.Add(buildingModel);
            }

            if (buildingModel.Template.BuildingContext.ResourcesUse != null)
            {
                foreach (var resourcesUseModel in buildingModel.Template.BuildingContext.ResourcesUse)
                {
                    if (!m_buildingsMapByUsResourcesType.TryGetValue(resourcesUseModel.GameResource.GameResourceType, out List<BuildingModel> byUseResourceModels))
                    {
                        byUseResourceModels = new List<BuildingModel>();
                        m_buildingsMapByUsResourcesType.Add(resourcesUseModel.GameResource.GameResourceType, byUseResourceModels);
                    }

                    byUseResourceModels.Add(buildingModel);
                }
            }
        }

        void RemoveBuildingFromHelpMaps(BuildingModel buildingModel)
        {
            if (m_buildingsMapByType.TryGetValue(buildingModel.Template.BuildingContext.BuildingType, out var list))
            {
                int index = list.FindIndex(e => e.Id.Equals(buildingModel.Id));
                if (index >= 0)
                {
                    list.RemoveAt(index);
                }

                if (list.Count == 0)
                    m_buildingsMapByType.Remove(buildingModel.Template.BuildingContext.BuildingType);
            }

            if (buildingModel.Template.BuildingContext.BuildingProduction is { ResourcesTemplate: not null })
            {
                if (m_buildingsMapByResourcesType.TryGetValue(buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.GameResourceType, out list))
                {
                    int index = list.FindIndex(e => e.Id.Equals(buildingModel.Id));
                    if (index >= 0)
                    {
                        list.RemoveAt(index);
                    }

                    if (list.Count == 0)
                        m_buildingsMapByResourcesType.Remove(buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.GameResourceType);
                }
            }

            if (buildingModel.Template.BuildingContext.ResourcesUse != null)
            {
                foreach (var resourcesUseModel in buildingModel.Template.BuildingContext.ResourcesUse)
                {
                    if (m_buildingsMapByUsResourcesType.TryGetValue(resourcesUseModel.GameResource.GameResourceType, out list))
                    {
                        int index = list.FindIndex(e => e.Id.Equals(buildingModel.Id));
                        if (index >= 0)
                        {
                            list.RemoveAt(index);
                        }

                        if (list.Count == 0)
                            m_buildingsMapByUsResourcesType.Remove(resourcesUseModel.GameResource.GameResourceType);
                    }
                }
            }
        }

        public bool TryGetBuildingsByType(BuildingType type, out ICollection<BuildingModel> buildings)
        {
            if (m_buildingsMapByType.TryGetValue(type, out var list))
            {
                buildings = list;
                return true;
            }

            buildings = null;
            return false;
        }

        public bool TryGetBuildingsByResourceType(GameResourceType type, out ICollection<BuildingModel> buildings)
        {
            if (m_buildingsMapByResourcesType.TryGetValue(type, out var list))
            {
                buildings = list;
                return true;
            }

            buildings = null;
            return false;
        }

        public List<GameResourceType> GetUsingResources()
        {
            return m_buildingsMapByUsResourcesType.Keys.ToList();
        }

        public List<GameResourceType> GetProductionResources()
        {
            return m_buildingsMapByResourcesType.Keys.ToList();
        }

        public bool TryGetBuildingsByUseResourceType(GameResourceType type, out ICollection<BuildingModel> buildings)
        {
            if (m_buildingsMapByUsResourcesType.TryGetValue(type, out var list))
            {
                buildings = list;
                return true;
            }

            buildings = null;
            return false;
        }
    }
}