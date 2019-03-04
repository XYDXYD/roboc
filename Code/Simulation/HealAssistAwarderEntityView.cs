using Svelto.ECS;

namespace Simulation
{
	internal sealed class HealAssistAwarderEntityView : EntityView
	{
		public IHealAssistComponent healAssistComponent;

		public IAliveStateComponent aliveStateComponent;

		public HealAssistAwarderEntityView()
			: this()
		{
		}
	}
}
