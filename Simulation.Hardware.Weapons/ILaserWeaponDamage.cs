namespace Simulation.Hardware.Weapons
{
	internal interface ILaserWeaponDamage
	{
		float MaxDamageDistance
		{
			get;
		}

		int MinDamage
		{
			get;
		}

		float MinDamageDistance
		{
			get;
		}
	}
}
