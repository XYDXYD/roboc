namespace Simulation.Hardware.Weapons
{
	public interface IWeaponSpinStatsComponent
	{
		float spinUpTime
		{
			get;
			set;
		}

		float spinDownTime
		{
			get;
			set;
		}

		float spinInitialCooldown
		{
			get;
			set;
		}
	}
}
