namespace Simulation
{
	public interface IEmpLocatorOwnerComponent
	{
		int ownerId
		{
			get;
		}

		int ownerMachineId
		{
			get;
		}

		bool isOnMyTeam
		{
			get;
		}
	}
}
