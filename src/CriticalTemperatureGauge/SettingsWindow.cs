using System;
using System.Collections.Generic;
using System.Linq;
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

		protected override Rect InitialWindowRectangle =>
			new Rect(
				Static.Settings.SettingsWindowPosition != Vector2.zero
					? Static.Settings.SettingsWindowPosition
					: new Vector2(Screen.width - 285, (Screen.height - 410) / 2),
				Vector2.zero);

		/// <summary>Creates the settings window.</summary>
		public SettingsWindow()
			: base(Static.Settings.BaseWindowId + 1, Static.PluginTitle + " ") { }

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
				padding = new RectOffset(0, 0, 3, 0),
			};
			var textEditStyle = new GUIStyle(GUI.skin.textField);
			var labelStyle = new GUIStyle(GUI.skin.label)
			{
				normal = new GUIStyleState { textColor = Color.white },
				focused = new GUIStyleState { textColor = Color.white },
				alignment = TextAnchor.MiddleLeft,
			};
			var sliderStyle = new GUIStyle(GUI.skin.horizontalSlider) { fixedWidth = 222 };
			var sliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);

			if(GUI.Button(new Rect(WindowRectangle.width - 19, 3, 17, 17), "×", closeButtonStyle) && Static.AppLauncher != null)
				Static.AppLauncher.ButtonState = false;
			
			// Drawing layout
			GUILayout.Space(SeparatorHeight);

			// Drawing gauge visibility settings controls
			GUILayout.Label($"Gauge showing threshold: {Static.Settings.GaugeShowingThreshold:G2}", labelStyle);
			Static.Settings.GaugeShowingThreshold =
				Math.Round(GUILayout.HorizontalSlider((float)Static.Settings.GaugeShowingThreshold, 1F / ThresholdSteps, 1F - 1F / ThresholdSteps, sliderStyle, sliderThumbStyle) * ThresholdSteps) / ThresholdSteps;
			GUILayout.Label($"Gauge hiding threshold: {Static.Settings.GaugeHidingThreshold:G2}", labelStyle);
			Static.Settings.GaugeHidingThreshold = Math.Min(Static.Settings.GaugeShowingThreshold,
				Math.Round(GUILayout.HorizontalSlider((float)Static.Settings.GaugeHidingThreshold, 1F / ThresholdSteps, 1F - 1F / ThresholdSteps, sliderStyle, sliderThumbStyle) * ThresholdSteps) / ThresholdSteps);
			Static.Settings.ForceShowGauge = GUILayout.Toggle(Static.Settings.ForceShowGauge, "Force show gauge");

			// Drawing additional information settings controls
			Static.Settings.ShowTemperature = GUILayout.Toggle(Static.Settings.ShowTemperature, "Show temperature");
			GUILayout.BeginHorizontal();
			GUILayout.Space(IndentWidth);
			Static.Settings.ShowTemperatureLimit = GUILayout.Toggle(Static.Settings.ShowTemperatureLimit, "Show temperature limit");
			GUILayout.EndHorizontal();
			Static.Settings.ShowTemperatureRate = GUILayout.Toggle(Static.Settings.ShowTemperatureRate, "Show temperature rate");
			Static.Settings.ShowCriticalPart = GUILayout.Toggle(Static.Settings.ShowCriticalPart, "Show critical part name");
			Static.Settings.HighlightCriticalPart = GUILayout.Toggle(Static.Settings.HighlightCriticalPart, "Highlight critical part");

			// Drawing exclusion list settings controls
			GUILayout.Space(SeparatorHeight);
			Static.Settings.UseExclusionList = GUILayout.Toggle(Static.Settings.UseExclusionList, "Ignore parts with following\nmodules (comma-separated):");
			GUILayout.Space(SeparatorHeight * 2);
			Static.Settings.ExclusionList = GUILayout.TextField(Static.Settings.ExclusionList, 512, textEditStyle);

			// Drawing interface settings controls
			GUILayout.Space(SeparatorHeight);
			Static.Settings.LockGaugeWindow = GUILayout.Toggle(Static.Settings.LockGaugeWindow, "Lock gauge position");
			Static.Settings.ShowAppLauncherButton = GUILayout.Toggle(Static.Settings.ShowAppLauncherButton, "Show AppLauncher button");
			Static.AppLauncher?.Update();

			GUI.DragWindow();
		}
	}
}
