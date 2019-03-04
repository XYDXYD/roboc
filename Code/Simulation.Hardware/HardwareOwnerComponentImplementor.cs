namespace Simulation.Hardware
{
	internal class HardwareOwnerComponentImplementor : IHardwareOwnerComponent
	{
		public int ownerId
		{
			get;
			private set;
		}

		public int ownerTeam
		{
			get;
			private set;
		}

		public int machineId
		{
			get;
			private set;
		}

		public bool ownedByMe
		{
			get;
			private set;
		}

		public bool isEnemy
		{
			get;
			private set;
		}

		public bool ownedByAi
		{
			get;
			private set;
		}

		internal HardwareOwnerComponentImplementor(int newOwnerId, int newOwnerTeam, int newMachineId, bool isOwnedByMe, bool enemy, bool isOwnedByAi)
		{
			ownerId = newOwnerId;
			ownerTeam = newOwnerTeam;
			machineId = newMachineId;
			ownedByMe = isOwnedByMe;
			isEnemy = enemy;
			ownedByAi = isOwnedByAi;
		}
	}
}
