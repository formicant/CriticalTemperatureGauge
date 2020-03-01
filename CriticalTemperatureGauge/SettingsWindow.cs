using System;
using System.Collections.Generic;
using System.Linq;
using KSP.Localization;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Represents the settings window.
	/// </summary>
	public class SettingsWindow : Window
	{
		// Slider resolution
		const int ThresholdSteps = 20;
		const int ScaleSteps = 20;

		// Layout parameters
		const int SeparatorHeight = 8;
		const int IndentWidth = 20;

		// GUI element styles
		GUIStyle _closeButtonStyle;
		GUIStyle CloseButtonStyle => _closeButtonStyle ??=
			new GUIStyle(GUI.skin.button)
			{
				alignment = TextAnchor.MiddleCenter,
				padding = new RectOffset(0, 0, 0, 0),
			};

		static GUIStyle _textEditStyle;
		static GUIStyle TextEditStyle => _textEditStyle ??=
			new GUIStyle(GUI.skin.textField);

		static GUIStyle _labelStyle;
		static GUIStyle LabelStyle => _labelStyle ??=
			new GUIStyle(GUI.skin.label)
			{
				normal = new GUIStyleState { textColor = Color.white },
				focused = new GUIStyleState { textColor = Color.white },
				alignment = TextAnchor.MiddleLeft,
			};

		static GUIStyle _sliderStyle;
		static GUIStyle SliderStyle => _sliderStyle ??=
			new GUIStyle(GUI.skin.horizontalSlider) { fixedWidth = 198 };

		static GUIStyle _sliderThumbStyle;
		static GUIStyle SliderThumbStyle => _sliderThumbStyle ??=
			new GUIStyle(GUI.skin.horizontalSliderThumb);

		static GUIStyle _toggleStyle;
		static GUIStyle ToggleStyle => _toggleStyle ??=
			new GUIStyle(GUI.skin.toggle) { contentOffset = new Vector2(4, 0) };


		protected override GUISkin Skin => GUI.skin;

		protected override Rect InitialWindowRectangle =>
			new Rect(
				Static.Settings.SettingsWindowPosition != Vector2.zero
					? Static.Settings.SettingsWindowPosition
					: new Vector2(0, 68),
				Vector2.zero);

		/// <summary>Creates the settings window.</summary>
		public SettingsWindow()
			: base(Static.BaseWindowId + 1, Static.PluginTitle + " "/* U+2003 Em Space */) { }

		/// <summary>Writes current window position into settings.</summary>
		protected override void OnWindowRectUpdated()
		{
			Static.Settings.SettingsWindowPosition = WindowRectangle.position;
		}


		/// <summary>Draws the window contents.</summary>
		/// <param name="windowId">Id of the window.</param>
		protected override void WindowGUI(int windowId)
		{
			// Close button
			if(GUI.Button(new Rect(WindowRectangle.width - 17, 2, 15, 15), "×", CloseButtonStyle))
				Hide();

			// Gauge visibility settings controls
			GUILayout.Space(SeparatorHeight);
			GUILayout.Label(
				Localizer.Format("#LOC_CriticalTemperatureGauge_GaugeShowingThreshold") + $" {Static.Settings.GaugeShowingThreshold:G2}",
				LabelStyle);
			Static.Settings.GaugeShowingThreshold =
				Math.Round(GUILayout.HorizontalSlider((float)Static.Settings.GaugeShowingThreshold, 1F / ThresholdSteps, 1F - 1F / ThresholdSteps, SliderStyle, SliderThumbStyle) * ThresholdSteps) / ThresholdSteps;
			GUILayout.Label(
				Localizer.Format("#LOC_CriticalTemperatureGauge_GaugeHidingThreshold") + $" {Static.Settings.GaugeHidingThreshold:G2}",
				LabelStyle);
			Static.Settings.GaugeHidingThreshold = Math.Min(Static.Settings.GaugeShowingThreshold,
				Math.Round(GUILayout.HorizontalSlider((float)Static.Settings.GaugeHidingThreshold, 1F / ThresholdSteps, 1F - 1F / ThresholdSteps, SliderStyle, SliderThumbStyle) * ThresholdSteps) / ThresholdSteps);
			Static.Settings.AlwaysShowGauge = GUILayout.Toggle(
				Static.Settings.AlwaysShowGauge,
				Localizer.Format("#LOC_CriticalTemperatureGauge_AlwaysShowGauge"),
				ToggleStyle);

			// Additional information settings controls
			Static.Settings.ShowTemperature = GUILayout.Toggle(
				Static.Settings.ShowTemperature,
				Localizer.Format("#LOC_CriticalTemperatureGauge_ShowTemperature"),
				ToggleStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Space(IndentWidth);
			Static.Settings.ShowTemperatureLimit = GUILayout.Toggle(
				Static.Settings.ShowTemperatureLimit,
				Localizer.Format("#LOC_CriticalTemperatureGauge_ShowTemperatureLimit"),
				ToggleStyle);
			GUILayout.EndHorizontal();
			Static.Settings.ShowTemperatureRate = GUILayout.Toggle(
				Static.Settings.ShowTemperatureRate,
				Localizer.Format("#LOC_CriticalTemperatureGauge_ShowTemperatureRate"),
				ToggleStyle);
			Static.Settings.UseBoldFont = GUILayout.Toggle(
				Static.Settings.UseBoldFont,
				Localizer.Format("#LOC_CriticalTemperatureGauge_UseBoldFont"),
				ToggleStyle);
			Static.Settings.ShowCriticalPart = GUILayout.Toggle(
				Static.Settings.ShowCriticalPart,
				Localizer.Format("#LOC_CriticalTemperatureGauge_ShowCriticalPartName"),
				ToggleStyle);
			Static.Settings.HighlightCriticalPart = GUILayout.Toggle(
				Static.Settings.HighlightCriticalPart,
				Localizer.Format("#LOC_CriticalTemperatureGauge_HighlightCriticalPart"),
				ToggleStyle);

			// Exclusion list settings controls
			GUILayout.Space(SeparatorHeight);
			Static.Settings.UseExclusionList = GUILayout.Toggle(
				Static.Settings.UseExclusionList,
				Localizer.Format("#LOC_CriticalTemperatureGauge_IgnorePartModules"),
				ToggleStyle);
			Static.Settings.ExclusionList = GUILayout.TextField(Static.Settings.ExclusionList, 512, TextEditStyle);

			// Part menu settings controls
			GUILayout.Space(SeparatorHeight);
			Static.Settings.PartMenuTemperature = GUILayout.Toggle(
				Static.Settings.PartMenuTemperature,
				Localizer.Format("#LOC_CriticalTemperatureGauge_ShowTemperatureInPartMenu"),
				ToggleStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Space(IndentWidth);
			GUILayout.BeginVertical();
			Static.Settings.PartMenuTemperatureLimit = GUILayout.Toggle(
				Static.Settings.PartMenuTemperatureLimit,
				Localizer.Format("#LOC_CriticalTemperatureGauge_ShowTemperatureLimit"),
				ToggleStyle);
			Static.Settings.PartMenuTemperatureRate = GUILayout.Toggle(
				Static.Settings.PartMenuTemperatureRate,
				Localizer.Format("#LOC_CriticalTemperatureGauge_ShowTemperatureRate"),
				ToggleStyle);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			// Interface settings controls
			GUILayout.Space(SeparatorHeight);
			GUILayout.Label(
				Localizer.Format("#LOC_CriticalTemperatureGauge_GaugeScale") + $" {Static.Settings.GaugeWindowScale:F2}",
				LabelStyle);
			Static.Settings.GaugeWindowScale =
				(float)Math.Round(GUILayout.HorizontalSlider(Static.Settings.GaugeWindowScale, 0.5F, 2F, SliderStyle, SliderThumbStyle) * ScaleSteps) / ScaleSteps;
			Static.Settings.DockGaugeWindow = GUILayout.Toggle(
				Static.Settings.DockGaugeWindow,
				Localizer.Format("#LOC_CriticalTemperatureGauge_DockGaugeToAltimeter"),
				ToggleStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Space(IndentWidth);
			Static.Settings.LockGaugeWindow = Static.Settings.DockGaugeWindow || GUILayout.Toggle(
				Static.Settings.LockGaugeWindow,
				Localizer.Format("#LOC_CriticalTemperatureGauge_LockGaugePosition"),
				ToggleStyle);
			GUILayout.EndHorizontal();

			GUI.DragWindow();
		}
	}
}
