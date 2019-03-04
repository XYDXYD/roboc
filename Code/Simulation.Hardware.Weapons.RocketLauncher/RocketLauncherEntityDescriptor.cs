using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal class RocketLauncherEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static RocketLauncherEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[24]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<LoadWeaponStatsNode>(),
				new EntityViewBuilder<LoadRocketLauncherStatsNode>(),
				new EntityViewBuilder<WeaponAimNode>(),
				new EntityViewBuilder<LockOnTargetNode>(),
				new EntityViewBuilder<WeaponSwitchNode>(),
				new EntityViewBuilder<RocketLauncherShootingNode>(),
				new EntityViewBuilder<WeaponFireTimingNode>(),
				new EntityViewBuilder<NextWeaponToFireNode>(),
				new EntityViewBuilder<RocketLauncherEffectsNode>(),
				new EntityViewBuilder<DamageMultiplierNode>(),
				new EntityViewBuilder<ZoomNode>(),
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
				new EntityViewBuilder<WeaponDamageBuffNode>(),
				new EntityViewBuilder<WeaponTrackerNode>()
			};
		}

		public RocketLauncherEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
