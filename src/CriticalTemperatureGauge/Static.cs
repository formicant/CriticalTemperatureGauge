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
		/// <summary>User-friendly name of the plugin.</summary>
		public static readonly string PluginTitle = Localizer.Format("#ModCriticalTemperatureGauge_Title");

		/// <summary>Path to the Textures folder inside the add-on folder.</summary>
		public const string TexturePath = nameof(CriticalTemperatureGauge) + "/Textures/";

		/// <summary>Add-on settings.</summary>
		public static Settings<Flight> Settings => _settings ?? (_settings = Settings<Flight>.Load());
		static Settings<Flight> _settings;

		/// <summary>AppLauncher button.</summary>
		public static AppLauncher AppLauncher { get; set; }

		/// <summary>Current critical part state.</summary>
		/// <remarks><c>null</c> if there is no current critical part.</remarks>
		public static PartTemperatureState CriticalPartState { get; set; }

		/// <summary>A ‘random’ number used as Id of the mod windows.</summary>
		public const int BaseWindowId = -130716;
	}
}
