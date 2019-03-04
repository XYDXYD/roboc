using UnityEngine;

namespace Simulation.BattleArena.Equalizer
{
	internal interface IRigidbodyComponent
	{
		Rigidbody rb
		{
			get;
			set;
		}

		Vector3 activePosition
		{
			get;
			set;
		}

		Vector3 inactivePosition
		{
			get;
			set;
		}
	}
}
