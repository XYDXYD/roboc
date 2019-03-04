using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class WaveVictoryImplementor : IWaveVictoryComponent
	{
		public float timeRequired
		{
			get;
			private set;
		}

		public int killsRequired
		{
			get;
			private set;
		}

		public DispatchOnSet<bool> victoryAchieved
		{
			get;
			private set;
		}

		public WaveVictoryImplementor(float timeRequired_, int killsRequired_, int entityId)
		{
			timeRequired = timeRequired_;
			killsRequired = killsRequired_;
			victoryAchieved = new DispatchOnSet<bool>(entityId);
		}
	}
}
