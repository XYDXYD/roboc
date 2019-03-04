namespace Simulation
{
	internal interface IDiscShieldOwnerComponent
	{
		int discShieldOwner
		{
			get;
		}

		bool isMine
		{
			get;
		}

		bool isOnMyTeam
		{
			get;
		}
	}
}
