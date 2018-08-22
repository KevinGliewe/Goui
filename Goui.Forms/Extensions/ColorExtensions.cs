using System;

namespace Goui.Forms.Extensions
{
	public static class ColorExtensions
	{
		static Color ToGouiColor (ref Xamarin.Forms.Color color)
		{
            byte r = (byte)(color.R * 255.0 + 0.5);
            byte g = (byte)(color.G * 255.0 + 0.5);
            byte b = (byte)(color.B * 255.0 + 0.5);
            byte a = (byte)(color.A * 255.0 + 0.5);
            return new Color (r, g, b, a);
		}

        public static Color ToGouiColor (this Xamarin.Forms.Color color, Xamarin.Forms.Color defaultColor)
        {
            if (color == Xamarin.Forms.Color.Default)
                return ToGouiColor (ref defaultColor);
            return ToGouiColor (ref color);
        }

        public static Color ToGouiColor (this Xamarin.Forms.Color color, Goui.Color defaultColor)
        {
            if (color == Xamarin.Forms.Color.Default)
                return defaultColor;
            return ToGouiColor (ref color);
        }
	}
}
