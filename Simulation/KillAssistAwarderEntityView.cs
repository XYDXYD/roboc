using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal class KillAssistAwarderEntityView : EntityView
	{
		public IDamagedByComponent damagedByComponent;

		public IMachineOwnerComponent ownerComponent;

		public IAliveStateComponent aliveStateComponent;

		public KillAssistAwarderEntityView()
			: this()
		{
		}
	}
}
