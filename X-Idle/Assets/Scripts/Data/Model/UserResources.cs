using System;

namespace Data.Model
{
    public class UserResources
    {
        internal event Action<UserResources> OnUserResourcesChanged;
        public int Scrap { get; private set; }
        public int Energy { get; private set; }
        public int Water { get; private set; }
        public int Food { get; private set; }

        internal void SetScrap(int value)
        {
            Scrap = value;
            OnUserResourcesChanged?.Invoke(this);
        }

        internal void SetEnergy(int value)
        {
            Energy = value;
            OnUserResourcesChanged?.Invoke(this);
        }

        internal void SetWater(int value)
        {
            Water = value;
            OnUserResourcesChanged?.Invoke(this);
        }

        internal void SetFood(int value)
        {
            Food = value;
            OnUserResourcesChanged?.Invoke(this);
        }
    }
}