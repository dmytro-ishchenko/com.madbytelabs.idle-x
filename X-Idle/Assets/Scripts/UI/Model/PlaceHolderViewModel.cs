using Data.Enum;
using UnityEngine;

namespace UI.Model
{
    internal class PlaceHolderViewModel
    {
        public PlaceHolderViewModel(string id, PlaceHolderStatus status, GameObject view, Transform targetTransform)
        {
            View = view;
            TargetTransform = targetTransform;
            Status = status;
            Id = id;
        }

        public string Id { get; }
        public PlaceHolderStatus Status { get; }
        public GameObject View { get; }
        public Transform TargetTransform { get; }
    }
}