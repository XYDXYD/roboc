using Simulation.Hardware.Weapons;
using UnityEngine;

namespace Simulation
{
	internal sealed class PhysicsStatusFactory
	{
		public void MachineBuilt(Rigidbody rb, IMachineMap map, TargetType targetType, bool computeInertiaTensor = true)
		{
			PhysicsStatusSetter physicsStatusSetter = new PhysicsStatusSetter(rb, map, computeInertiaTensor, targetType);
		}
	}
}
