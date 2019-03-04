namespace Simulation.SinglePlayer
{
	public interface IAIBotIdDataComponent
	{
		int playerId
		{
			get;
		}

		int teamId
		{
			get;
		}

		int machineId
		{
			get;
		}
	}
}
