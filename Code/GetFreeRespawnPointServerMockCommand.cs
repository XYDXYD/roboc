using RCNetwork.Events;

internal sealed class GetFreeRespawnPointServerMockCommand : GetFreeSpawnPointServerMockCommand
{
	protected override void SendEventToPlayer(int senderId, SpawnPointDependency spawnPointDependency)
	{
		base.networkEventManagerServer.SendEventToPlayer(NetworkEvent.FreeRespawnPoint, senderId, spawnPointDependency);
	}
}
