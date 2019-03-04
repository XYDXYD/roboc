using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class TeleportModuleEffectsNode : EntityView
	{
		public ITeleportModuleEffectsComponent effectsComponent;

		public ITeleporterComponent teleporterComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IHardwareOwnerComponent ownerComponent;

		public ITeleportModuleSettingsComponent settingsComponent;

		public TeleportModuleEffectsNode()
			: this()
		{
		}
	}
}
