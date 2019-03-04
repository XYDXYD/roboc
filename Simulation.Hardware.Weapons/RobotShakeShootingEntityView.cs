using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class RobotShakeShootingEntityView : EntityView
	{
		public IWeaponRotationTransformsComponent weaponRotationTransforms;

		public IRobotShakeComponent robotShakeComponent;

		public RobotShakeShootingEntityView()
			: this()
		{
		}
	}
}
