using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal struct FiringInfo
	{
		public readonly int weaponId;

		public readonly Vector3 direction;

		public readonly Vector3 targetPoint;

		public FiringInfo(int weaponId_, Vector3 direction_, Vector3 targetPoint_)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			this = default(FiringInfo);
			weaponId = weaponId_;
			direction = direction_;
			targetPoint = targetPoint_;
		}
	}
}
