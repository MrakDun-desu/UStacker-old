using System.Drawing;
using Color = UnityEngine.Color;

namespace Blockstacker.Common
{
    public static class CreateColor
    {
        public static Color From255Range(float r, float g, float b, float a = 0)
        {
            return new Color(r / 255, g / 255, b / 255, a / 255);
        }

        public static Color FromString(string htmlString)
        {
            System.Drawing.Color systemColor;
            try
            {
                systemColor = ColorTranslator.FromHtml(htmlString);
            }
            catch
            {
                systemColor = System.Drawing.Color.White;
            }


            return htmlString.Length <= 7
                ? new Color(systemColor.R / 255f, systemColor.G / 255f, systemColor.B / 255f)
                : new Color(systemColor.A / 255f, systemColor.R / 255f, systemColor.G / 255f, systemColor.B / 255f);
        }
    }
}