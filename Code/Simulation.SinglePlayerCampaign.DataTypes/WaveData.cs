namespace Simulation.SinglePlayerCampaign.DataTypes
{
	internal struct WaveData
	{
		public readonly WaveRobot[] WaveRobots;

		public readonly int PlayerSpawnLocation;

		public readonly int KillTargetAmount;

		public readonly int MinimumTime;

		public readonly int MaximumTime;

		public WaveData(WaveRobot[] waveRobots)
		{
			PlayerSpawnLocation = -1;
			WaveRobots = waveRobots;
			KillTargetAmount = 0;
			MinimumTime = 0;
			MaximumTime = 0;
		}

		public WaveData(int playerSpawnLocation_, WaveRobot[] waveRobots_, int killTarget_, int timeMin_, int timeMax_)
		{
			PlayerSpawnLocation = playerSpawnLocation_;
			WaveRobots = waveRobots_;
			KillTargetAmount = killTarget_;
			MinimumTime = timeMin_;
			MaximumTime = timeMax_;
		}
	}
}
