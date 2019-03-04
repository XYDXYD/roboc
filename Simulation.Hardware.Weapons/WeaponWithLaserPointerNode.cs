using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponWithLaserPointerNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IWeaponMuzzleFlash muzzleComponent;

		public IProjectileRangeComponent projectileRangeStats;

		public IFireOrderComponent fireOrderComponent;

		public WeaponWithLaserPointerNode()
			: this()
		{
		}
	}
}
