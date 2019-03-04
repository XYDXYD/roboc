using Simulation.Hardware.Weapons.RocketLauncher;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Nano
{
	internal class NanoWeaponEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static NanoWeaponEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[24]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<LoadWeaponStatsNode>(),
				new EntityViewBuilder<LoadRocketLauncherStatsNode>(),
				new EntityViewBuilder<RocketLauncherShootingNode>(),
				new EntityViewBuilder<LockOnTargetNode>(),
				new EntityViewBuilder<NextWeaponToFireNode>(),
				new EntityViewBuilder<WeaponAimNode>(),
				new EntityViewBuilder<ZoomNode>(),
				new EntityViewBuilder<WeaponDamageBuffNode>(),
				new EntityViewBuilder<DamageMultiplierNode>(),
				new EntityViewBuilder<WeaponFireTimingNode>(),
				new EntityViewBuilder<WeaponSwitchNode>(),
				new EntityViewBuilder<NanoCrosshairUpdaterNode>(),
				new EntityViewBuilder<WeaponWithoutLaserPointerNode>(),
				new EntityViewBuilder<ShootingAfterEffectsNode>(),
				new EntityViewBuilder<RobotShakeShootingEntityView>(),
				new EntityViewBuilder<CameraShakeShootingEntityView>(),
				new EntityViewBuilder<HealingProjectileEffectNode>(),
				new EntityViewBuilder<WeaponAccuracyNode>(),
				new EntityViewBuilder<CrosshairWeaponTrackerNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<CrosshairWeaponNoFireStateTrackerNode>(),
				new EntityViewBuilder<SmartWeaponFireNode>(),
				new EntityViewBuilder<BlockWeaponFireNode>()
			};
		}

		public NanoWeaponEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
