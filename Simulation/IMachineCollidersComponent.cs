using Svelto.DataStructures;
using UnityEngine;

namespace Simulation
{
	internal interface IMachineCollidersComponent
	{
		FasterList<Collider> colliders
		{
			get;
			set;
		}
	}
}
