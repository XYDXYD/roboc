using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	internal interface IWaveDefeatComponent
	{
		float timeLimit
		{
			get;
		}

		DispatchOnSet<bool> defeatHappened
		{
			get;
		}
	}
}
