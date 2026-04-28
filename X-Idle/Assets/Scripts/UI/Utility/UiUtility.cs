using System;
using System.Globalization;

namespace UI.Utility
{
    internal static class UiUtility
    {
        public static string ValueToString(float value)
        {
            value = (float)Math.Round(value);

            if (value < 1000)
                return value.ToString(CultureInfo.InvariantCulture);


            double shortened = value;
            int suffixIndex = -1;

            while (shortened >= 1000)
            {
                shortened /= 1000;
                suffixIndex++;
            }

            double rounded = shortened < 10 ? Math.Round(shortened, 1) : Math.Round(shortened, 0);

            if (rounded >= 1000)
            {
                rounded /= 1000;
                suffixIndex++;
            }

            string number = rounded < 10 ? rounded.ToString("0.#") : rounded.ToString("0");

            string suffix = string.Empty;
            switch (suffixIndex)
            {
                case 0:
                    suffix = "K";
                    break;
                case 1:
                    suffix = "M";
                    break;
                case 2:
                    suffix = "B";
                    break;
                case 3:
                    suffix = "T";
                    break;
                case 4:
                    suffix = "Q";
                    break;
                case 5:
                    suffix = "G";
                    break;
            }

            return $"{number}{suffix}";
        }
    }
}