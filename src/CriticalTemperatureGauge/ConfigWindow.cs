using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Represents the configuration window.
	/// </summary>
	public class ConfigWindow : Window
	{
		/// <summary>Slider resolution..</summary>
		const int ThresholdSteps = 20;

		// Layout parameters
		const int SeparatorHeight = 8;
		const int IndentWidth = 20;

		protected override Rect InitialWindowRectangle =>
			new Rect(
				Static.Settings.ConfigWindowPosition != Vector2.zero
					? Static.Settings.ConfigWindowPosition
					: new Vector2(Screen.width - 285, (Screen.height - 410) / 2),
				Vector2.zero);

		/// <summary>Creates the configuration window.</summary>
		public ConfigWindow()
			: base(Static.Settings.BaseWindowId + 1, "Critical Temperature Gauge") { }

		/// <summary>Writes current window position into settings.</summary>
		protected override void OnWindowRectUpdated()
		{
			Static.Settings.ConfigWindowPosition = WindowRectangle.position;
		}

		/// <summary>Draws the window contents.</summary>
		/// <param name="windowId">Id of the window.</param>
		protected override void WindowGUI(int windowId)
		{
			// Defining styles
			var windowStyle = new GUIStyle(GUI.skin.window) { padding = new RectOffset(8, 8, 8, 8) };
			var textEditStyle = new GUIStyle(GUI.skin.textField);
			var labelStyle = new GUIStyle(GUI.skin.label)
			{
				normal = new GUIStyleState { textColor = Color.white },
				focused = new GUIStyleState { textColor = Color.white },
				alignment = TextAnchor.MiddleLeft,
			};
			var sliderStyle = new GUIStyle(GUI.skin.horizontalSlider) { fixedWidth = 222 };
			var sliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);

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
			Static.Settings.ShowCriticalPart = GUILayout.Toggle(Static.Settings.ShowCriticalPart, "Show critical part");
			// (Part highlighting does not work for some reason)
			//Static.Settings.HighlightCriticalPart = GUILayout.Toggle(Static.Settings.HighlightCriticalPart, "Highlight critical part");

			// Drawing exclusion list settings controls
			GUILayout.Space(SeparatorHeight);
			Static.Settings.UseExclusionList = GUILayout.Toggle(Static.Settings.UseExclusionList, "Ignore parts with following\nmodules (comma-separated):");
			GUILayout.Space(SeparatorHeight * 2);
			Static.Settings.ExclusionList = GUILayout.TextField(Static.Settings.ExclusionList, 512, textEditStyle);

			// Drawing interface settings controls
			GUILayout.Space(SeparatorHeight);
			Static.Settings.LockGaugeWindow = GUILayout.Toggle(Static.Settings.LockGaugeWindow, "Lock gauge position");

			GUI.DragWindow();
		}
	}
}
