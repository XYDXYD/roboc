namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileOwnerComponent
	{
		int ownerId
		{
			get;
			set;
		}

		int machineId
		{
			get;
			set;
		}

		int ownerTeam
		{
			get;
			set;
		}

		Byte3 weaponGridPos
		{
			get;
			set;
		}

		bool isEnemy
		{
			get;
			set;
		}

		bool isAi
		{
			get;
			set;
		}

		bool ownedByMe
		{
			get;
			set;
		}
	}
}
