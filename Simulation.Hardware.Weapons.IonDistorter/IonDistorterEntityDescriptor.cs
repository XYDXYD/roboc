using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal class IonDistorterEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static IonDistorterEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[23]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<LoadWeaponStatsNode>(),
				new EntityViewBuilder<WeaponAimNode>(),
				new EntityViewBuilder<WeaponSwitchNode>(),
				new EntityViewBuilder<IonDistorterShootingNode>(),
				new EntityViewBuilder<WeaponFireTimingNode>(),
				new EntityViewBuilder<NextWeaponToFireNode>(),
				new EntityViewBuilder<IonDistorterEffectsNode>(),
				new EntityViewBuilder<DamageMultiplierNode>(),
				new EntityViewBuilder<ZoomNode>(),
				new EntityViewBuilder<WeaponWithoutLaserPointerNode>(),
				new EntityViewBuilder<ShootingAfterEffectsNode>(),
				new EntityViewBuilder<RobotShakeShootingEntityView>(),
				new EntityViewBuilder<CameraShakeShootingEntityView>(),
				new EntityViewBuilder<CrosshairWeaponTrackerNode>(),
				new EntityViewBuilder<WeaponAccuracyNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<CrosshairWeaponNoFireStateTrackerNode>(),
				new EntityViewBuilder<SmartWeaponFireNode>(),
				new EntityViewBuilder<BlockWeaponFireNode>(),
				new EntityViewBuilder<WeaponDamageBuffNode>(),
				new EntityViewBuilder<WeaponTrackerNode>(),
				new EntityViewBuilder<ShootAnimationNode>()
			};
		}

		public IonDistorterEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
