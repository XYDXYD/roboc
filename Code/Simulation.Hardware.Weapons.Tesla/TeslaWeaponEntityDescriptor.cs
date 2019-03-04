using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal class TeslaWeaponEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static TeslaWeaponEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[15]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<TeslaRamNode>(),
				new EntityViewBuilder<TeslaRamCollisionNode>(),
				new EntityViewBuilder<TeslaRamEffectsNode>(),
				new EntityViewBuilder<WeaponDamageBuffNode>(),
				new EntityViewBuilder<DamageMultiplierNode>(),
				new EntityViewBuilder<WeaponFireTimingNode>(),
				new EntityViewBuilder<WeaponSwitchNode>(),
				new EntityViewBuilder<NextWeaponToFireNode>(),
				new EntityViewBuilder<LoadTeslaWeaponStatsNode>(),
				new EntityViewBuilder<WeaponWithoutLaserPointerNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>(),
				new EntityViewBuilder<CrosshairWeaponNoFireStateTrackerNode>(),
				new EntityViewBuilder<BlockWeaponCollisionNode>(),
				new EntityViewBuilder<WeaponTrackerNode>()
			};
		}

		public TeslaWeaponEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
