using UnityEngine;

namespace Simulation
{
	public class TargetInfo
	{
		public Rigidbody rigidBody
		{
			get;
			set;
		}

		public float horizontalRadius
		{
			get;
			set;
		}

		internal IMachineMap machineMap
		{
			get;
			set;
		}
	}
}
