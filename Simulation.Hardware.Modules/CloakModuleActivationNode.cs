using Simulation.Hardware.Weapons;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class CloakModuleActivationNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IModuleActivationComponent activationComponent;

		public IWeaponFireCostComponent manaCostComponent;

		public ICloakMaterialsComponent materialsComponent;

		public ICloakAudioObjectsComponent cloakAudioComponent;

		public CloakModuleActivationNode()
			: this()
		{
		}
	}
}
