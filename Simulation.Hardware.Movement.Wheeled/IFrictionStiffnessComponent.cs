namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface IFrictionStiffnessComponent
	{
		float stoppedFrictionStiffness
		{
			get;
		}

		float movingFrictionStiffness
		{
			get;
		}
	}
}
