using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface ISlipComponent
	{
		float forwardSlip
		{
			get;
			set;
		}

		float sidewaysSlip
		{
			get;
			set;
		}

		Vector3 sidewaysDir
		{
			get;
			set;
		}
	}
}
