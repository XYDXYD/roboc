using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class WeaponDamageBuffNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IProjectileDamageStatsComponent damageStats;

		public WeaponDamageBuffNode()
			: this()
		{
		}
	}
}
