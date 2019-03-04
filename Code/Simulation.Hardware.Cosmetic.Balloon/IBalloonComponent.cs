using UnityEngine;

namespace Simulation.Hardware.Cosmetic.Balloon
{
	public interface IBalloonComponent
	{
		Transform[] joints
		{
			get;
		}

		float springStength
		{
			get;
		}

		float damping
		{
			get;
		}

		float lateralInfluence
		{
			get;
		}

		float angularInfluence
		{
			get;
		}

		Vector3 lastPos
		{
			get;
			set;
		}

		float lastTime
		{
			get;
			set;
		}

		Vector3 lastVelocity
		{
			get;
			set;
		}

		Vector3 lastRotationEuler
		{
			get;
			set;
		}

		Vector3 stalkRotationVelocity
		{
			get;
			set;
		}
	}
}
