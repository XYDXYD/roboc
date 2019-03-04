namespace Simulation.BattleArena
{
	internal interface IFusionShieldHealthChangeComponent
	{
		bool isHealing
		{
			get;
			set;
		}

		bool isTakingDamage
		{
			get;
			set;
		}

		float timeHealthChanging
		{
			get;
			set;
		}
	}
}
