using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Статические методы для работы с цветом
    /// </summary>
    public static class ColorTools
    {
        public static Color GetRandomHSVColor()
        {
            return Random.ColorHSV(0f, 0.69f, 0.95f, 1f, 1f, 1f);
        }

        public static Color GetDarkerColor(Color color, float darkFactor)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            return Color.HSVToRGB(h, s, v * darkFactor);
        }
    }
}