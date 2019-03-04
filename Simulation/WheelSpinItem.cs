using System;
using UnityEngine;

namespace Simulation
{
	[Serializable]
	internal sealed class WheelSpinItem
	{
		public Transform spinningTransform;

		public Transform wheelObj;

		public float radius = 1f;

		public float spinScale = 1f;

		public Vector3 axis = Vector3.get_right();

		[NonSerialized]
		public float sizeScale = 1f;

		[NonSerialized]
		public float wheelRadius = 1f;

		public void InitSpinning(float pWheelRadius)
		{
			wheelRadius = pWheelRadius;
			if (radius > 0f)
			{
				sizeScale = wheelRadius / radius;
			}
		}
	}
}
