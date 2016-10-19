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

		const int FlashPeriod = 4;

		int _currentHighlightColor = 0;
		Part _previousHighlightedPart = null;

		/// <summary>Returns whether there is a highlighted part.</summary>
		public bool IsThereHighlightedPart { get; private set; }

		/// <summary>Highlights a given part.</summary>
		/// <param name="part">Part to higilight or <c>null</c>.</param>
		public void SetHighlightedPart(Part part)
		{
			IsThereHighlightedPart = part != null;
			if(part != _previousHighlightedPart)
			{
				ResetPartHighlight(_previousHighlightedPart);
				_previousHighlightedPart = part;
			}
			SetPartHighlight(part);
		}

		void ResetPartHighlight(Part part)
		{
			if(part != null)
			{
				part.SetHighlightDefault();
				part.SetHighlight(false, false);
			}
		}

		void SetPartHighlight(Part part)
		{
			if(part != null)
			{
				part.highlightType = Part.HighlightType.AlwaysOn;
				part.SetHighlightColor(HighlightColors[_currentHighlightColor / FlashPeriod]);
				part.SetHighlight(true, false);
			}
			_currentHighlightColor = (_currentHighlightColor + 1) % (HighlightColors.Length * FlashPeriod);
		}
	}
}
