namespace Simulation.Hardware
{
	internal interface IInputMotorComponent
	{
		bool motorForward
		{
			get;
			set;
		}

		bool motorBackward
		{
			get;
			set;
		}
	}
}
