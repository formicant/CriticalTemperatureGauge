using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Represents the temperature gauge window.
	/// </summary>
	public class GaugeWindow : Window
	{
		// Resources and layout
		static readonly Texture2D GaugeFrameTexture = GameDatabase.Instance.GetTexture(Static.TexturePath + "GaugeFrame", false);
		static readonly Texture2D GaugeScaleTexture = GameDatabase.Instance.GetTexture(Static.TexturePath + "GaugeScale", false);

		static readonly Color TextColor = Color.white;
		static readonly Color[] TextShadowColors =
		{
			new Color(0, 0, 0, 0.8F),
			new Color(0, 0, 0, 0.3F),
			new Color(0, 0, 0, 0.2F),
		};

		static readonly Rect GaugeFrameRectangle = new Rect(0, 0, 240, 24);
		static readonly Rect GaugeScaleRectangle = new Rect(8, 5, 232, 14);
		static readonly Rect InnerLabelRectangle = new Rect(0, 0, 240, 24);
		static readonly Rect OuterLabelRectangle = new Rect(0, 19, 240, 24);
		static readonly Vector2 WindowSize = new Vector2(240, 40);
		const int GaugeScaleNominalLength = 224;

		const float FontSize = 14;

		protected override GUISkin Skin => GUI.skin;

		float AltimeterScale => GameSettings.UI_SCALE * GameSettings.UI_SCALE_ALTIMETER;
		float Scale => AltimeterScale * Static.Settings.GaugeWindowScale;

		Vector2 DockPosition => new Vector2(Screen.width / 2, AltimeterScale * 83);

		protected override Vector2? ConstWindowPosition =>
			Static.Settings.LockGaugeWindow
				? (Static.Settings.DockGaugeWindow ? DockPosition : Static.Settings.GaugeWindowPosition) +
					Scale * WindowSize.x / 2 * Vector2.left
				: (Vector2?)null;

		protected override Vector2? ConstWindowSize => Scale * WindowSize;

		protected override Rect InitialWindowRectangle =>
			new Rect(
				DockPosition - new Vector2(Scale * 120, 0),
				ConstWindowSize.Value);

		/// <summary>Creates the temperature gauge window.</summary>
		public GaugeWindow()
			: base(Static.BaseWindowId + 0, hasClearBackground: true, clickThrough: true) { }

		/// <summary>Writes current window position into settings.</summary>
		protected override void OnWindowRectUpdated()
		{
			Static.Settings.GaugeWindowPosition = Static.Settings.DockGaugeWindow
				? DockPosition
				: WindowRectangle.position + Scale * WindowSize.x / 2 * Vector2.right;
		}

		/// <summary>Draws the window contents.</summary>
		/// <param name="windowId">Id of the window.</param>
		protected override void WindowGUI(int windowId)
		{
			var scale = Scale;

			if(Static.CriticalPartState != null)
			{
				float gaugeScaleValue = (float)Static.CriticalPartState.Index;
				
				GUILayout.BeginVertical();

				// Drawing gauge
				GUI.DrawTexture(GaugeFrameRectangle.Scale(scale), GaugeFrameTexture);
				GUI.DrawTextureWithTexCoords(
					new Rect(
						scale * GaugeScaleRectangle.x,
						scale * GaugeScaleRectangle.y,
						scale * GaugeScaleNominalLength * gaugeScaleValue,
						scale * GaugeScaleRectangle.height),
					GaugeScaleTexture,
					new Rect(0, 0, gaugeScaleValue * GaugeScaleNominalLength / GaugeScaleRectangle.width, 1));

				int fontSize = (int)Math.Round(scale * FontSize);
				var innerLabelRectangle = InnerLabelRectangle.Scale(scale);

				// Drawing temperature and temperature limit values
				if(Static.Settings.ShowTemperature)
					DrawContrastLabel(
						innerLabelRectangle,
						TextAnchor.MiddleCenter,
						fontSize,
						Format.Temperature(
							Static.CriticalPartState.CriticalTemperature,
							Static.Settings.ShowTemperatureLimit.Then(Static.CriticalPartState.CriticalTemperatureLimit)));

				// Drawing temperature rate value
				if(Static.Settings.ShowTemperatureRate)
					DrawContrastLabel(
						innerLabelRectangle,
						TextAnchor.MiddleLeft,
						fontSize,
						Format.TemperatureRate(Static.CriticalPartState.CriticalTemperatureRate));

				// Drawing critical part name
				if(Static.Settings.ShowCriticalPart)
					DrawContrastLabel(
						OuterLabelRectangle.Scale(scale),
						TextAnchor.MiddleLeft,
						fontSize,
						Format.PartName(Static.CriticalPartState.Title, Static.CriticalPartState.IsSkinCritical));

				GUILayout.EndVertical();

				if(!Static.Settings.LockGaugeWindow)
					GUI.DragWindow();
			}
		}

		/// <summary>Draws white text with a black shadow.</summary>
		/// <param name="rectangle">Label rectangle</param>
		/// <param name="alignment">Text alignment.</param>
		/// <param name="fontSize">Font size.</param>
		/// <param name="text">Text to draw.</param>
		void DrawContrastLabel(Rect rectangle, TextAnchor alignment, int fontSize, string text)
		{
			var style = new GUIStyle(GUI.skin.label)
			{
				alignment = alignment,
				fontSize = fontSize,
				wordWrap = false,
			};

			// Drawing text shadow
			style.normal.textColor = TextShadowColors[0];
			rectangle.x--;
			GUI.Label(rectangle, text, style);
			rectangle.x++; rectangle.y--;
			GUI.Label(rectangle, text, style);
			rectangle.x++; rectangle.y++;
			GUI.Label(rectangle, text, style);
			rectangle.x--; rectangle.y++;
			GUI.Label(rectangle, text, style);
			style.normal.textColor = TextShadowColors[1];
			rectangle.x--;
			GUI.Label(rectangle, text, style);
			rectangle.y -= 2;
			GUI.Label(rectangle, text, style);
			rectangle.x += 2;
			GUI.Label(rectangle, text, style);
			rectangle.y += 2;
			GUI.Label(rectangle, text, style);
			style.normal.textColor = TextShadowColors[2];
			rectangle.y--; rectangle.x++;
			GUI.Label(rectangle, text, style);
			rectangle.x -= 4;
			GUI.Label(rectangle, text, style);
			rectangle.x += 2;

			// Drawing text
			style.normal.textColor = TextColor;
			GUI.Label(rectangle, text, style);
		}
	}
}
