namespace SinglePlayerCampaign.GUI.Simulation.Components
{
	internal interface ITimedEliminationComponent
	{
		int remainingEnemies
		{
			set;
		}

		string timeLeft
		{
			set;
		}

		bool isActive
		{
			set;
		}
	}
}
