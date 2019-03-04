namespace Simulation.Hardware
{
	internal class MachineRaycastImplementor : IWeaponRaycastComponent
	{
		public WeaponRaycast weaponRaycast
		{
			get;
			private set;
		}

		public MachineRaycastImplementor(WeaponRaycast weaponRaycast)
		{
			this.weaponRaycast = weaponRaycast;
		}
	}
}
