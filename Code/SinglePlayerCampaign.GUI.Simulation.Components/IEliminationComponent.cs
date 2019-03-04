namespace SinglePlayerCampaign.GUI.Simulation.Components
{
	internal interface IEliminationComponent
	{
		int remainingEnemies
		{
			set;
		}

		bool isActive
		{
			set;
		}
	}
}
