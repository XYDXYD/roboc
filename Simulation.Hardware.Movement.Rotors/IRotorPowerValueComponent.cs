namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IRotorPowerValueComponent
	{
		float currentPower
		{
			get;
			set;
		}

		float power
		{
			get;
			set;
		}

		float powerChangeRate
		{
			get;
		}
	}
}
