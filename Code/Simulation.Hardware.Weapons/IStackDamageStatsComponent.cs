namespace Simulation.Hardware.Weapons
{
	internal interface IStackDamageStatsComponent
	{
		float buffStackExpireTime
		{
			get;
			set;
		}

		int buffDamagePerStack
		{
			get;
			set;
		}

		int buffMaxStacks
		{
			get;
			set;
		}
	}
}
