using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineWeaponsBlockedNode : EntityView
	{
		public IMachineOwnerComponent machineOwnerComponent;

		public IMachineWeaponsBlockedComponent machineWeaponsBlockedComponent;

		public IRigidBodyComponent machineRigidbodyComponent;

		public IOwnerTeamComponent ownerTeamComponent;

		public MachineWeaponsBlockedNode()
			: this()
		{
		}
	}
}
