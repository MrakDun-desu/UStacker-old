
/************************************
ColorExtensions.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;

namespace UStacker.Common.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float newAlpha)
        {
            return new Color(color.r, color.g, color.b, newAlpha);
        }

        public static Color WithValue(this Color color, Color other)
        {
            return new Color(other.r, other.g, other.b, color.a);
        }
    }
}
/************************************
end ColorExtensions.cs
*************************************/
