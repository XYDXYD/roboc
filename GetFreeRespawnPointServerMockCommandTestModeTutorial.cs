using RCNetwork.Events;

internal sealed class GetFreeRespawnPointServerMockCommandTestModeTutorial : GetFreeSpawnPointServerMockCommand
{
	public override SpawnPoints.SpawnPointsType SpawnType => SpawnPoints.SpawnPointsType.TestModeEnemy;

	public override bool UseOnlyTeamBasedSpawnPoints => false;

	protected override void SendEventToPlayer(int senderId, SpawnPointDependency spawnPointDependency)
	{
		base.networkEventManagerServer.SendEventToPlayer(NetworkEvent.FreeRespawnPoint, senderId, spawnPointDependency);
	}
}
