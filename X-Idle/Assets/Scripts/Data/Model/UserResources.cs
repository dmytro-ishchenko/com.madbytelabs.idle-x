using System;

namespace Data.Model
{
    public class UserResources
    {
        internal event Action<UserResources> OnUserResourcesChanged;
        public float Scrap { get; private set; }
        public float Energy { get; private set; }
        public float Water { get; private set; }
        public float Food { get; private set; }
        public float Data { get; private set; }
        public float Parts { get; private set; }

        internal void SetScrap(float value)
        {
            Scrap = value;
            OnUserResourcesChanged?.Invoke(this);
        }

        internal void SetEnergy(float value)
        {
            Energy = value;
            OnUserResourcesChanged?.Invoke(this);
        }

        internal void SetWater(float value)
        {
            Water = value;
            OnUserResourcesChanged?.Invoke(this);
        }

        internal void SetFood(float value)
        {
            Food = value;
            OnUserResourcesChanged?.Invoke(this);
        }

        internal void SetData(float value)
        {
            Data = value;
        }

        internal void SetParts(float value)
        {
            Parts = value;
        }
    }
}