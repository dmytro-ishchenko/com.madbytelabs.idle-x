using UnityEngine;

namespace Data.Model.Popup
{
    public class InfoElementModel
    {
        public InfoElementModel(Sprite icon, string text, string value, bool isPositive)
        {
            Icon = icon;
            Text = text;
            Value = value;
            IsPositive = isPositive;
        }

        public Sprite Icon { get; }
        public string Text { get; }
        public string Value { get; }
        public bool IsPositive { get; }
    }
}