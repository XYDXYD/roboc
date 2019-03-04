using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IAimingVariablesComponent
	{
		float sqrRotationVelocity
		{
			get;
			set;
		}

		float targetHorizAngle
		{
			get;
			set;
		}

		float targetVertAngle
		{
			get;
			set;
		}

		float currHorizAngle
		{
			get;
			set;
		}

		float currVertAngle
		{
			get;
			set;
		}

		Quaternion initialHorizRot
		{
			get;
			set;
		}

		Quaternion initialVertRot
		{
			get;
			set;
		}

		bool largeAimOffset
		{
			get;
			set;
		}

		bool changingAimQuickly
		{
			get;
			set;
		}

		bool isBlocked
		{
			get;
			set;
		}

		Vector3 targetPoint
		{
			get;
			set;
		}

		Vector3 direction
		{
			get;
			set;
		}

		bool aimToPoint
		{
			get;
		}
	}
}
