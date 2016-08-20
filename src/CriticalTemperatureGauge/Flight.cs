using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Main add-on class.
	/// </summary>
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class Flight : MonoBehaviour
	{
		Window _gaugeWindow;

		// KSP events:

		public void Start()
		{
			Debug.Log($"CriticalTemperatureGauge: Entering scene {HighLogic.LoadedScene}.");
			_gaugeWindow = new GaugeWindow();
		}

		public void OnDestroy()
		{
			_gaugeWindow?.Hide();
			Static.CriticalPartState = null;
			Static.Settings.Save();
			Debug.Log($"CriticalTemperatureGauge: Exiting scene {HighLogic.LoadedScene}.");
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
				if(!_gaugeWindow.IsVisible &&
					criticalPartState != null &&
					(criticalPartState.Index > Static.Settings.GaugeShowingThreshold ||
					Static.Settings.ForceShowGauge))
				{
					_gaugeWindow.Show();
					// (Part highlighting does not work for some reason)
					//if(Static.Settings.HighlightCriticalPart)
					//	HighlightPart(vessel, criticalPartState.Id);
				}
				else if(_gaugeWindow.IsVisible &&
					(criticalPartState == null ||
					criticalPartState.Index < Static.Settings.GaugeHidingThreshold &&
					!Static.Settings.ForceShowGauge))
				{
					_gaugeWindow.Hide();
					//HighlightNone(vessel);
				}
			}
		}

		/// <summary>Finds the part with the greatest Temp/TempLimit ratio.</summary>
		/// <param name="vessel">Current vessel.</param>
		/// <returns>Critical part state.</returns>
		PartState GetCriticalPartState(Vessel vessel) =>
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
		bool IsPartNotIgnored(Part part) =>
			!(Static.Settings.UseExclusionList && Static.Settings.ExclusionListItems.Any(moduleName => part.Modules.Contains(moduleName)));

		// Currently, not used
		static void HighlightNone(Vessel craft) => HighlightPart(craft, 0);

		// Currently, not used
		static void HighlightPart(Vessel craft, uint partId)
		{
			foreach(var part in craft.parts)
			{
				if(part.flightID == partId)
					part.highlighter.FlashingOn(Color.red, Color.yellow, 0.5F);
				else
					part.highlighter.FlashingOff();
			}
		}

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
			string title;
			try
			{
				title = PartLoader.getPartInfoByName(name).title;
				if(string.IsNullOrEmpty(title))
					title = name;
			}
			catch
			{
				title = name;
			}
			return title;
		}
	}
}
