using UnityEngine;

namespace Blockstacker.Common
{
    public static class CreateColor
    {
        public static Color From255Range(float r, float g, float b, float a = 0)
        {
            return new Color(r / 255, g / 255, b / 255, a / 255);
        }
    }
}