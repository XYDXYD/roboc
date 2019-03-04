using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	public class MachineTargetsEntityView : EntityView
	{
		public IRigidBodyComponent rigidBodyComponent;

		public IOwnerTeamComponent ownerTeamComponent;

		public IMachineOwnerComponent machineOwnerComponent;

		public IMachineTargetInfoComponent machineTargetInfoComponent;

		public IMachineTargetsComponent machineTargetsComponent;

		public IMachineVisibilityComponent machineVisibilityComponent;

		public MachineTargetsEntityView()
			: this()
		{
		}
	}
}
