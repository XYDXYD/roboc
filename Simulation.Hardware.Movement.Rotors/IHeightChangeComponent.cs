namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IHeightChangeComponent
	{
		float heightMaxChangeSpeed
		{
			get;
		}

		float heightAcceleration
		{
			get;
		}
	}
}
