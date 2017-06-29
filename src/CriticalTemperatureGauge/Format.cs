using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KSP.Localization;

namespace CriticalTemperatureGauge
{
	internal static class Format
	{
		public static string Temperature(double temperature, double? temperatureLimit) =>
			temperatureLimit.HasValue
				? $"{temperature.ToUnsignedString(4)} / {temperatureLimit.Value.ToUnsignedString(3)} {Kelvin}"
				: $"{temperature.ToUnsignedString(4)} {Kelvin}";

		public static string TemperatureRate(double temperatureRate) =>
			$"   {temperatureRate.ToSignedString(1)} {KelvinPerSecond}";

		public static string TemperatureWithRate(double temperature, double? temperatureLimit, double? temperatureRate) =>
			temperatureRate.HasValue
				? $"{Temperature(temperature, temperatureLimit)} {temperatureRate.Value.ToSignedString(3, "–"/* U+2013 En Dash */)} {KelvinPerSecond}"
				: $"{Temperature(temperature, temperatureLimit)}";

		public static string PartName(string partName, bool isSkin) =>
			$" {partName} ({(isSkin ? Skin : Core)})";


		public static T? Then<T>(this bool condition, T thenValue)
			where T : struct
			=>
			condition ? thenValue : (T?)null;


		static string ToUnsignedString(this double value, int intPlaces)
		{
			var numberString = value.ToString("F0", CultureInfo.InvariantCulture);
			var spaces = Math.Max(0, intPlaces - numberString.Length);
			return new string(' '/* U+2007 Figure Space */, spaces) + numberString;
		}

		static string ToSignedString(this double value, int intPlaces, string minus = "−"/* U+2212 Minus Sign */, string plus = "+")
		{
			var numberString = Math.Abs(value).ToString("F0", CultureInfo.InvariantCulture);
			var signString = value < 0 ? minus : plus;
			var spaces = Math.Max(0, intPlaces - numberString.Length);
			return new string(' '/* U+2007 Figure Space */, spaces) + signString + numberString;
		}


		static readonly string Kelvin = Localizer.Format("#LOC_CriticalTemperatureGauge_Kelvin");
		static readonly string KelvinPerSecond = Localizer.Format("#LOC_CriticalTemperatureGauge_KelvinPerSecond");
		static readonly string Core = Localizer.Format("#LOC_CriticalTemperatureGauge_Core");
		static readonly string Skin = Localizer.Format("#LOC_CriticalTemperatureGauge_Skin");
	}
}
