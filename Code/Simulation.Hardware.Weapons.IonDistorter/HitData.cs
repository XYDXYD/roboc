using UnityEngine;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal class HitData
	{
		public TargetType type;

		public Vector3 hitPoint;

		public Vector3 normal;

		public Rigidbody rb;

		public int machineId;

		public HitData(TargetType type_, Vector3 hitPoint_, Vector3 normal_, Rigidbody rb_, int machineId_)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			type = type_;
			hitPoint = hitPoint_;
			normal = normal_;
			rb = rb_;
			machineId = machineId_;
		}
	}
}
