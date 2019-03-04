using Simulation.BattleTracker;
using Simulation.Hardware.Weapons.Plasma;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Mortar
{
	internal class MortarEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static MortarEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[24]
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
				new EntityViewBuilder<SmartWeaponFireNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<CrosshairWeaponNoFireStateTrackerNode>(),
				new EntityViewBuilder<BlockWeaponFireNode>(),
				new EntityViewBuilder<ShootAnimationNode>(),
				new EntityViewBuilder<WeaponTrackerNode>()
			};
		}

		public MortarEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
