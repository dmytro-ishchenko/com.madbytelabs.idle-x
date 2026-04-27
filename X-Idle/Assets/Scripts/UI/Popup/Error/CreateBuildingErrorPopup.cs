using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using TMPro;
using UI.Model;
using UI.Popup.Upgrade;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Error
{
    internal class CreateBuildingErrorPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private Image m_icon;
        [SerializeField] private SerializedDictionary<RequireElementType, RequiredElementView> m_requiredElements;
        [SerializeField] private Transform m_root;

        private readonly List<RequiredElementView> m_requirementViews = new();

        public override void Show<T>(T context)
        {
            if (context is CreateBuildingErrorContext model)
            {
                m_name.text = model.BuildingTemplate.Name;
                m_icon.sprite = model.BuildingTemplate.Icon;
                int elementIndex = 1;

                List<bool> requirementsFollowList = new List<bool>();


                foreach (var element in model.BuildingTemplate.BuildingContext.BuildingRequirements)
                {
                    if (model.UserBuildingsData.TryGetBuildingsByType(element.BuildingType, out var requiredBuildings))
                    {
                        bool buildingFollow = false;

                        foreach (var requiredElement in requiredBuildings)
                        {
                            buildingFollow = false;
                            if (requiredElement.Level >= element.Level)
                            {
                                buildingFollow = true;
                            }

                            requirementsFollowList.Add(buildingFollow);
                        }

                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Building], m_root);
                        requirementElement.Init(buildingFollow, $"{requiredBuildings.ElementAt(0).Template.Name} Level: {element.Level}");

                        requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                        rectTransform.SetSiblingIndex(elementIndex);

                        m_requirementViews.Add(requirementElement);

                        elementIndex += 1;
                    }
                }


                foreach (var element in model.BuildingTemplate.BuildingContext.UpgradeCostModel.CostModels)
                {
                    bool resourceFollow = false;
                    float requiredResource = element.Cost * model.BuildingTemplate.BuildingContext.BuildingProduction.LevelMultiplier;

                    if (model.UserResources.GetGameResourceValue(element.GameResourceType) > requiredResource)
                    {
                        resourceFollow = true;
                    }

                    requirementsFollowList.Add(resourceFollow);

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