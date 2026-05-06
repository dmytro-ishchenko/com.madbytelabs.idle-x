using UnityEngine;

namespace Data.Model.Popup
{
    public class InfoElementModel
    {
        public InfoElementModel(string value)
        {
            Icon = null;
            Text = string.Empty;
            Value = value;
        }

        public InfoElementModel(Sprite icon, string text, string value)
        {
            Icon = icon;
            Text = text;
            Value = value;
        }

        public Sprite Icon { get; }
        public string Text { get; }
        public string Value { get; }
    }
}