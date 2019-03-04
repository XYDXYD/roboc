namespace Simulation.SinglePlayer
{
	internal interface IAIWaypointListComponent
	{
		PathNode[] nodes
		{
			get;
		}

		int teamId
		{
			get;
		}
	}
}
