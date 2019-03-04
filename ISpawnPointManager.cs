internal interface ISpawnPointManager
{
	void AddSpawningPointList(SpawnPoints.SpawnPointsType spawnPointType, SpawningPoint[] spawnPoints);

	SpawningPoint GetNextFreeSpawnPoint(SpawnPoints.SpawnPointsType spawnPointType, int playerId);
}
