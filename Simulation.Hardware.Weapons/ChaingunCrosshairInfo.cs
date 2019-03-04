using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	[Serializable]
	internal sealed class ChaingunCrosshairInfo : BaseCrosshairInfo
	{
		public Transform rotatingPart;

		public float rotationSpeed = 360f;

		public UISprite baseSprite;

		public UISprite blurredSprite;

		[Range(0f, 1f)]
		public float blurMaxThreshold = 1f;
	}
}
