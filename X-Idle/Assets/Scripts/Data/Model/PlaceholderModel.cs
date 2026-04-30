using Data.Enum;
using UnityEngine;

namespace Data.Model
{
    public class PlaceholderModel
    {
        public PlaceholderModel(string id, PlaceHolderStatus status)
        {
            Id = id;
            Status = status;
            Transform = null;
        }

        public string Id { get; }
        public PlaceHolderStatus Status { get; }
        public Transform Transform { get; private set; }

        public void SetTransform(Transform transform)
        {
            Transform = transform;
        }
    }
}