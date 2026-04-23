using System.Collections.Generic;
using Data.ContentLibrary.Templates.Context;
using UnityEngine;

namespace Data.ContentLibrary.Templates
{
    public class SceneTemplate : ScriptableObject
    {
        [SerializeField] private List<SceneBuildingContext> sceneBuildings = new();
        public List<SceneBuildingContext> SceneBuildings => sceneBuildings;

        public void AddSceneBuilding(SceneBuildingContext sceneBuildingContext)
        {
            sceneBuildings.Add(sceneBuildingContext);
        }
    }
}