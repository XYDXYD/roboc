using Svelto.ECS;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal sealed class LoadRocketLauncherStatsNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IHomingProjectileStatsComponent homingProjectileStats;

		public ILockOnTargetingParametersComponent lockOnComponent;

		public LoadRocketLauncherStatsNode()
			: this()
		{
		}
	}
}
