namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface ITrackGroundedComponent
	{
		bool grounded
		{
			get;
			set;
		}

		int groundedWheelCount
		{
			get;
			set;
		}
	}
}
