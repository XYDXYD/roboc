using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	internal interface IWaveVictoryComponent
	{
		float timeRequired
		{
			get;
		}

		int killsRequired
		{
			get;
		}

		DispatchOnSet<bool> victoryAchieved
		{
			get;
		}
	}
}
