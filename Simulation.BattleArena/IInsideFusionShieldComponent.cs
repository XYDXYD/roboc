namespace Simulation.BattleArena
{
	internal interface IInsideFusionShieldComponent
	{
		bool isInsideShield
		{
			get;
			set;
		}

		bool isEncapsulated
		{
			get;
			set;
		}

		int teamId
		{
			get;
			set;
		}
	}
}
