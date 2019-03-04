using UnityEngine;

namespace Simulation
{
	internal sealed class RemoteLegManager : LegManager
	{
		public RemoteLegManager(Rigidbody rb)
		{
			_legGraphics = new LegGraphicsManager(rb.get_gameObject());
		}

		public override void Tick(float deltaTime)
		{
			if (_legs.Count > 0)
			{
				_legGraphics.Tick(deltaTime);
			}
		}

		public override void PhysicsTick(float deltaTime)
		{
			if (_legs.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeLeg cubeLeg = _legs[i];
				if (!(cubeLeg.T == null) && !cubeLeg.isHidden)
				{
					cubeLeg.UpdateCachedValues();
					ProcessCurrentVelocity(cubeLeg, deltaTime);
					UpdateTargetGroundedPosition(cubeLeg, deltaTime);
				}
			}
		}
	}
}
