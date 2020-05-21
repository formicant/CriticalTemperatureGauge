using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Part highlighting.
	/// </summary>
	public class Highlighter
	{
		// Highlight color sequence
		static readonly Color[] HighlightColors =
		{
			Color.yellow,
			Color.red,
			Color.blue,
		};

		const double FlashPeriod = 0.166;

		double _lastColorChangeTime;
		int _currentHighlightColor;
		Part _previousHighlightedPart;

		/// <summary>Returns whether there is a highlighted part.</summary>
		public bool IsThereHighlightedPart { get; private set; }

		/// <summary>Highlights a given part.</summary>
		/// <param name="part">Part to highlight or <c>null</c>.</param>
		public void SetHighlightedPart(Part part)
		{
			IsThereHighlightedPart = part is object;
			if(part != _previousHighlightedPart)
			{
				ResetPartHighlight(_previousHighlightedPart);
				_previousHighlightedPart = part;
			}
			SetPartHighlight(part);
		}

		void ResetPartHighlight(Part part)
		{
			if(part is object)
			{
				part.SetHighlightDefault();
				part.SetHighlight(active: false, recursive: false);
			}
		}

		void SetPartHighlight(Part part)
		{
			if(part is object)
			{
				part.highlightType = Part.HighlightType.AlwaysOn;
				part.SetHighlightColor(HighlightColors[_currentHighlightColor]);
				part.SetHighlight(active: true, recursive: false);
				
				var time = Planetarium.GetUniversalTime();
				if(time - _lastColorChangeTime > FlashPeriod * TimeWarp.CurrentRate)
				{
					_currentHighlightColor = (_currentHighlightColor + 1) % HighlightColors.Length;
					_lastColorChangeTime = time;
				}
			}
		}
	}
}
