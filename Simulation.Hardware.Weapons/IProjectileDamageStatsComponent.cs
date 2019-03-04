namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileDamageStatsComponent
	{
		int damage
		{
			get;
			set;
		}

		float protoniumDamageScale
		{
			get;
			set;
		}

		float damageBuff
		{
			get;
			set;
		}

		float damageMultiplier
		{
			get;
			set;
		}

		float damageBoost
		{
			get;
			set;
		}

		float campaignDifficultyFactor
		{
			get;
			set;
		}
	}
}
