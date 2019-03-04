using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	internal interface IRemainingLivesComponent
	{
		DispatchOnSet<int> remainingLives
		{
			get;
		}
	}
}
