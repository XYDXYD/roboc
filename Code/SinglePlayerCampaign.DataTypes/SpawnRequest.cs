namespace SinglePlayerCampaign.DataTypes
{
	internal struct SpawnRequest
	{
		public readonly string robotName;

		public readonly int spawnEventId;

		public readonly bool isBoss;

		public readonly bool isKillRequirement;

		public SpawnRequest(string robotName_, int spawnEventId_, bool isBoss_, bool isKillRequirement_)
		{
			robotName = robotName_;
			spawnEventId = spawnEventId_;
			isBoss = isBoss_;
			isKillRequirement = isKillRequirement_;
		}
	}
}
