namespace Simulation.Hardware.Movement
{
	internal interface IStrafingCustomInputComponent
	{
		float forwardInput
		{
			get;
			set;
		}

		float strafingInput
		{
			get;
			set;
		}

		float turningInput
		{
			get;
			set;
		}
	}
}
