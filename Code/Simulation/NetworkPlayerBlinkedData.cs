namespace Simulation
{
	public struct NetworkPlayerBlinkedData
	{
		public bool teleportStarted;

		public int playerId;

		public NetworkPlayerBlinkedData(bool teleportStarted_, int playerId_)
		{
			teleportStarted = teleportStarted_;
			playerId = playerId_;
		}

		public void SetValues(bool teleportStarted_, int playerId_)
		{
			teleportStarted = teleportStarted_;
			playerId = playerId_;
		}
	}
}
