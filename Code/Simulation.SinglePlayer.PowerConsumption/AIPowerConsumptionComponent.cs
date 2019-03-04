namespace Simulation.SinglePlayer.PowerConsumption
{
	internal class AIPowerConsumptionComponent : IAIPowerConsumptionComponent
	{
		public float maxPower
		{
			get;
			set;
		}

		public float power
		{
			get;
			set;
		}

		public float currentWeaponPowerConsumption
		{
			get;
			set;
		}
	}
}
