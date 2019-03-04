using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class DamageMultiplierNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IProjectileDamageStatsComponent damageStats;

		public DamageMultiplierNode()
			: this()
		{
		}
	}
}
