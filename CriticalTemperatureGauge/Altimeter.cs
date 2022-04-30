using System;
using System.Collections.Generic;
using System.Linq;
using KSP.UI.Screens.Flight;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	public class Altimeter
	{
		public Altimeter()
		{
			var altitudeTumbler = GameObject.FindObjectOfType<KSP.UI.Screens.Flight.AltitudeTumbler>();
			_altimeterPanel = altitudeTumbler.gameObject.transform.parent.parent.gameObject;
		}

		public float Scale => _altimeterPanel.transform.lossyScale.y;

		public Vector2 DockPosition
		{
			get
			{
				var position = _altimeterPanel.transform.position;
				var x = Screen.width / 2f + position.x;
				var y = Screen.height / 2f - position.y + (Height - Margin) * Scale;
				return new Vector2(x, y);
			}
		}

		readonly GameObject _altimeterPanel;

		const float Height = 82.5f;
		const float Margin = 8.5f;
	}
}