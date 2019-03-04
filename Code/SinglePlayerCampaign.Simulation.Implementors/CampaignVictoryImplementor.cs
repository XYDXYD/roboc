using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class CampaignVictoryImplementor : ICampaignVictoryComponent
	{
		public DispatchOnSet<bool> victoryAchieved
		{
			get;
			private set;
		}

		public CampaignVictoryImplementor(int entityId)
		{
			victoryAchieved = new DispatchOnSet<bool>(entityId);
		}
	}
}
