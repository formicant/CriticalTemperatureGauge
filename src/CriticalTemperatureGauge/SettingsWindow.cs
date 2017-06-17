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
		/// <summary>Slider resolution.</summary>
		const int ThresholdSteps = 20;

		// Layout parameters
		const int SeparatorHeight = 8;
		const int IndentWidth = 20;

		protected override GUISkin Skin => GUI.skin;

		protected override Rect InitialWindowRectangle =>
			new Rect(
				Static.Settings.SettingsWindowPosition != Vector2.zero
					? Static.Settings.SettingsWindowPosition
					: new Vector2(0, 68),
				Vector2.zero);

		/// <summary>Creates the settings window.</summary>
		public SettingsWindow()
			: base(Static.BaseWindowId + 1, Static.PluginTitle + " ") { }

		/// <summary>Writes current window position into settings.</summary>
		protected override void OnWindowRectUpdated()
		{
			Static.Settings.SettingsWindowPosition = WindowRectangle.position;
		}

		/// <summary>Draws the window contents.</summary>
		/// <param name="windowId">Id of the window.</param>
		protected override void WindowGUI(int windowId)
		{
			// Defining styles
			var closeButtonStyle = new GUIStyle(GUI.skin.button)
			{
				alignment = TextAnchor.MiddleCenter,
				padding = new RectOffset(0, 0, 0, 0),
			};
			var textEditStyle = new GUIStyle(GUI.skin.textField);
			var labelStyle = new GUIStyle(GUI.skin.label)
			{
				normal = new GUIStyleState { textColor = Color.white },
				focused = new GUIStyleState { textColor = Color.white },
				alignment = TextAnchor.MiddleLeft,
			};
			var sliderStyle = new GUIStyle(GUI.skin.horizontalSlider) { fixedWidth = 198 };
			var sliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
			var toggleStyle = new GUIStyle(GUI.skin.toggle) { contentOffset = new Vector2(4, 0) };

			if(GUI.Button(new Rect(WindowRectangle.width - 17, 2, 15, 15), "×", closeButtonStyle) && Static.AppLauncher != null)
				Static.AppLauncher.ButtonState = false;
			
			// Drawing layout

			// Drawing gauge visibility settings controls
			GUILayout.Space(SeparatorHeight);
			GUILayout.Label(
				Localizer.Format("#ModCriticalTemperatureGauge_GaugeShowingThreshold") + $" {Static.Settings.GaugeShowingThreshold:G2}",
				labelStyle);
			Static.Settings.GaugeShowingThreshold =
				Math.Round(GUILayout.HorizontalSlider((float)Static.Settings.GaugeShowingThreshold, 1F / ThresholdSteps, 1F - 1F / ThresholdSteps, sliderStyle, sliderThumbStyle) * ThresholdSteps) / ThresholdSteps;
			GUILayout.Label(
				Localizer.Format("#ModCriticalTemperatureGauge_GaugeHidingThreshold") + $" {Static.Settings.GaugeHidingThreshold:G2}",
				labelStyle);
			Static.Settings.GaugeHidingThreshold = Math.Min(Static.Settings.GaugeShowingThreshold,
				Math.Round(GUILayout.HorizontalSlider((float)Static.Settings.GaugeHidingThreshold, 1F / ThresholdSteps, 1F - 1F / ThresholdSteps, sliderStyle, sliderThumbStyle) * ThresholdSteps) / ThresholdSteps);
			Static.Settings.ForceShowGauge = GUILayout.Toggle(
				Static.Settings.ForceShowGauge,
				Localizer.Format("#ModCriticalTemperatureGauge_ForceShowGauge"),
				toggleStyle);

			// Drawing additional information settings controls
			Static.Settings.ShowTemperature = GUILayout.Toggle(
				Static.Settings.ShowTemperature,
				Localizer.Format("#ModCriticalTemperatureGauge_ShowTemperature"),
				toggleStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Space(IndentWidth);
			Static.Settings.ShowTemperatureLimit = GUILayout.Toggle(
				Static.Settings.ShowTemperatureLimit,
				Localizer.Format("#ModCriticalTemperatureGauge_ShowTemperatureLimit"),
				toggleStyle);
			GUILayout.EndHorizontal();
			Static.Settings.ShowTemperatureRate = GUILayout.Toggle(
				Static.Settings.ShowTemperatureRate,
				Localizer.Format("#ModCriticalTemperatureGauge_ShowTemperatureRate"),
				toggleStyle);
			Static.Settings.ShowCriticalPart = GUILayout.Toggle(
				Static.Settings.ShowCriticalPart,
				Localizer.Format("#ModCriticalTemperatureGauge_ShowCriticalPartName"),
				toggleStyle);
			Static.Settings.HighlightCriticalPart = GUILayout.Toggle(
				Static.Settings.HighlightCriticalPart,
				Localizer.Format("#ModCriticalTemperatureGauge_HighlightCriticalPart"),
				toggleStyle);

			// Drawing exclusion list settings controls
			GUILayout.Space(SeparatorHeight);
			Static.Settings.UseExclusionList = GUILayout.Toggle(
				Static.Settings.UseExclusionList,
				Localizer.Format("#ModCriticalTemperatureGauge_IgnorePartModules"),
				toggleStyle);
			Static.Settings.ExclusionList = GUILayout.TextField(Static.Settings.ExclusionList, 512, textEditStyle);

			// Drawing part menu settings controls
			GUILayout.Space(SeparatorHeight);
			Static.Settings.PartMenuTemperature = GUILayout.Toggle(
				Static.Settings.PartMenuTemperature,
				Localizer.Format("#ModCriticalTemperatureGauge_ShowTemperatureInPartMenu"),
				toggleStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Space(IndentWidth);
			GUILayout.BeginVertical();
			Static.Settings.PartMenuTemperatureLimit = GUILayout.Toggle(
				Static.Settings.PartMenuTemperatureLimit,
				Localizer.Format("#ModCriticalTemperatureGauge_ShowTemperatureLimit"),
				toggleStyle);
			Static.Settings.PartMenuTemperatureRate = GUILayout.Toggle(
				Static.Settings.PartMenuTemperatureRate,
				Localizer.Format("#ModCriticalTemperatureGauge_ShowTemperatureRate"),
				toggleStyle);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			// Drawing interface settings controls
			GUILayout.Space(SeparatorHeight);
			Static.Settings.LockGaugeWindow = GUILayout.Toggle(
				Static.Settings.LockGaugeWindow,
				Localizer.Format("#ModCriticalTemperatureGauge_LockGaugePosition"),
				toggleStyle);
			Static.Settings.ShowAppLauncherButton = GUILayout.Toggle(
				Static.Settings.ShowAppLauncherButton,
				Localizer.Format("#ModCriticalTemperatureGauge_ShowAppLauncherButton"),
				toggleStyle);
			Static.AppLauncher?.Update();

			GUI.DragWindow();
		}
	}
}
