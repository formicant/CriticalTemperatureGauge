using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		static readonly Rect GaugeFrameRectangle = new Rect(0, 0, GaugeFrameTexture.width, GaugeFrameTexture.height);
		static readonly Rect GaugeScaleRectangle = new Rect(6, 4, GaugeScaleTexture.width, 12);
		static readonly Rect InnerLabelRectangle = new Rect(0, 2, GaugeFrameTexture.width, GaugeFrameTexture.height);
		static readonly Rect OuterLabelRectangle = new Rect(0, GaugeFrameTexture.height - 2, GaugeFrameTexture.width, GaugeFrameTexture.height);
		const int FontSize = 12;

		protected override Rect InitialWindowRectangle =>
			new Rect(
				Static.Settings.GaugeWindowPosition != Vector2.zero
					? Static.Settings.GaugeWindowPosition
					: new Vector2((Screen.width - GaugeFrameTexture.width) / 2, 83),
				new Vector2(GaugeFrameTexture.width, GaugeFrameTexture.height));

		/// <summary>Creates the temperature gauge window.</summary>
		public GaugeWindow()
			: base(Static.Settings.BaseWindowId + 0, hasClearBackground: true) { }

		/// <summary>Writes current window position into settings.</summary>
		protected override void OnWindowRectUpdated()
		{
			Static.Settings.GaugeWindowPosition = WindowRectangle.position;
		}

		/// <summary>Draws the window contents.</summary>
		/// <param name="windowId">Id of the window.</param>
		protected override void WindowGUI(int windowId)
		{
			if(Static.CriticalPartState != null)
			{
				float gaugeScaleValue = (float)Static.CriticalPartState.Index;

				GUILayout.BeginVertical();

				// Drawing gauge
				GUI.DrawTexture(GaugeFrameRectangle, GaugeFrameTexture);
				GUI.DrawTextureWithTexCoords(
					new Rect(GaugeScaleRectangle.x, GaugeScaleRectangle.y, GaugeScaleRectangle.width * gaugeScaleValue, GaugeScaleRectangle.height),
					GaugeScaleTexture, new Rect(0, 0, gaugeScaleValue, 1));

				// Drawing temperature and temperature limit values
				if(Static.Settings.ShowTemperature)
					DrawContrastLabel(InnerLabelRectangle, TextAnchor.MiddleCenter, FontSize,
						$@"{Static.CriticalPartState.CriticalTemperature:F0}{
							(Static.Settings.ShowTemperatureLimit ? $" / {Static.CriticalPartState.CriticalTemperatureLimit:F0}" : "")} K");

				// Drawing temperature rate value
				if(Static.Settings.ShowTemperatureRate)
					DrawContrastLabel(InnerLabelRectangle, TextAnchor.MiddleLeft, FontSize,
						$@"  {(Static.CriticalPartState.CriticalTemperatureRate < 0 ? "−" : "+")}{
							Math.Abs(Static.CriticalPartState.CriticalTemperatureRate):F0} K/s");

				// Drawing critical part name
				if(Static.Settings.ShowCriticalPart)
					DrawContrastLabel(OuterLabelRectangle, TextAnchor.MiddleLeft, FontSize,
						$@" {Static.CriticalPartState.Name} ({(Static.CriticalPartState.IsSkinCritical ? "skin" : "core")})");

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
