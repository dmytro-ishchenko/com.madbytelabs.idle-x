using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Common;
using Data.ContentLibrary;
using Data.Enum;
using Data.Interface;
using Data.Model;
using UnityEditor;

namespace Data
{
    internal class ApplicationData : IApplicationData
    {
        private IAssetLibrary m_assetLibrary;
        private UserData m_userData = new UserData();

        public IAssetLibrary AssetLibrary
        {
            get
            {
                if (m_assetLibrary == null)
                    m_assetLibrary = (AssetLibrary)AssetDatabase.LoadAssetAtPath(AssetPath.LIBRARY_PATH, typeof(AssetLibrary));
                return m_assetLibrary;
            }
        }

        public IList<IBuildingTemplate> GetAvailableBuilding()
        {
            var userBuildings = m_userData.UserBuildingsData.BuildingsMap.Values.ToList();

            if (userBuildings.Count == 0)
            {
                if (AssetLibrary.TryGetBuildingTemplate(BuildingType.MainBuilding, out var template))
                {
                    return new BindingList<IBuildingTemplate>() { template };
                }
            }

            IList<IBuildingTemplate> templates = new List<IBuildingTemplate>();


            foreach (var buildingTemplate in AssetLibrary.Buildings)
            {
                var userBuilding = userBuildings.FirstOrDefault(b => b.Template.BuildingContext.BuildingType.Equals(buildingTemplate.BuildingContext.BuildingType));

                if (userBuilding == null || userBuilding.Template.BuildingContext.CanDuplicate)
                {
                    templates.Add(buildingTemplate);
                }
            }

            return templates;
        }
    }
}