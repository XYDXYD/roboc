using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	[Serializable]
	internal class RocketLauncherCrosshairInfo : BaseCrosshairInfo
	{
		public List<GameObject> Locks;

		public GameObject TargetIndicator;

		public GameObject AcquiringTargetIndicator;
	}
}
