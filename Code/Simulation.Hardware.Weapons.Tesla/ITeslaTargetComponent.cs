using Svelto.DataStructures;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal interface ITeslaTargetComponent
	{
		bool hasTarget
		{
			get;
			set;
		}

		int playerId
		{
			get;
			set;
		}

		int machineId
		{
			get;
			set;
		}

		TargetType targetType
		{
			get;
			set;
		}

		Transform hitObjectTransform
		{
			get;
			set;
		}

		Vector3 hitPoint
		{
			get;
			set;
		}

		FasterList<Collider> targetColliders
		{
			get;
		}
	}
}
