using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	internal interface ICampaignVictoryComponent
	{
		DispatchOnSet<bool> victoryAchieved
		{
			get;
		}
	}
}
