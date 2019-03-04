namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface IFrictionAngleComponent
	{
		float angleWithMaxFriction
		{
			get;
		}

		float angleWithMinFriction
		{
			get;
		}

		float minFrictionScalar
		{
			get;
		}
	}
}
