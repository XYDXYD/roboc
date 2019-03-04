namespace Simulation.Hardware.Movement.TankTracks
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
