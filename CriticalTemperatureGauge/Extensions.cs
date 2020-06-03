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

		public static IEnumerable<string> GetCommaSeparatedItems(this string list) =>
			list
				.Split(',')
				.Select(item => item.Trim().ToLowerInvariant())
				.Where(item => !string.IsNullOrEmpty(item));

		public static IEnumerable<string> AddModulePrefixes(this IEnumerable<string> moduleNames)
		{
			foreach(var moduleName in moduleNames)
			{
				yield return moduleName;
				if(!moduleName.StartsWith(ModulePrefix, StringComparison.OrdinalIgnoreCase))
					yield return ModulePrefix + moduleName;
			}
		}

		const string ModulePrefix = "Module";
	}
}
