using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Data.Enum;
using Data.Model;
using TMPro;
using UI.Model;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Upgrade
{
    internal class UpgradeBuildingPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_level;
        [SerializeField] private SerializedDictionary<RequireElementType, RequiredElementView> m_requiredElements;
        [SerializeField] private Transform m_root;
        [SerializeField] private Button m_upgrade;

        private readonly List<RequiredElementView> m_requirementViews = new();

        public override void Show<T>(T context)
        {
            if (context is UpgradeBuildingContext model)
            {
                m_name.text = model.BuildingModel.Template.Name;
                m_level.text = model.BuildingModel.Level.ToString();
                m_icon.sprite = model.BuildingModel.Template.Icon;
                int elementIndex = 1;
                if (model.BuildingModel.Template.BuildingContext.BuildingType != BuildingType.MainBuilding)
                {
                    foreach (var element in model.BuildingModel.Template.BuildingContext.BuildingRequirements)
                    {
                        if (model.UserBuildingsData.TryGetBuildingsByType(element.BuildingType, out var requiredBuildings))
                        {
                            bool buildingFollow = false;

                            foreach (var requiredElement in requiredBuildings)
                            {
                                if (requiredElement.Level >= element.Level)
                                {
                                    buildingFollow = true;
                                }
                            }

                            var requirementElement = Instantiate(m_requiredElements[RequireElementType.Building], m_root);
                            requirementElement.Init(buildingFollow, $"{requiredBuildings.ElementAt(0).Template.Name} Level: {element.Level}");

                            requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                            rectTransform.SetSiblingIndex(elementIndex);

                            m_requirementViews.Add(requirementElement);

                            elementIndex += 1;
                        }
                    }
                }

                foreach (var element in model.BuildingModel.Template.BuildingContext.UpgradeCostModel.CostModels)
                {
                    bool resourceFollow = false;

                    float requiredResource = element.Cost * (model.BuildingModel.Level) * model.BuildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier;

                    if (model.UserResources.GetGameResourceValue(element.GameResourceType) > requiredResource)
                    {
                        resourceFollow = true;
                    }

                    var requirementElement = Instantiate(m_requiredElements[RequireElementType.Resource], m_root);
                    requirementElement.Init(resourceFollow, $"{element.GameResourceType} : {requiredResource}");

                    requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                    rectTransform.SetSiblingIndex(elementIndex);

                    m_requirementViews.Add(requirementElement);

                    elementIndex += 1;
                }

                base.Show(context);
            }
        }

        public override void Close()
        {
            foreach (var requirementView in m_requirementViews)
            {
                Destroy(requirementView.gameObject);
            }

            m_requirementViews.Clear();

            base.Close();
        }
    }
}