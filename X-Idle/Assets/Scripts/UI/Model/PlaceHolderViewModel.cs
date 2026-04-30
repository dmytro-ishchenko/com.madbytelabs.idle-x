using Data.Enum;
using UI.Controller;
using UnityEngine;

namespace UI.Model
{
    internal class PlaceHolderViewModel
    {
        public PlaceHolderViewModel(string id, PlaceHolderStatus status, PlaceHolderViewElement view, Transform targetTransform)
        {
            View = view;
            TargetTransform = targetTransform;
            Status = status;
            Id = id;
        }

        public string Id { get; }
        public PlaceHolderStatus Status { get; }
        public PlaceHolderViewElement View { get; }
        public Transform TargetTransform { get; }
    }
}