using System;
using System.Collections.Generic;
using System.Linq;
using KSP.Localization;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Static class containing miscellaneous static objects.
	/// </summary>
	public static class Static
	{
		/// <summary>Identifier of the plugin.</summary>
		public const string PluginId = nameof(CriticalTemperatureGauge);

		/// <summary>User-friendly name of the plugin.</summary>
		public static readonly string PluginTitle = Localizer.Format("#LOC_CriticalTemperatureGauge_Title");

		/// <summary>Part module field category id.</summary>
		public const string FieldCategory = "Temperature";

		/// <summary>Path to the Textures folder inside the add-on folder.</summary>
		public const string TexturePath = PluginId + "/Textures/";

		/// <summary>Add-on settings.</summary>
		public static Settings<Flight> Settings => _settings ?? (_settings = Settings<Flight>.Load());
		static Settings<Flight> _settings;

		/// <summary>Current critical part state.</summary>
		/// <remarks><c>null</c> if there is no current critical part.</remarks>
		public static PartTemperatureState CriticalPartState { get; set; }

		/// <summary>A ‘random’ number used as Id of the mod windows.</summary>
		public const int BaseWindowId = -130716;
	}
}
