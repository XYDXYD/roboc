using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	[Serializable]
	internal sealed class RailGunCrosshairInfo : BaseCrosshairInfo
	{
		public GameObject MoveablePart;

		public float OffsetAtMinAccuracy = 60f;
	}
}
