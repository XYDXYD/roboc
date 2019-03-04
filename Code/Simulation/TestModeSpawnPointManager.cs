namespace Simulation
{
	internal sealed class TestModeSpawnPointManager : ISpawnPointManager
	{
		private SpawningPoint[] _spawnPoints;

		private int _currentSpawnPoint;

		public void AddSpawningPointList(SpawnPoints.SpawnPointsType spawnType, SpawningPoint[] inputSpawnPoints)
		{
			if (spawnType == SpawnPoints.SpawnPointsType.TestModeEnemy)
			{
				_spawnPoints = inputSpawnPoints;
			}
		}

		public SpawningPoint GetNextFreeSpawnPoint(SpawnPoints.SpawnPointsType spawnPointType, int playerId)
		{
			SpawningPoint result = _spawnPoints[_currentSpawnPoint];
			_currentSpawnPoint = (_currentSpawnPoint + 1) % _spawnPoints.Length;
			return result;
		}
	}
}
