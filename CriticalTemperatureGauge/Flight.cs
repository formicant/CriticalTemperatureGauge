using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.UI.Screens;
using Wrappers.ToolbarControl_NS;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Main add-on class.
	/// </summary>
	[KSPAddon(KSPAddon.Startup.Flight, once: false)]
	public class Flight : MonoBehaviour
	{
		readonly Window _gaugeWindow = new GaugeWindow();
		readonly Highlighter _highlighter = new Highlighter();
		readonly SettingsWindow _settingsWindow = new SettingsWindow();
		ToolbarControl _toolbarControl;

		// Toolbar control:

		void CreateToolbarControl()
		{
			if(_toolbarControl is null)
			{
				_toolbarControl = gameObject.AddComponent<ToolbarControl>();
				_toolbarControl.AddToAllToolbars(
					onTrue: OnButtonToggle,
					onFalse: OnButtonToggle,
					visibleInScenes: ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
					nameSpace: Static.PluginId,
					toolbarId: $"{Static.PluginId}Settings",
					largeToolbarIcon: $"{Static.TexturePath}ToolbarIcon-57",
          smallToolbarIcon: $"{Static.TexturePath}ToolbarIcon-24",
					toolTip: Static.PluginTitle);
			}
		}

		void DestroyToolbarControl()
		{
			if(_toolbarControl is object)
			{
				_toolbarControl.OnDestroy();
				Destroy(_toolbarControl);
				_toolbarControl = null;
			}
		}

		void OnButtonToggle()
		{
			_toolbarControl.SetFalse();
			_settingsWindow.Toggle();
		}

		// KSP events:

		public void Start()
		{
			Debug.Log($"[{Static.PluginId}]: Entering scene {HighLogic.LoadedScene}.");
			CreateToolbarControl();
		}

		public void Awake()
		{
			GameEvents.onShowUI.Add(OnShowUI);
			GameEvents.onHideUI.Add(OnHideUI);
		}

		public void OnDestroy()
		{
			try
			{
				Static.Settings.Save();
				GameEvents.onShowUI.Remove(OnShowUI);
				GameEvents.onHideUI.Remove(OnHideUI);
				_gaugeWindow.Hide();
				_settingsWindow.Hide();
				Static.CriticalPartState = null;
				DestroyToolbarControl();
				Debug.Log($"[{Static.PluginId}]: Exiting scene {HighLogic.LoadedScene}.");
			}
			catch(Exception exception)
			{
				Debug.Log($"[{Static.PluginId}]: Exception during exiting scene {HighLogic.LoadedScene}: {exception}");
			}
		}

		void OnShowUI()
		{
			_gaugeWindow.CanShow = true;
			_settingsWindow.CanShow = true;
		}

		void OnHideUI()
		{
			_gaugeWindow.CanShow = false;
			_settingsWindow.CanShow = false;
		}

		public void OnGUI()
		{
			if(_firstGuiDrawing)
				InitiateGui();
			_gaugeWindow.DrawGUI();
			_settingsWindow.DrawGUI();
		}

		// Forcing a GUI element to appear once during scene loading
		// to prevent a delay during flight.
		void InitiateGui()
		{
			_firstGuiDrawing = false;
			GUILayout.Window(Static.BaseWindowId - 1, new Rect(), id => { }, " ");
		}

		static bool _firstGuiDrawing = true;

		public void Update()
		{
			var vessel = FlightGlobals.ActiveVessel;
			if(vessel is object)
			{
				if(Static.Settings.ExclusionListChanged)
				{
					Static.Settings.ExclusionListChanged = false;
					UpdateExcludedParts(vessel);
				}

				// Updating critical part state
				var criticalPartState = GetCriticalPartState(vessel);
				Static.CriticalPartState = criticalPartState;

				// Determining if the gauge should be shown or hidden
				if(!_gaugeWindow.IsLogicallyVisible &&
					criticalPartState is object &&
					(criticalPartState.Index > Static.Settings.GaugeShowingThreshold ||
					Static.Settings.AlwaysShowGauge))
				{
					_gaugeWindow.Show();
				}
				else if(_gaugeWindow.IsLogicallyVisible &&
					(criticalPartState is null ||
					criticalPartState.Index < Static.Settings.GaugeHidingThreshold &&
					!Static.Settings.AlwaysShowGauge))
				{
					_gaugeWindow.Hide();
				}

				// Highlighting the critical part if needed
				_highlighter.SetHighlightedPart(
					Static.Settings.HighlightCriticalPart &&
					criticalPartState is object &&
					(_highlighter.IsThereHighlightedPart &&
					criticalPartState.Index > Static.Settings.GaugeHidingThreshold ||
					criticalPartState.Index > Static.Settings.GaugeShowingThreshold)
						? criticalPartState.part
						: null);
			}
		}

		static void UpdateExcludedParts(Vessel vessel)
		{
			foreach(var part in GetPartTemperatureStates(vessel))
				part.UpdateExclusionStatus();
		}

		static IEnumerable<PartTemperatureState> GetPartTemperatureStates(Vessel vessel) =>
			vessel.parts
				.Select(GetPartState)
				.OfType<PartTemperatureState>();

		/// <summary>Finds the part with the greatest Temp/TempLimit ratio.</summary>
		/// <param name="vessel">Current vessel.</param>
		/// <returns>Critical part state.</returns>
		static PartTemperatureState GetCriticalPartState(Vessel vessel) =>
			GetPartTemperatureStates(vessel)
				.Where(part => part.IsNotIgnored)
				.OrderByDescending(partState => partState.Index)
				.FirstOrDefault();

		/// <summary>Gets parameters of a part. Adds TemperatureState module to the part if absent.</summary>
		/// <param name="part">A vessel part.</param>
		/// <returns>Part state.</returns>
		static PartTemperatureState GetPartState(Part part)
		{
			var partTemperatureState = part.Modules.GetModules<PartTemperatureState>().FirstOrDefault();
			if(partTemperatureState is null)
				part.AddModule(nameof(PartTemperatureState));
			return partTemperatureState;
		}
	}
}
