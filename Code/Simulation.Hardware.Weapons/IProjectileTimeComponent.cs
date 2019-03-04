namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileTimeComponent
	{
		float maxTime
		{
			get;
			set;
		}

		float startTime
		{
			get;
			set;
		}
	}
}
