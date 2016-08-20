using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Static class containing miscellaneous static objects.
	/// </summary>
	public static class Static
	{
		/// <summary>Path to the Textures folder inside the add-on folder.</summary>
		public const string TexturePath = nameof(CriticalTemperatureGauge) + "/Textures/";

		static Settings<Flight> _settings;
		/// <summary>Add-on settings.</summary>
		public static Settings<Flight> Settings => _settings ?? (_settings = Settings<Flight>.Load());

		/// <summary>Current critical part state.</summary>
		/// <remarks><c>null</c> if there is no current critical part.</remarks>
		public static PartState CriticalPartState { get; set; }
	}
}
