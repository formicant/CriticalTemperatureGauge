using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CriticalTemperatureGauge
{
	public class PartTemperatureState : PartModule
	{
		[KSPField(category = Static.FieldCategory, isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "#LOC_CriticalTemperatureGauge_PartMenuCore")]
		public string CoreTemperatureMenuLabel;
		[KSPField(category = Static.FieldCategory, isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "#LOC_CriticalTemperatureGauge_PartMenuSkin")]
		public string SkinTemperatureMenuLabel;

		BaseField CoreTemperatureField { get; set; }
		BaseField SkinTemperatureField { get; set; }

		/// <summary>Moment of time the parameters were measured at.</summary>
		public double Time { get; private set; }

		/// <summary>Part localized name.</summary>
		public string Title { get; private set; }

		// Thermal parameters
		public double CoreTemperatureLimit { get; private set; }
		public double SkinTemperatureLimit { get; private set; }
		public double CoreTemperature { get; private set; }
		public double SkinTemperature { get; private set; }
		public double CoreTemperatureRate { get; private set; }
		public double SkinTemperatureRate { get; private set; }

		// Critical thermal parameters
		public bool IsSkinCritical => CoreTemperature / CoreTemperatureLimit < SkinTemperature / SkinTemperatureLimit;
		public double CriticalTemperatureLimit => IsSkinCritical ? SkinTemperatureLimit : CoreTemperatureLimit;
		public double CriticalTemperature => IsSkinCritical ? SkinTemperature : CoreTemperature;
		public double CriticalTemperatureRate => IsSkinCritical ? SkinTemperatureRate : CoreTemperatureRate;

		/// <summary>A number between 0 and 1 showing how critical the part is.</summary>
		public double Index => Math.Max(CoreTemperature / CoreTemperatureLimit, SkinTemperature / SkinTemperatureLimit);

		/// <summary>The part is not in the exclusion list.</summary>
		public bool IsNotIgnored { get; private set; }

		public void UpdateExclusionStatus()
		{
			IsNotIgnored = !(Static.Settings.UseExclusionList && (
				Static.Settings.ExcludedNames.Contains(part.partInfo.name) ||
				Static.Settings.ExcludedNames.Contains(part.partInfo.title) ||
				part.Modules.OfType<PartModule>().Any(module =>
					Static.Settings.ExcludedModules.Contains(module.moduleName))));
			if(!IsNotIgnored)
			{
				CoreTemperatureMenuLabel = null;
				SkinTemperatureMenuLabel = null;
				CoreTemperatureField.guiActive = false;
				SkinTemperatureField.guiActive = false;
			}
		}

		public void Start()
		{
			CoreTemperatureField = Fields[nameof(CoreTemperatureMenuLabel)];
			SkinTemperatureField = Fields[nameof(SkinTemperatureMenuLabel)];
			StartCoroutine(InitValues());
		}

		IEnumerator InitValues()
		{
			while(!HighLogic.LoadedSceneIsFlight || vessel.HoldPhysics)
				yield return new WaitForFixedUpdate();

			Time = Planetarium.GetUniversalTime();
			Title = part.partInfo.title;
			CoreTemperatureLimit = part.maxTemp;
			SkinTemperatureLimit = part.skinMaxTemp;
			CoreTemperature = part.temperature;
			SkinTemperature = part.skinTemperature;
			CoreTemperatureRate = 0;
			SkinTemperatureRate = 0;
			UpdateExclusionStatus();
		}

		public void Update()
		{
			if(vessel.isActiveVessel)
			{
				if(IsNotIgnored && HighLogic.LoadedSceneIsFlight && !vessel.HoldPhysics)
				{
					if(Static.Settings.PartMenuTemperature)
					{
						// Updating the part menu values
						CoreTemperatureMenuLabel =
							Format.TemperatureWithRate(
								CoreTemperature,
								Static.Settings.PartMenuTemperatureLimit.Then(CoreTemperatureLimit),
								Static.Settings.PartMenuTemperatureRate.Then(CoreTemperatureRate));
						SkinTemperatureMenuLabel =
							Format.TemperatureWithRate(
								SkinTemperature,
								Static.Settings.PartMenuTemperatureLimit.Then(SkinTemperatureLimit),
								Static.Settings.PartMenuTemperatureRate.Then(SkinTemperatureRate));

						CoreTemperatureField.guiActive = true;
						SkinTemperatureField.guiActive = true;
					}
					else
					{
						CoreTemperatureMenuLabel = null;
						SkinTemperatureMenuLabel = null;
						CoreTemperatureField.guiActive = false;
						SkinTemperatureField.guiActive = false;
					}
				}
			}
			else // not a part of the active vessel
				part.RemoveModule(this);
		}

		public void FixedUpdate()
		{
			if(IsNotIgnored && HighLogic.LoadedSceneIsFlight && !vessel.HoldPhysics)
			{
				var newTime = Planetarium.GetUniversalTime();
				double timeSpan = 2 * (newTime - Time);
				if(Time > 0 && timeSpan > 0)
				{
					// Updating the temperature rate values
					CoreTemperatureRate =
						(1D - CoreTemperatureRateSmoothing) * CoreTemperatureRate +
						CoreTemperatureRateSmoothing * (part.temperature - CoreTemperature) / timeSpan;
					SkinTemperatureRate =
						(1D - SkinTemperatureRateSmoothing) * SkinTemperatureRate +
						SkinTemperatureRateSmoothing * (part.skinTemperature - SkinTemperature) / timeSpan;

					CoreTemperature = part.temperature;
					SkinTemperature = part.skinTemperature;
					Time = newTime;
				}
			}
		}

		// Temperature rate value temporal smoothing
		const double CoreTemperatureRateSmoothing = 0.5;
		const double SkinTemperatureRateSmoothing = 0.1;
	}
}
