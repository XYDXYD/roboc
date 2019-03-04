namespace Simulation.Hardware.Movement
{
	internal interface IValidMovementComponent
	{
		bool isValid
		{
			get;
		}

		bool affectsMaxSpeed
		{
			get;
		}
	}
}
