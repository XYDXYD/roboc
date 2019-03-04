using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class RemainingLivesImplementor : IRemainingLivesComponent
	{
		public DispatchOnSet<int> remainingLives
		{
			get;
			private set;
		}

		public RemainingLivesImplementor(int initialLives, int entityId)
		{
			DispatchOnSet<int> val = new DispatchOnSet<int>(entityId);
			val.set_value(initialLives);
			remainingLives = val;
		}
	}
}
