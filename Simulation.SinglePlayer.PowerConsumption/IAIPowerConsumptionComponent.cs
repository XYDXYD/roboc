namespace Simulation.SinglePlayer.PowerConsumption
{
	public interface IAIPowerConsumptionComponent
	{
		float maxPower
		{
			get;
			set;
		}

		float power
		{
			get;
			set;
		}

		float currentWeaponPowerConsumption
		{
			get;
			set;
		}
	}
}
