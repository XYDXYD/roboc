using UnityEngine;

namespace Simulation.Hardware.Movement
{
	internal class MachineTopSpeedImplementor : IMachineTopSpeedComponent
	{
		public bool initNeeded
		{
			get;
			set;
		}

		public Vector3 negativeAxisMaxSpeed
		{
			get;
			set;
		}

		public Vector3 positiveAxisMaxSpeed
		{
			get;
			set;
		}

		public float prevSpeedDelta
		{
			get;
			set;
		}

		public float limitTimestamp
		{
			get;
			set;
		}

		public MachineTopSpeedImplementor()
		{
			initNeeded = true;
		}
	}
}
