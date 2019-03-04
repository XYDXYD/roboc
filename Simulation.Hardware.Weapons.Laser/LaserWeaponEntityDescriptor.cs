using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Laser
{
	internal class LaserWeaponEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static LaserWeaponEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[22]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<LoadWeaponStatsNode>(),
				new EntityViewBuilder<WeaponAimNode>(),
				new EntityViewBuilder<WeaponSwitchNode>(),
				new EntityViewBuilder<LaserWeaponShootingNode>(),
				new EntityViewBuilder<WeaponFireTimingNode>(),
				new EntityViewBuilder<WeaponDamageBuffNode>(),
				new EntityViewBuilder<DamageMultiplierNode>(),
				new EntityViewBuilder<ZoomNode>(),
				new EntityViewBuilder<NextWeaponToFireNode>(),
				new EntityViewBuilder<LaserWeaponEffectsNode>(),
				new EntityViewBuilder<WeaponWithoutLaserPointerNode>(),
				new EntityViewBuilder<ShootingAfterEffectsNode>(),
				new EntityViewBuilder<RobotShakeShootingEntityView>(),
				new EntityViewBuilder<CameraShakeShootingEntityView>(),
				new EntityViewBuilder<WeaponAccuracyNode>(),
				new EntityViewBuilder<CrosshairWeaponTrackerNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<CrosshairWeaponNoFireStateTrackerNode>(),
				new EntityViewBuilder<SmartWeaponFireNode>(),
				new EntityViewBuilder<BlockWeaponFireNode>(),
				new EntityViewBuilder<WeaponTrackerNode>()
			};
		}

		public LaserWeaponEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
