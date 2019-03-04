using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	[Serializable]
	internal class MortarCrosshairInfo : BaseCrosshairInfo
	{
		public GameObject GroundWarning;

		public UILabel horizonAngleLabel;

		public float numberUpdateRate = 0.25f;
	}
}
