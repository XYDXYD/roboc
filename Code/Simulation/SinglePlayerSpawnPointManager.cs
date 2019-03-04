namespace Simulation
{
	internal sealed class SinglePlayerSpawnPointManager : ISpawnPointManager
	{
		private SpawningPoint[][] _spawnPoints = new SpawningPoint[2][];

		private int[] _currentSpawnPoint = new int[2];

		public void AddSpawningPointList(SpawnPoints.SpawnPointsType spawnType, SpawningPoint[] inputSpawnPoints)
		{
			if (spawnType == SpawnPoints.SpawnPointsType.Team0Start)
			{
				_spawnPoints[0] = inputSpawnPoints;
			}
			if (spawnType == SpawnPoints.SpawnPointsType.Team1Start)
			{
				_spawnPoints[1] = inputSpawnPoints;
			}
		}

		public SpawningPoint GetNextFreeSpawnPoint(SpawnPoints.SpawnPointsType spawnPointType, int playerId)
		{
			int num = (spawnPointType != 0) ? 1 : 0;
			SpawningPoint result = _spawnPoints[num][_currentSpawnPoint[num]];
			_currentSpawnPoint[num] = (_currentSpawnPoint[num] + 1) % _spawnPoints[num].Length;
			return result;
		}
	}
}
