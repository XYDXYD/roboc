namespace Simulation.Hardware.Movement.Wheeled
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
