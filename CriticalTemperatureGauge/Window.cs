using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ClickThroughFix;

namespace CriticalTemperatureGauge
{
	/// <summary>
	/// Represents a GUI window.
	/// </summary>
	public abstract class Window
	{
		public int WindowId { get; }

		public string Title { get; }

		/// <summary>Is the background of the window transparent.</summary>
		public bool HasClearBackground { get; }

		/// <summary>Do not use ClickThroughBlocker.</summary>
		public bool ClickThrough { get; }

		/// <summary>Should the window be visible? (Disregarding the F2 key.)</summary>
		public bool IsLogicallyVisible { get; private set; }

		/// <summary><c>false</c>, if the F2 key pressed, <c>true</c> otherwise.</summary>
		public bool CanShow { get; set; } = true;

		/// <summary>Is the window visible?</summary>
		public bool IsVisible => CanShow && IsLogicallyVisible;

		Rect? _windowRectangle;
		public Rect WindowRectangle
		{
			get => _windowRectangle ?? (_windowRectangle = InitialWindowRectangle).Value;
			private set
			{
				_windowRectangle = value;
				OnWindowRectUpdated();
			}
		}

		protected virtual Vector2? ConstWindowPosition => null;
		protected virtual Vector2? ConstWindowSize => null;

		protected Window(int windowId, string title = "", bool hasClearBackground = false, bool clickThrough = false)
		{
			WindowId = windowId;
			Title = title;
			HasClearBackground = hasClearBackground;
			ClickThrough = clickThrough;
		}

		protected abstract GUISkin Skin { get; }

		public void Show()
		{
			IsLogicallyVisible = true;
		}

		public void Hide()
		{
			IsLogicallyVisible = false;
		}

		public void Toggle()
		{
			IsLogicallyVisible = !IsLogicallyVisible;
		}

		public void DrawGUI()
		{
			if(IsVisible)
			{
				GUI.skin = Skin;
				var backgroundColor = GUI.backgroundColor;
				if(HasClearBackground)
					GUI.backgroundColor = Color.clear;
				var windowRectangle = ClickThrough
					? GUILayout.Window(WindowId, WindowRectangle, WindowGUI, Title)
					: ClickThruBlocker﻿.GUILayoutWindow(WindowId, WindowRectangle, WindowGUI, Title);
				WindowRectangle = new Rect(
					ConstWindowPosition ?? windowRectangle.position,
					ConstWindowSize ?? windowRectangle.size);
				GUI.backgroundColor = backgroundColor;
			}
		}

		protected abstract Rect InitialWindowRectangle { get; }

		protected abstract void OnWindowRectUpdated();

		protected abstract void WindowGUI(int windowId);
	}
}
