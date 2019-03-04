using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	[Serializable]
	internal class WheelColliderData
	{
		public Transform wheelObj;

		public float mass;

		public float radius;

		public float wheelDampingRate;

		public float suspensionDistance;

		public Vector3 centerOffset;

		public float spring = 7000f;

		public float damper = 900f;

		public float targetPosition = 0.2f;

		[NonSerialized]
		public Transform cubeRoot;

		[NonSerialized]
		public int priority;

		[NonSerialized]
		public WheelCollider wheelCollider;

		[NonSerialized]
		public List<IWheelColliderInfo> wheelColliderInfo = new List<IWheelColliderInfo>();

		public bool support;
	}
}
