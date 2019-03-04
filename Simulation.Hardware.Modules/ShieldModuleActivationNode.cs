using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ShieldModuleActivationNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IModuleActivationComponent activationComponent;

		public IModuleRangeComponent rangeComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IModuleConfirmActivationComponent confirmActivationComponent;

		public ShieldModuleActivationNode()
			: this()
		{
		}
	}
}
