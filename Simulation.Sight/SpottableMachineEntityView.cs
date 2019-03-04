using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.Sight
{
	internal sealed class SpottableMachineEntityView : EntityView
	{
		public ISpottableComponent spottableComponent;

		public IMachineOwnerComponent ownerComponent;

		public IOwnerTeamComponent teamComponent;

		public IMachineVisibilityComponent visibilityComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IAliveStateComponent aliveComponent;

		public IEntitySourceComponent sourceComponent;

		public SpottableMachineEntityView()
			: this()
		{
		}
	}
}
