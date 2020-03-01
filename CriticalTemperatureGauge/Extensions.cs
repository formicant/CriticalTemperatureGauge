using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	static class Extensions
	{
		public static T? Then<T>(this bool condition, T thenValue) where T : struct =>
			condition ? thenValue : (T?)null;

		public static Rect Scale(this Rect rectangle, float scale) =>
			new Rect(scale * rectangle.position, scale * rectangle.size);
	}
}
