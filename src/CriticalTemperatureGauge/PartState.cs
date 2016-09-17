using System;
using System.Collections.Generic;
using System.Linq;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Parameters of a part.
	/// </summary>
	public class PartState
	{
		/// <summary>Moment of time the parameters were measured at.</summary>
		public double Time { get; set; }

		/// <summary>Vessel part.</summary>
		public Part Part { get; set; }

		/// <summary>Vessel part Id.</summary>
		public uint Id { get; set; }

		/// <summary>Part short name.</summary>
		public string Name { get; set; }

		// Thermal parameters
		public double CoreTemperatureLimit { get; set; }
		public double SkinTemperatureLimit { get; set; }
		public double CoreTemperature { get; set; }
		public double SkinTemperature { get; set; }
		public double CoreTemperatureRate { get; set; }
		public double SkinTemperatureRate { get; set; }

		// Critical thermal parameters
		public bool IsSkinCritical => CoreTemperature / CoreTemperatureLimit < SkinTemperature / SkinTemperatureLimit;
		public double CriticalTemperatureLimit => IsSkinCritical ? SkinTemperatureLimit : CoreTemperatureLimit;
		public double CriticalTemperature => IsSkinCritical ? SkinTemperature : CoreTemperature;
		public double CriticalTemperatureRate => IsSkinCritical ? SkinTemperatureRate : CoreTemperatureRate;

		double? _index;
		/// <summary>A number between 0 and 1 showing how critical the part is.</summary>
		public double Index => _index ?? (_index =
			Math.Max(CoreTemperature / CoreTemperatureLimit, SkinTemperature / SkinTemperatureLimit)).Value;

		/// <summary>Calculates temperature rates.</summary>
		/// <param name="previousState">The part's state in the previous moment of time.</param>
		public void UpdateTemperatureRates(PartState previousState)
		{
			if(previousState != null && previousState.Id == Id)
			{
				double timeSpan = 2 * (Time - previousState.Time);
				CoreTemperatureRate = timeSpan > 0
					? (1D - CoreTemperatureRateSmoothing) * previousState.CoreTemperatureRate +
						CoreTemperatureRateSmoothing * (CoreTemperature - previousState.CoreTemperature) / timeSpan
					: previousState.CoreTemperatureRate;
				SkinTemperatureRate = timeSpan > 0
					? (1D - SkinTemperatureRateSmoothing) * previousState.SkinTemperatureRate +
						SkinTemperatureRateSmoothing * (SkinTemperature - previousState.SkinTemperature) / timeSpan
					: previousState.SkinTemperatureRate;
			}
		}

		// Temperature rate value temporal smoothing.
		const double CoreTemperatureRateSmoothing = 0.5;
		const double SkinTemperatureRateSmoothing = 0.1;
	}
}
