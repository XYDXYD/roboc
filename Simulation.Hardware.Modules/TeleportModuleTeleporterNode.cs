using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class TeleportModuleTeleporterNode : EntityView
	{
		public ITeleporterComponent teleporterComponent;

		public ITeleportModuleSettingsComponent settingsComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IHardwareOwnerComponent ownerComponent;

		public TeleportModuleTeleporterNode()
			: this()
		{
		}
	}
}
