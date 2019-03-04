using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal struct HitResult
	{
		public TargetType targetType;

		public int hitTargetMachineId;

		public Vector3 hitPoint;

		public Vector3 normal;

		public bool hitSelf;

		public bool hitOwnBase;

		public bool hitAlly;

		internal GridAllignedLineCheck.GridAlignedHitResult gridHit;

		internal void Initialise(ref WeaponRaycastUtility.Ray ray)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			hitPoint = ray.startPosition + ray.direction * ray.range;
			normal = -ray.direction;
			gridHit = default(GridAllignedLineCheck.GridAlignedHitResult);
			hitSelf = false;
			hitOwnBase = false;
			hitTargetMachineId = -1;
			targetType = TargetType.Environment;
			hitAlly = false;
		}
	}
}
