using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class TeleportModuleActivationNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IModuleActivationComponent activationComponent;

		public IModuleRangeComponent rangeComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineDimensionDataComponent machineSizeComponent;

		public ITeleporterComponent startTeleportComponent;

		public IModuleConfirmActivationComponent confirmActivationComponent;

		public TeleportModuleActivationNode()
			: this()
		{
		}
	}
}
