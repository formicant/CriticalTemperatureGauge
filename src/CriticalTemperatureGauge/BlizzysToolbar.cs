using System;
using System.Collections.Generic;
using System.Linq;

namespace CriticalTemperatureGauge
{
	public class BlizzysToolbar
	{
		const string ButtonId = "Settings";
		const string ButtonTexturePath = Static.TexturePath + "BizzysToolbarButton";

		IButton _toolbarButton;

		public void Start()
		{
			if(ToolbarManager.ToolbarAvailable)
			{
				_toolbarButton = ToolbarManager.Instance.add(nameof(CriticalTemperatureGauge), nameof(CriticalTemperatureGauge) + ButtonId);
				_toolbarButton.TexturePath = ButtonTexturePath;
				_toolbarButton.ToolTip = Static.PluginTitle;
				_toolbarButton.OnClick += e =>
				{
					if(Static.AppLauncher != null && e.MouseButton == 0 /* Left mouse button */)
						Static.AppLauncher.ButtonState = !Static.AppLauncher.ButtonState;
				};
			}
		}

		public void Destroy()
		{
			_toolbarButton?.Destroy();
		}
	}
}
