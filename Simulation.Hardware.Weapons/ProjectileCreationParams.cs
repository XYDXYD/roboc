namespace Simulation.Hardware.Weapons
{
	internal struct ProjectileCreationParams
	{
		public readonly int weaponId;

		public readonly bool isLocal;

		public ProjectileCreationParams(int weaponId, bool isLocal)
		{
			this = default(ProjectileCreationParams);
			this.weaponId = weaponId;
			this.isLocal = isLocal;
		}
	}
}
