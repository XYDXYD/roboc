namespace Simulation.SinglePlayerCampaign.DataTypes
{
	public struct WaveRobot
	{
		public readonly string robotName;

		public readonly string robotWeapon;

		public readonly string robotMovementPart;

		public readonly string robotRank;

		public readonly byte[] serializedRobotData;

		public readonly byte[] serializedRobotDataColour;

		public readonly int timeToSpawn;

		public readonly int killsToSpawn;

		public readonly int timeToDespawn;

		public readonly int killsToDespawn;

		public readonly int initialRobotAmount;

		public readonly int periodicRobotAmount;

		public readonly int spawnInterval;

		public readonly int minRobotAmount;

		public readonly int maxRobotAmount;

		public readonly bool isBoss;

		public readonly bool isKillRequirement;

		public WaveRobot(string robotName_, string robotWeapon_, string robotMovementPart_, string robotRank_, int initialRobotAmount_)
		{
			robotName = robotName_;
			robotWeapon = robotWeapon_;
			robotMovementPart = robotMovementPart_;
			robotRank = robotRank_;
			serializedRobotData = null;
			serializedRobotDataColour = null;
			timeToSpawn = 0;
			killsToSpawn = 0;
			timeToDespawn = 0;
			killsToDespawn = 0;
			initialRobotAmount = initialRobotAmount_;
			periodicRobotAmount = 0;
			spawnInterval = 0;
			minRobotAmount = 0;
			maxRobotAmount = 0;
			isBoss = false;
			isKillRequirement = true;
		}

		public WaveRobot(string robotName_, byte[] serializedRobotData_, byte[] serializedRobotDataColour_, int timeToSpawn_, int killsToSpawn_, int timeToDespawn_, int killsToDespawn_, int initialRobotAmount_, int periodicRobotAmount_, int spawnInterval_, int minRobotAmount_, int maxRobotAmount_, bool isBoss_, bool isKillRequirement_)
		{
			robotName = robotName_;
			robotWeapon = string.Empty;
			robotMovementPart = string.Empty;
			robotRank = string.Empty;
			serializedRobotData = serializedRobotData_;
			serializedRobotDataColour = serializedRobotDataColour_;
			timeToSpawn = timeToSpawn_;
			killsToSpawn = killsToSpawn_;
			timeToDespawn = timeToDespawn_;
			killsToDespawn = killsToDespawn_;
			initialRobotAmount = initialRobotAmount_;
			periodicRobotAmount = periodicRobotAmount_;
			spawnInterval = spawnInterval_;
			minRobotAmount = minRobotAmount_;
			maxRobotAmount = maxRobotAmount_;
			isBoss = isBoss_;
			isKillRequirement = isKillRequirement_;
		}
	}
}
