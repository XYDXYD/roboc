using Simulation.Hardware;

namespace Simulation
{
	internal sealed class MachineWeaponsBlockedImplementor : IMachineWeaponsBlockedComponent
	{
		public bool blocked
		{
			get;
			set;
		}

		public bool blockedByFusionShield
		{
			get;
			set;
		}

		public bool weaponNotGrounded
		{
			get;
			set;
		}

		public bool lastWeaponShotBlocked
		{
			get;
			set;
		}

		public MachineWeaponsBlockedImplementor()
		{
			blocked = false;
			blockedByFusionShield = false;
			weaponNotGrounded = false;
			lastWeaponShotBlocked = false;
		}
	}
}
