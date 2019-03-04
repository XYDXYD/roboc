using UnityEngine;

namespace Simulation.Common
{
	internal class DefaultGravityImplementor : IGravityComponent
	{
		public Vector3 gravity => Physics.get_gravity();
	}
}
