using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class TimeImplementor : ITimeComponent
	{
		public float elapsedTime
		{
			get;
			set;
		}

		public DispatchOnSet<bool> timeRunning
		{
			get;
			private set;
		}

		public TimeImplementor(int entityId)
		{
			timeRunning = new DispatchOnSet<bool>(entityId);
		}
	}
}
