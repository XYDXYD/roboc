using Simulation.BattleTracker;
using Simulation.Hardware.Weapons.Laser;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal class ChaingunEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static ChaingunEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[27]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<LoadWeaponStatsNode>(),
				new EntityViewBuilder<ChaingunSpinNode>(),
				new EntityViewBuilder<ChaingunSpinEffectNode>(),
				new EntityViewBuilder<ShellParticlesNode>(),
				new EntityViewBuilder<ChaingunSpinAudioNode>(),
				new EntityViewBuilder<LaserWeaponShootingNode>(),
				new EntityViewBuilder<LaserWeaponEffectsNode>(),
				new EntityViewBuilder<WeaponAimNode>(),
				new EntityViewBuilder<WeaponSwitchNode>(),
				new EntityViewBuilder<WeaponFireTimingNode>(),
				new EntityViewBuilder<WeaponDamageBuffNode>(),
				new EntityViewBuilder<DamageMultiplierNode>(),
				new EntityViewBuilder<ZoomNode>(),
				new EntityViewBuilder<NextWeaponToFireNode>(),
				new EntityViewBuilder<WeaponWithoutLaserPointerNode>(),
				new EntityViewBuilder<ShootingAfterEffectsNode>(),
				new EntityViewBuilder<RobotShakeShootingEntityView>(),
				new EntityViewBuilder<CameraShakeShootingEntityView>(),
				new EntityViewBuilder<WeaponAccuracyNode>(),
				new EntityViewBuilder<CrosshairWeaponTrackerNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<CrosshairWeaponNoFireStateTrackerNode>(),
				new EntityViewBuilder<SmartWeaponFireNode>(),
				new EntityViewBuilder<SharedSpinDataNode>(),
				new EntityViewBuilder<BlockWeaponFireNode>(),
				new EntityViewBuilder<WeaponTrackerNode>()
			};
		}

		public ChaingunEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
