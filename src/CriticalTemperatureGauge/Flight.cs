using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Main add-on class.
	/// </summary>
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class Flight : MonoBehaviour
	{
		readonly Window _gaugeWindow = new GaugeWindow();
		readonly Highlighter _highlighter = new Highlighter();
		readonly BlizzysToolbar _toolbar = new BlizzysToolbar();

		// KSP events:

		public void Start()
		{
			Debug.Log($"{nameof(CriticalTemperatureGauge)}: Entering scene {HighLogic.LoadedScene}.");
			_toolbar.Start();
		}

		public void Awake()
		{
			GameEvents.onShowUI.Add(OnShowUI);
			GameEvents.onHideUI.Add(OnHideUI);
		}

		public void OnDestroy()
		{
			GameEvents.onShowUI.Remove(OnShowUI);
			GameEvents.onHideUI.Remove(OnHideUI);
			_gaugeWindow?.Hide();
			if(Static.AppLauncher != null)
				Static.AppLauncher.ButtonState = false;
			Static.CriticalPartState = null;
			_toolbar.Destroy();
			Static.Settings.Save();
			Debug.Log($"{nameof(CriticalTemperatureGauge)}: Exiting scene {HighLogic.LoadedScene}.");
		}

		void OnShowUI()
		{
			if(_gaugeWindow != null)
				_gaugeWindow.CanShow = true;
		}

		void OnHideUI()
		{
			if(_gaugeWindow != null)
				_gaugeWindow.CanShow = false;
		}

		public void OnGUI()
		{
			_gaugeWindow?.DrawGUI();
		}

		public void Update()
		{
			var vessel = FlightGlobals.ActiveVessel;
			if(_gaugeWindow != null && vessel != null)
			{
				// Updating critical part state
				var criticalPartState = GetCriticalPartState(vessel);
				criticalPartState?.UpdateTemperatureRates(Static.CriticalPartState);
				Static.CriticalPartState = criticalPartState;

				// Determining if the gauge should be shown or hidden
				if(!_gaugeWindow.IsLogicallyVisible &&
					criticalPartState != null &&
					(criticalPartState.Index > Static.Settings.GaugeShowingThreshold ||
					Static.Settings.ForceShowGauge))
				{
					_gaugeWindow.Show();
				}
				else if(_gaugeWindow.IsLogicallyVisible &&
					(criticalPartState == null ||
					criticalPartState.Index < Static.Settings.GaugeHidingThreshold &&
					!Static.Settings.ForceShowGauge))
				{
					_gaugeWindow.Hide();
				}

				// Highlighting the critical part if needed
				_highlighter.SetHighlightedPart(
					Static.Settings.HighlightCriticalPart &&
					criticalPartState != null &&
					(_highlighter.IsThereHighlightedPart &&
					criticalPartState.Index > Static.Settings.GaugeHidingThreshold ||
					criticalPartState.Index > Static.Settings.GaugeShowingThreshold)
						? criticalPartState.Part
						: null);
			}
		}

		/// <summary>Finds the part with the greatest Temp/TempLimit ratio.</summary>
		/// <param name="vessel">Current vessel.</param>
		/// <returns>Critical part state.</returns>
		static PartState GetCriticalPartState(Vessel vessel) =>
			vessel.parts
				.Where(IsPartNotIgnored)
				.Select(GetPartState)
				.OrderByDescending(partState => partState.Index)
				.FirstOrDefault();

		/// <summary>Gets parameters of a part.</summary>
		/// <param name="part">A vessel part.</param>
		/// <returns>Part state.</returns>
		static PartState GetPartState(Part part) =>
			new PartState
			{
				Time = Planetarium.GetUniversalTime(),
				Part = part,
				Id = part.flightID,
				Name = GetPartTitle(part),
				CoreTemperatureLimit = part.maxTemp,
				SkinTemperatureLimit = part.skinMaxTemp,
				CoreTemperature = part.temperature,
				SkinTemperature = part.skinTemperature,
			};

		/// <summary>Determines if the part has a module containing in the exclusion list.</summary>
		/// <param name="part">A vessel part.</param>
		/// <returns><c>false</c> if the part is ignored; <c>true</c> otherwise.</returns>
		static bool IsPartNotIgnored(Part part) =>
			!(Static.Settings.UseExclusionList &&
				Static.Settings.ExclusionListItems.Any(moduleName => part.Modules.Contains(moduleName)));

		/// <summary>Gets the title (long name) of a part.</summary>
		/// <param name="part">A vessel part.</param>
		/// <returns>Part title.</returns>
		static string GetPartTitle(Part part)
		{
			// Removing vessel name from part name
			int indexOfWhitespace = part.name.IndexOf(' ');
			var name = indexOfWhitespace >= 0
				? part.name.Substring(0, indexOfWhitespace)
				: part.name;

			// Trying to get part title; using part name if failed
			try
			{
				var title = PartLoader.getPartInfoByName(name).title;
				return string.IsNullOrEmpty(title) ? title : name;
			}
			catch
			{
				return name;
			}
		}
	}
}
