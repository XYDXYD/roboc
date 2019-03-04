using Svelto.ECS;

namespace Simulation.Hardware.Cosmetic
{
	internal class ExhaustEffectEntityView : EntityView
	{
		public ITogglableCosmeticParticlesComponent exhaustComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public ExhaustEffectEntityView()
			: this()
		{
		}
	}
}
