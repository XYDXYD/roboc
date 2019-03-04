namespace Simulation.SinglePlayer.Rewards
{
	internal struct StatsUpdatedEvent
	{
		public int playerId;

		public InGameStatId gameStatId;

		public int deltaAmount;
	}
}
