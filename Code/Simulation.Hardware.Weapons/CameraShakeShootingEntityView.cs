using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class CameraShakeShootingEntityView : EntityView
	{
		public IWeaponRotationTransformsComponent weaponRotationTransforms;

		public ICameraShakeComponent camShakeComponent;

		public CameraShakeShootingEntityView()
			: this()
		{
		}
	}
}
