using System.Collections.Generic;
using Data.Enum;

namespace Data.Model.Popup
{
    public class OfflineReward : IShowPopupContext
    {
        public OfflineReward(long time, List<ResourceRewardModel> resourceRewardModels, List<StorageReachLimitModel> resourcesReachLimit)
        {
            Time = time;
            ResourceRewardModels = resourceRewardModels;
            ResourcesReachLimit = resourcesReachLimit;
        }

        public long Time { get; }
        public List<ResourceRewardModel> ResourceRewardModels { get; }
        public List<StorageReachLimitModel> ResourcesReachLimit { get; }
    }
}