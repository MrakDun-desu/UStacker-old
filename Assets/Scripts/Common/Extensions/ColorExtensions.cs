using UnityEngine;

namespace Blockstacker.Common.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float newAlpha)
        {
            return new(color.r, color.g, color.b, newAlpha);
        }

        public static Color WithValue(this Color color, Color other)
        {
            return new(other.r, other.g, other.b, color.a);
        }
    }
}