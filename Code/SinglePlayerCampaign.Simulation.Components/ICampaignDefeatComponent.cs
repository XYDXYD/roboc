using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	internal interface ICampaignDefeatComponent
	{
		DispatchOnSet<bool> defeatHappened
		{
			get;
		}
	}
}
