using UnityEngine;

namespace AiryCat.UtilitiesForUnity.Utility
{
    public static class HexColor
    {
        public static Color ColorFromHex(string hex)
        {
            var red = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            var green = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            var blue = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            return new Color((float) red / 255, (float) green / 255, (float) blue / 255);
        }
    }
}