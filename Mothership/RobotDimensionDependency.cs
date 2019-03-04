using UnityEngine;

namespace Mothership
{
	internal class RobotDimensionDependency
	{
		public readonly Vector3 minBounds;

		public readonly Vector3 maxBounds;

		public RobotDimensionDependency(Vector3 minBounds_, Vector3 maxBounds_)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			minBounds = minBounds_;
			maxBounds = maxBounds_;
		}
	}
}
