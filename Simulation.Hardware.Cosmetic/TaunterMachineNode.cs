using Svelto.ECS;

namespace Simulation.Hardware.Cosmetic
{
	internal class TaunterMachineNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IRigidBodyComponent rbComponent;

		public TaunterMachineNode()
			: this()
		{
		}
	}
}
