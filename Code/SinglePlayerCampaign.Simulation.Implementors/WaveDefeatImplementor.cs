using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class WaveDefeatImplementor : IWaveDefeatComponent
	{
		public float timeLimit
		{
			get;
			private set;
		}

		public DispatchOnSet<bool> defeatHappened
		{
			get;
			private set;
		}

		public WaveDefeatImplementor(float timeLimit_, int entityId)
		{
			timeLimit = timeLimit_;
			defeatHappened = new DispatchOnSet<bool>(entityId);
		}
	}
}
