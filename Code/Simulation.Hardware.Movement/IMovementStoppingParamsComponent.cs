namespace Simulation.Hardware.Movement
{
	internal interface IMovementStoppingParamsComponent
	{
		float stoppingDampingScale
		{
			get;
		}

		float slowingDampingScale
		{
			get;
		}
	}
}
