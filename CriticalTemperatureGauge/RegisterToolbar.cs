using UnityEngine;
using ToolbarControl_NS;

namespace CriticalTemperatureGauge
{
	[KSPAddon(KSPAddon.Startup.MainMenu, once: true)]
	public class RegisterToolbar : MonoBehaviour
	{
		void Start()
		{
			ToolbarControl.RegisterMod(Static.PluginId, Static.PluginTitle);
		}
	}
}
