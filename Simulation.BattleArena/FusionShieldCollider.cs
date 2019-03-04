using UnityEngine;

namespace Simulation.BattleArena
{
	internal struct FusionShieldCollider
	{
		public Vector3 localCenter;

		public float radiusSq;

		public FusionShieldCollider(Vector3 _localCenter, float _radius)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			localCenter = _localCenter;
			radiusSq = _radius * _radius;
		}
	}
}
