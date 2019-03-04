using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class KillCountImplementor : IKillCountComponent
	{
		public DispatchOnSet<int> killCount
		{
			get;
			set;
		}

		public KillCountImplementor(int entityId)
		{
			killCount = new DispatchOnSet<int>(entityId);
		}
	}
}
