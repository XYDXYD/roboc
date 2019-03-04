using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ModuleReadyEffectNode : EntityView
	{
		public IReadyEffectActivationComponent readyEffectActivationComponent;

		public IHardwareOwnerComponent ownerComponent;

		public ModuleReadyEffectNode()
			: this()
		{
		}
	}
}
