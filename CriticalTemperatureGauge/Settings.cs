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
		public float GaugeWindowScale { get; set; }
		public bool ShowAppLauncherButton { get; set; }
		public bool DockGaugeWindow { get; set; }
		public bool LockGaugeWindow { get; set; }

		// Additional information settings
		public bool ShowTemperature { get; set; }
		public bool ShowTemperatureLimit { get; set; }
		public bool ShowTemperatureRate { get; set; }
		public bool ShowCriticalPart { get; set; }
		public bool HighlightCriticalPart { get; set; }

		// Part menu settings
		public bool PartMenuTemperature { get; set; }
		public bool PartMenuTemperatureLimit { get; set; }
		public bool PartMenuTemperatureRate { get; set; }

		// Gauge visibility settings

		public bool AlwaysShowGauge { get; set; }

		const double DefaultGaugeShowingThreshold = 0.5;
		double _gaugeShowingThreshold;
		public double GaugeShowingThreshold
		{
			get => _gaugeShowingThreshold;
			set => _gaugeShowingThreshold = value > 0 && value < 1 ? value : DefaultGaugeShowingThreshold;
		}

		const double DefaultGaugeHidingThreshold = 0.4;
		double _gaugeHidingThreshold;
		public double GaugeHidingThreshold
		{
			get => _gaugeHidingThreshold;
			set => _gaugeHidingThreshold = Math.Min(GaugeShowingThreshold, value > 0 && value < 1 ? value : DefaultGaugeHidingThreshold);
		}

		// Exclusion list settings

		public bool UseExclusionList { get; set; }

		string _exclusionList;
		public string ExclusionList
		{
			get => _exclusionList;
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

		/// <summary>Saves the settings to the XML file inside PluginData directory.</summary>
		public void Save()
		{
			var settings = PluginConfiguration.CreateForType<TAddon>();

			settings.SetValue(nameof(SettingsWindowPosition),   SettingsWindowPosition);
			settings.SetValue(nameof(GaugeWindowPosition),      GaugeWindowPosition);
			settings.SetValue(nameof(GaugeWindowScale),         GaugeWindowScale);
			settings.SetValue(nameof(DockGaugeWindow),          DockGaugeWindow);
			settings.SetValue(nameof(LockGaugeWindow),          LockGaugeWindow);
			settings.SetValue(nameof(AlwaysShowGauge),          AlwaysShowGauge);
			settings.SetValue(nameof(GaugeShowingThreshold),    GaugeShowingThreshold);
			settings.SetValue(nameof(GaugeHidingThreshold),     GaugeHidingThreshold);
			settings.SetValue(nameof(ShowTemperature),          ShowTemperature);
			settings.SetValue(nameof(ShowTemperatureLimit),     ShowTemperatureLimit);
			settings.SetValue(nameof(ShowTemperatureRate),      ShowTemperatureRate);
			settings.SetValue(nameof(ShowCriticalPart),         ShowCriticalPart);
			settings.SetValue(nameof(HighlightCriticalPart),    HighlightCriticalPart);
			settings.SetValue(nameof(PartMenuTemperature),      PartMenuTemperature);
			settings.SetValue(nameof(PartMenuTemperatureLimit), PartMenuTemperatureLimit);
			settings.SetValue(nameof(PartMenuTemperatureRate),  PartMenuTemperatureRate);
			settings.SetValue(nameof(UseExclusionList),         UseExclusionList);
			settings.SetValue(nameof(ExclusionList),            ExclusionList);

			settings.save();
		}

		/// <summary>Loads settings from the XML file inside PluginData directory.</summary>
		/// <returns>Loaded settings.</returns>
		public static Settings<TAddon> Load()
		{
			var settings = PluginConfiguration.CreateForType<TAddon>();
			settings.load();

			return new Settings<TAddon>
			{
				SettingsWindowPosition   = settings.GetValue(nameof(SettingsWindowPosition),   Vector2.zero),
				GaugeWindowPosition      = settings.GetValue(nameof(GaugeWindowPosition),      Vector2.zero),
				GaugeWindowScale         = settings.GetValue(nameof(GaugeWindowScale),         1),
				DockGaugeWindow          = settings.GetValue(nameof(DockGaugeWindow),          true),
				LockGaugeWindow          = settings.GetValue(nameof(LockGaugeWindow),          true),
				AlwaysShowGauge          = settings.GetValue(nameof(AlwaysShowGauge),          false),
				GaugeShowingThreshold    = settings.GetValue(nameof(GaugeShowingThreshold),    DefaultGaugeShowingThreshold),
				GaugeHidingThreshold     = settings.GetValue(nameof(GaugeHidingThreshold),     DefaultGaugeHidingThreshold),
				ShowTemperature          = settings.GetValue(nameof(ShowTemperature),          true),
				ShowTemperatureLimit     = settings.GetValue(nameof(ShowTemperatureLimit),     true),
				ShowTemperatureRate      = settings.GetValue(nameof(ShowTemperatureRate),      true),
				ShowCriticalPart         = settings.GetValue(nameof(ShowCriticalPart),         true),
				HighlightCriticalPart    = settings.GetValue(nameof(HighlightCriticalPart),    true),
				PartMenuTemperature      = settings.GetValue(nameof(PartMenuTemperature),      true),
				PartMenuTemperatureLimit = settings.GetValue(nameof(PartMenuTemperatureLimit), true),
				PartMenuTemperatureRate  = settings.GetValue(nameof(PartMenuTemperatureRate),  true),
				UseExclusionList         = settings.GetValue(nameof(UseExclusionList),         false),
				ExclusionList            = settings.GetValue(nameof(ExclusionList),            ""),
			};
		}
	}
}
