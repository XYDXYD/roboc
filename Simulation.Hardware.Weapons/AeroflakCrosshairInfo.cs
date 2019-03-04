using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	[Serializable]
	internal class AeroflakCrosshairInfo : BaseCrosshairInfo
	{
		public GameObject Stacks;

		public GameObject GroundWarning;

		public GameObject MoveablePart;

		public int minMoveableSpriteSize = 40;

		public float OffsetAtMinAccuracy = 30f;

		public float stacksDuration = 1f;
	}
}
