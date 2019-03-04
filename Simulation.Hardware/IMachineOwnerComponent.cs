namespace Simulation.Hardware
{
	public interface IMachineOwnerComponent
	{
		bool ownedByMe
		{
			get;
			set;
		}

		bool ownedByAi
		{
			get;
			set;
		}

		int ownerId
		{
			get;
			set;
		}

		int ownerMachineId
		{
			get;
			set;
		}
	}
}
