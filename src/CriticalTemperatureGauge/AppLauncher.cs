using System;
using System.Collections.Generic;
using System.Linq;
using KSP.UI.Screens;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Class that adds the AppLauncher button.
	/// </summary>
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class AppLauncher : MonoBehaviour
	{
		// Resources
		static readonly Texture2D AppLauncherButtonTexture =
			GameDatabase.Instance.GetTexture(Static.TexturePath + "AppLauncherButton", false);

		ApplicationLauncherButton _appLauncherButton;
		readonly SettingsWindow _settingsWindow = new SettingsWindow();

		public void Update()
		{
			if(_appLauncherButton != null)
				_appLauncherButton.VisibleInScenes =
					Static.Settings.ShowAppLauncherButton
						? ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW
						: ApplicationLauncher.AppScenes.NEVER;
		}

		public bool ButtonState
		{
			get => _settingsWindow.IsLogicallyVisible;
			set
			{
				if(value)
					_appLauncherButton?.SetTrue();
				else
					_appLauncherButton?.SetFalse();
			}
		}

		// KSP events:

		public void Start()
		{
			Static.AppLauncher = this;
		}

		public void Awake()
		{
			GameEvents.onGUIApplicationLauncherReady.Add(OnGUIApplicationLauncherReady);
			GameEvents.onGameSceneLoadRequested.Add(OnSceneChangeRequest);
			GameEvents.onShowUI.Add(OnShowUI);
			GameEvents.onHideUI.Add(OnHideUI);
		}

		internal void OnDestroy()
		{
			GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIApplicationLauncherReady);
			GameEvents.onGameSceneLoadRequested.Remove(OnSceneChangeRequest);
			GameEvents.onShowUI.Remove(OnShowUI);
			GameEvents.onHideUI.Remove(OnHideUI);
			RemoveAppLauncherButton();
			Static.AppLauncher = null;
		}

		public void OnGUI()
		{
			_settingsWindow.DrawGUI();
		}

		void OnGUIApplicationLauncherReady()
		{
			AddAppLauncherButton();
		}

		void OnSceneChangeRequest(GameScenes scene)
		{
			RemoveAppLauncherButton();
		}

		void OnShowUI()
		{
			_settingsWindow.CanShow = true;
		}

		void OnHideUI()
		{
			_settingsWindow.CanShow = false;
		}

		/// <summary>Shows the settings window when the button is pressed.</summary>
		void OnButtonOn()
		{
			_settingsWindow.Show();
		}

		/// <summary>Hides the settings window when the button is pressed again.</summary>
		void OnButtonOff()
		{
			_settingsWindow.Hide();
		}

		/// <summary>Adds the button to the appLauncher.</summary>
		void AddAppLauncherButton()
		{
			if(_appLauncherButton == null)
				_appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
					onTrue: OnButtonOn,
					onFalse: OnButtonOff,
					onHover: null,
					onHoverOut: null,
					onEnable: null,
					onDisable: null,
					visibleInScenes: ApplicationLauncher.AppScenes.NEVER,
					texture: AppLauncherButtonTexture);
			Update();
		}

		/// <summary>Removes the button from the appLauncher.</summary>
		void RemoveAppLauncherButton()
		{
			if(_appLauncherButton != null)
				ApplicationLauncher.Instance.RemoveModApplication(_appLauncherButton);
		}
	}
}
