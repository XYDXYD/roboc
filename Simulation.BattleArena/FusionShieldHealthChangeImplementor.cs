namespace Simulation.BattleArena
{
	internal class FusionShieldHealthChangeImplementor : IFusionShieldHealthChangeComponent
	{
		public bool isHealing
		{
			get;
			set;
		}

		public bool isTakingDamage
		{
			get;
			set;
		}

		public float timeHealthChanging
		{
			get;
			set;
		}
	}
}
