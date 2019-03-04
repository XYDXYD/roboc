using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class PowerModuleEffectsNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IRigidBodyComponent rigidBodyComponent;

		public IMachineDimensionDataComponent machineDimensionComponent;

		public IPowerModuleEffectsComponent effectsComponent;

		public PowerModuleEffectsNode()
			: this()
		{
		}
	}
}
