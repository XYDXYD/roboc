using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	[Serializable]
	internal sealed class NanoWeaponCrosshairInfo : BaseCrosshairInfo
	{
		public GameObject RangeIndicator;

		public GameObject Lock;
	}
}
