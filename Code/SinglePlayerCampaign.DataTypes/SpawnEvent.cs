using Svelto.ECS;

namespace SinglePlayerCampaign.DataTypes
{
	internal class SpawnEvent
	{
		public int robotsSpawned;

		public float timeOfNextSpawn;

		public bool initialRobotsSpawned;

		public DispatchOnSet<int> robotsKilled;

		public bool alreadyDespawned;
	}
}
