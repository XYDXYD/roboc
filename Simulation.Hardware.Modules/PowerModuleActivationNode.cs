using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class PowerModuleActivationNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IModuleActivationComponent activationComponent;

		public IModuleConfirmActivationComponent confirmActivationComponent;

		public PowerModuleActivationNode()
			: this()
		{
		}
	}
}
