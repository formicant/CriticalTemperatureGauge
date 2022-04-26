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
			_screenCenter = Screen.width / 2f;
		}

		public float Scale => _altimeterPanel.transform.lossyScale.y;

		public Vector2 DockPosition
		{
			get
			{
				var position = _altimeterPanel.transform.position;
				var x = ShiftX + position.x;
				var y = ShiftY - position.y + (Height - Margin) * Scale;
				return new Vector2(x, y);
			}
		}

		readonly GameObject _altimeterPanel;
		readonly float _screenCenter;

		const float ShiftX = 840;
		const float ShiftY = 525;
		const float Height = 82.5f;
		const float Margin = 8.5f;
	}
}