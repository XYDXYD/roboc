using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.BattleArena
{
	internal class InsideFusionShieldEntityView : EntityView
	{
		public IAliveStateComponent aliveStateComponent;

		public IInsideFusionShieldComponent insideFusionShieldComponent;

		public IRigidBodyComponent rigidBodyComponent;

		public IMachineOwnerComponent machineOwnerComponent;

		public IOwnerTeamComponent ownerTeamComponent;

		public InsideFusionShieldEntityView()
			: this()
		{
		}
	}
}
