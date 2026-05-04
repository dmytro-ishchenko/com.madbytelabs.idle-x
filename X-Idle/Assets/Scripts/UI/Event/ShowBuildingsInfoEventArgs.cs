using Common.Pattern.BobbleEvent;
using Data.Model;

namespace UI.Event
{
    public class ShowBuildingsInfoEventArgs: IEventArgs
    {
        public ShowBuildingsInfoEventArgs(BuildingModel buildingModel)
        {
            BuildingModel = buildingModel;
        }

        public BuildingModel BuildingModel { get;  }
    }
}