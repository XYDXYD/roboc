using Svelto.ECS;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface ICollisionComponent
	{
		DispatchOnSet<Collider> onTriggerEnter
		{
			get;
		}

		DispatchOnSet<Collider> onTriggerExit
		{
			get;
		}
	}
}
