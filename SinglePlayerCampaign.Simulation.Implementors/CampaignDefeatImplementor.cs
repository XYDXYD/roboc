using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class CampaignDefeatImplementor : ICampaignDefeatComponent
	{
		public DispatchOnSet<bool> defeatHappened
		{
			get;
			private set;
		}

		public CampaignDefeatImplementor(int entityId)
		{
			defeatHappened = new DispatchOnSet<bool>(entityId);
		}
	}
}
