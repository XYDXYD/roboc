using UnityEngine;

namespace Simulation.Hardware.Movement
{
	internal interface IMachineTopSpeedComponent
	{
		Vector3 positiveAxisMaxSpeed
		{
			get;
			set;
		}

		Vector3 negativeAxisMaxSpeed
		{
			get;
			set;
		}

		float prevSpeedDelta
		{
			get;
			set;
		}

		float limitTimestamp
		{
			get;
			set;
		}

		bool initNeeded
		{
			get;
			set;
		}
	}
}
