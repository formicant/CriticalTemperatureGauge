using System;
using System.Collections.Generic;
using System.Linq;
using KSP.IO;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Represents a set of add-on settings.
	/// </summary>
	/// <typeparam name="TAddon">A type attributed with KSPAddonAttribute.</typeparam>
	public class Settings<TAddon>
	{
		// Window settings
		public Vector2 SettingsWindowPosition { get; set; }
		public Vector2 GaugeWindowPosition { get; set; }
		public bool ShowAppLauncherButton { get; set; }
		public bool LockGaugeWindow { get; set; }

		// Additional information settings
		public bool ShowTemperature { get; set; }
		public bool ShowTemperatureLimit { get; set; }
		public bool ShowTemperatureRate { get; set; }
		public bool ShowCriticalPart { get; set; }
		public bool HighlightCriticalPart { get; set; }

		// Gauge visibility settings

		public bool ForceShowGauge { get; set; }

		const double DefaultGaugeShowingThreshold = 0.5;
		double _gaugeShowingThreshold;
		public double GaugeShowingThreshold
		{
			get { return _gaugeShowingThreshold; }
			set { _gaugeShowingThreshold = value > 0 && value < 1 ? value : DefaultGaugeShowingThreshold; }
		}

		const double DefaultGaugeHidingThreshold = 0.4;
		double _gaugeHidingThreshold;
		public double GaugeHidingThreshold
		{
			get { return _gaugeHidingThreshold; }
			set { _gaugeHidingThreshold = Math.Min(GaugeShowingThreshold, value > 0 && value < 1 ? value : DefaultGaugeHidingThreshold); }
		}

		const int DefaultBaseWindowId = -130716;
		int _baseWindowId;
		public int BaseWindowId
		{
			get { return _baseWindowId; }
			set { _baseWindowId = value != 0 ? value : DefaultBaseWindowId; }
		}

		// Exclusion list settings

		public bool UseExclusionList { get; set; }

		string _exclusionList;
		public string ExclusionList
		{
			get { return _exclusionList; }
			set
			{
				_exclusionList = value;
				ExclusionListItems = _exclusionList.Split(',')
					.Select(item => item.Trim())
					.Where(item => !string.IsNullOrEmpty(item))
					.ToList();
			}
		}
		public IEnumerable<string> ExclusionListItems { get; private set; }

		/// <summary>Saves the settings to an XML file inside PluginData directory.</summary>
		public void Save()
		{
			var settings = PluginConfiguration.CreateForType<TAddon>();

			settings.SetValue(nameof(SettingsWindowPosition), SettingsWindowPosition);
			settings.SetValue(nameof(GaugeWindowPosition),    GaugeWindowPosition);
			settings.SetValue(nameof(ShowAppLauncherButton),  ShowAppLauncherButton);
			settings.SetValue(nameof(LockGaugeWindow),        LockGaugeWindow);
			settings.SetValue(nameof(ForceShowGauge),         ForceShowGauge);
			settings.SetValue(nameof(GaugeShowingThreshold),  GaugeShowingThreshold);
			settings.SetValue(nameof(GaugeHidingThreshold),   GaugeHidingThreshold);
			settings.SetValue(nameof(ShowTemperature),        ShowTemperature);
			settings.SetValue(nameof(ShowTemperatureLimit),   ShowTemperatureLimit);
			settings.SetValue(nameof(ShowTemperatureRate),    ShowTemperatureRate);
			settings.SetValue(nameof(ShowCriticalPart),       ShowCriticalPart);
			settings.SetValue(nameof(HighlightCriticalPart),  HighlightCriticalPart);
			settings.SetValue(nameof(UseExclusionList),       UseExclusionList);
			settings.SetValue(nameof(ExclusionList),          ExclusionList);

			settings.save();
		}

		/// <summary>Loads settings from an XML file inside PluginData directory.</summary>
		/// <returns>Loaded settings.</returns>
		public static Settings<TAddon> Load()
		{
			var settings = PluginConfiguration.CreateForType<TAddon>();
			settings.load();

			return new Settings<TAddon>
			{
				SettingsWindowPosition = settings.GetValue(nameof(SettingsWindowPosition), Vector2.zero),
				GaugeWindowPosition    = settings.GetValue(nameof(GaugeWindowPosition),    Vector2.zero),
				ShowAppLauncherButton  = settings.GetValue(nameof(ShowAppLauncherButton),  true),
				LockGaugeWindow        = settings.GetValue(nameof(LockGaugeWindow),        true),
				ForceShowGauge         = settings.GetValue(nameof(ForceShowGauge),         false),
				GaugeShowingThreshold  = settings.GetValue(nameof(GaugeShowingThreshold),  DefaultGaugeShowingThreshold),
				GaugeHidingThreshold   = settings.GetValue(nameof(GaugeHidingThreshold),   DefaultGaugeHidingThreshold),
				ShowTemperature        = settings.GetValue(nameof(ShowTemperature),        true),
				ShowTemperatureLimit   = settings.GetValue(nameof(ShowTemperatureLimit),   true),
				ShowTemperatureRate    = settings.GetValue(nameof(ShowTemperatureRate),    true),
				ShowCriticalPart       = settings.GetValue(nameof(ShowCriticalPart),       true),
				HighlightCriticalPart  = settings.GetValue(nameof(HighlightCriticalPart),  false),
				UseExclusionList       = settings.GetValue(nameof(UseExclusionList),       false),
				ExclusionList          = settings.GetValue(nameof(ExclusionList),          ""),
			};
		}
	}
}
