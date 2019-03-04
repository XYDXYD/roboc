using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Plasma
{
	internal class PlasmaWeaponEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static PlasmaWeaponEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[23]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<LoadPlasmaWeaponStatsNode>(),
				new EntityViewBuilder<LoadWeaponStatsNode>(),
				new EntityViewBuilder<PlasmaWeaponShootingNode>(),
				new EntityViewBuilder<NextWeaponToFireNode>(),
				new EntityViewBuilder<WeaponAimNode>(),
				new EntityViewBuilder<WeaponDamageBuffNode>(),
				new EntityViewBuilder<DamageMultiplierNode>(),
				new EntityViewBuilder<WeaponFireTimingNode>(),
				new EntityViewBuilder<WeaponSwitchNode>(),
				new EntityViewBuilder<ZoomNode>(),
				new EntityViewBuilder<PlasmaWeaponEffectsNode>(),
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

		public PlasmaWeaponEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
