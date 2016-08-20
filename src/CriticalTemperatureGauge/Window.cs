using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Represents a GUI window.
	/// </summary>
	public abstract class Window
	{
		public int WindowId { get; }
		public string Title { get; }
		public bool HasClearBackground { get; }
		public bool IsVisible { get; private set; }

		Rect? _windowRectangle;
		public Rect WindowRectangle
		{
			get
			{
				return _windowRectangle ?? (_windowRectangle = InitialWindowRectangle).Value;
			}
			private set
			{
				_windowRectangle = value;
				OnWindowRectUpdated();
			}
		}

		protected Window(int windowId, string title = "", bool hasClearBackground = false)
		{
			WindowId = windowId;
			Title = title;
			HasClearBackground = hasClearBackground;
		}

		public void Show()
		{
			IsVisible = true;
		}

		public void Hide()
		{
			IsVisible = false;
		}

		public void DrawGUI()
		{
			if(IsVisible)
			{
				GUI.skin = HighLogic.Skin;
				var backgroundColor = GUI.backgroundColor;
				if(HasClearBackground)
					GUI.backgroundColor = Color.clear;
				WindowRectangle = GUILayout.Window(WindowId, WindowRectangle, WindowGUI, Title);
				GUI.backgroundColor = backgroundColor;
			}
		}

		protected abstract Rect InitialWindowRectangle { get; }

		protected abstract void OnWindowRectUpdated();

		protected abstract void WindowGUI(int windowId);
	}
}
