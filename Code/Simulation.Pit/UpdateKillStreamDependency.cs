namespace Simulation.Pit
{
	public class UpdateKillStreamDependency
	{
		public int playerId;

		public uint streak;

		public UpdateKillStreamDependency(int id, uint strk)
		{
			playerId = id;
			streak = strk;
		}
	}
}
