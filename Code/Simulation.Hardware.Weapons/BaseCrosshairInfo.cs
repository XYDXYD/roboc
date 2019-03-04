using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	[Serializable]
	internal abstract class BaseCrosshairInfo
	{
		public GameObject CrosshairParent;

		public GameObject Default;

		public GameObject Damage;

		public GameObject CriticalHit;

		public GameObject NoFire;

		public float DamageMinScale = 1f;

		public float DamageMaxScale = 2f;

		public float DamageScaleIncreasePerHit = 0.1f;

		public float DamageScaleDecreasePerSecond = 0.5f;
	}
}
