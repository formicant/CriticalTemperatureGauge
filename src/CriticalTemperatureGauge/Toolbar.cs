using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.UI.Screens;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Class that adds the toolbar button.
	/// </summary>
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class Toolbar : MonoBehaviour
	{
		// Resources
		static readonly Texture2D ToolbarButtonTexture = GameDatabase.Instance.GetTexture(Static.TexturePath + "ToolbarButton", false);

		readonly ConfigWindow _configWindow = new ConfigWindow();
		ApplicationLauncherButton _toolbarButton;

		// KSP events:

		public void Start()
		{
			OnGUIApplicationLauncherReady();
		}

		public void Awake()
		{
			GameEvents.onGUIApplicationLauncherReady.Add(OnGUIApplicationLauncherReady);
			GameEvents.onGameSceneLoadRequested.Add(OnSceneChangeRequest);
		}

		internal void OnDestroy()
		{
			GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIApplicationLauncherReady);
			GameEvents.onGameSceneLoadRequested.Remove(OnSceneChangeRequest);
			RemoveToolbarButton();
		}

		public void OnGUI()
		{
			_configWindow?.DrawGUI();
		}

		void OnGUIApplicationLauncherReady()
		{
			AddToolbarButton();
		}

		void OnSceneChangeRequest(GameScenes scene)
		{
			RemoveToolbarButton();
		}

		/// <summary>Shows the configuration window when the button is pressed.</summary>
		void OnButtonOn()
		{
			_configWindow?.Show();
		}

		/// <summary>Hides the configuration window when the button is pressed again.</summary>
		void OnButtonOff()
		{
			_configWindow?.Hide();
		}

		/// <summary>Adds the button to the toolbar.</summary>
		void AddToolbarButton()
		{
			if(_toolbarButton == null)
				_toolbarButton = ApplicationLauncher.Instance.AddModApplication(
					onTrue: OnButtonOn,
					onFalse: OnButtonOff,
					onHover: null,
					onHoverOut: null,
					onEnable: null,
					onDisable: null,
					visibleInScenes: ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
					texture: ToolbarButtonTexture);
		}

		/// <summary>Removes the button from the toolbar.</summary>
		void RemoveToolbarButton()
		{
			if(_toolbarButton != null)
				ApplicationLauncher.Instance.RemoveModApplication(_toolbarButton);
		}
	}
}
