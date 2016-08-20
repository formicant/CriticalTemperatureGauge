using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		public Vector2 ConfigWindowPosition { get; set; }
		public Vector2 GaugeWindowPosition { get; set; }
		public bool LockGaugeWindow { get; set; }

		// Additional information settings
		public bool ShowTemperature { get; set; }
		public bool ShowTemperatureLimit { get; set; }
		public bool ShowTemperatureRate { get; set; }
		public bool ShowCriticalPart { get; set; }

		// (Part highlighting does not work for some reason)
		//public bool HighlightCriticalPart { get; set; }

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
			var config = PluginConfiguration.CreateForType<TAddon>();

			config.SetValue(nameof(ConfigWindowPosition),  ConfigWindowPosition);
			config.SetValue(nameof(GaugeWindowPosition),   GaugeWindowPosition);
			config.SetValue(nameof(LockGaugeWindow),       LockGaugeWindow);
			config.SetValue(nameof(ForceShowGauge),        ForceShowGauge);
			config.SetValue(nameof(GaugeShowingThreshold), GaugeShowingThreshold);
			config.SetValue(nameof(GaugeHidingThreshold),  GaugeHidingThreshold);
			config.SetValue(nameof(ShowTemperature),       ShowTemperature);
			config.SetValue(nameof(ShowTemperatureLimit),  ShowTemperatureLimit);
			config.SetValue(nameof(ShowTemperatureRate),   ShowTemperatureRate);
			config.SetValue(nameof(ShowCriticalPart),      ShowCriticalPart);
			//config.SetValue(nameof(HighlightCriticalPart), HighlightCriticalPart);
			config.SetValue(nameof(UseExclusionList),      UseExclusionList);
			config.SetValue(nameof(ExclusionList),         ExclusionList);

			config.save();
		}

		/// <summary>Loads settings from an XML file inside PluginData directory.</summary>
		/// <returns>Loaded settings.</returns>
		public static Settings<TAddon> Load()
		{
			var config = PluginConfiguration.CreateForType<TAddon>();
			config.load();

			return new Settings<TAddon>
			{
				ConfigWindowPosition  = config.GetValue(nameof(ConfigWindowPosition),  Vector2.zero),
				GaugeWindowPosition   = config.GetValue(nameof(GaugeWindowPosition),   Vector2.zero),
				LockGaugeWindow       = config.GetValue(nameof(LockGaugeWindow),       true),
				ForceShowGauge        = config.GetValue(nameof(ForceShowGauge),        false),
				GaugeShowingThreshold = config.GetValue(nameof(GaugeShowingThreshold), DefaultGaugeShowingThreshold),
				GaugeHidingThreshold  = config.GetValue(nameof(GaugeHidingThreshold),  DefaultGaugeHidingThreshold),
				ShowTemperature       = config.GetValue(nameof(ShowTemperature),       true),
				ShowTemperatureLimit  = config.GetValue(nameof(ShowTemperatureLimit),  true),
				ShowTemperatureRate   = config.GetValue(nameof(ShowTemperatureRate),   true),
				ShowCriticalPart      = config.GetValue(nameof(ShowCriticalPart),      true),
				//HighlightCriticalPart = config.GetValue(nameof(HighlightCriticalPart), false),
				UseExclusionList      = config.GetValue(nameof(UseExclusionList),      false),
				ExclusionList         = config.GetValue(nameof(ExclusionList),         ""),
			};
		}
	}
}
