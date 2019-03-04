using Svelto.DataStructures;
using UnityEngine;

namespace Simulation
{
	internal class MachineCollidersImplementor : IMachineCollidersComponent
	{
		public FasterList<Collider> colliders
		{
			get;
			set;
		}
	}
}
