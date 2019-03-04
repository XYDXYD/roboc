using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponWithoutLaserPointerNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IFireOrderComponent fireOrderComponent;

		public WeaponWithoutLaserPointerNode()
			: this()
		{
		}
	}
}
