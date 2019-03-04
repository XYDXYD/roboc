namespace Simulation.Hardware
{
	internal interface IHardwareOwnerComponent
	{
		int ownerId
		{
			get;
		}

		int ownerTeam
		{
			get;
		}

		int machineId
		{
			get;
		}

		bool ownedByMe
		{
			get;
		}

		bool isEnemy
		{
			get;
		}

		bool ownedByAi
		{
			get;
		}
	}
}
