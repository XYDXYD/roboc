using UnityEngine;

namespace Simulation.Hardware.Movement.Hovers
{
	public interface IHoverInfoComponent
	{
		Transform forcePointTransform
		{
			get;
		}

		float distanceToGround
		{
			get;
			set;
		}

		float validGroundScalar
		{
			get;
			set;
		}

		float initialYPos
		{
			get;
		}

		Vector3 lastPos
		{
			get;
			set;
		}

		int resetLastPosUpdates
		{
			get;
			set;
		}

		RaycastHit raycastHit
		{
			get;
			set;
		}
	}
}
