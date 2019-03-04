using Simulation.BattleArena;
using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal sealed class CurableMachineNode : EntityView
	{
		public IAliveStateComponent aliveStateComponent;

		public IMachineOwnerComponent machineOwnerComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IOwnerTeamComponent ownerTeamComponent;

		public IInsideFusionShieldComponent insideFusionShieldComponent;

		public IFusionShieldHealthChangeComponent fusionShieldHealthChangeComponent;

		public CurableMachineNode()
			: this()
		{
		}
	}
}
