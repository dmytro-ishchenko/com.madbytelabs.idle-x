using System.Collections.Generic;

namespace Data.Model.Popup
{
    public class OfflineReward : IShowPopupContext
    {
        public OfflineReward(long time, List<ResourceRewardModel> resourceRewardModels)
        {
            Time = time;
            ResourceRewardModels = resourceRewardModels;
        }

        public long Time { get; }
        public List<ResourceRewardModel> ResourceRewardModels { get; }
    }
}