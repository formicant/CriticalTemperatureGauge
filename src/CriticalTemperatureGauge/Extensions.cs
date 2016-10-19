using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CriticalTemperatureGauge
{
	internal static class Extensions
	{
		public static string ToUnsignedString(this double value, int intPlaces, int fracPlaces)
		{
			var numberString = value.ToString("F" + fracPlaces, CultureInfo.InvariantCulture);
			var spaces = Math.Max(0, intPlaces + fracPlaces + (fracPlaces > 0 ? 1 : 0) - numberString.Length);
			return new string(' '/* U+2007 Figure Space */, spaces) + numberString;
		}

		public static string ToSignedString(this double value, int intPlaces, int fracPlaces)
		{
			var numberString = Math.Abs(value).ToString("F" + fracPlaces, CultureInfo.InvariantCulture);
			var signString = value < 0 ? "−"/* U+2212 Minus Sign */ : "+";
			var spaces = Math.Max(0, intPlaces + fracPlaces + (fracPlaces > 0 ? 1 : 0) - numberString.Length);
			return new string(' '/* U+2007 Figure Space */, spaces) + signString + numberString;
		}
	}
}
