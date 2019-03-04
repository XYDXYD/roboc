namespace Simulation.BattleArena
{
	internal class InsideFusionShieldImplementor : IInsideFusionShieldComponent
	{
		private int _teamId = -1;

		public bool isInsideShield
		{
			get;
			set;
		}

		public bool isEncapsulated
		{
			get;
			set;
		}

		public int teamId
		{
			get
			{
				return _teamId;
			}
			set
			{
				_teamId = value;
			}
		}
	}
}
