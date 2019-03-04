namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IMassComponent
	{
		float mass
		{
			get;
			set;
		}

		float modifiedMass
		{
			get;
			set;
		}
	}
}
