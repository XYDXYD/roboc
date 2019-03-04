using Svelto.IoC;
using UnityEngine;

public sealed class SpawnPoints : MonoBehaviour, ISpawnPointsComponent
{
	public enum SpawnPointsType
	{
		Unknown = -1,
		Team0Start,
		Team1Start,
		PitModeStartLocations,
		TestModeEnemy,
		AIStartLocations,
		CampaignStartLocation
	}

	[SerializeField]
	private SpawningPoint[] spawningPoints;

	[SerializeField]
	private SpawnPointsType spawnPointType;

	SpawningPoint[] ISpawnPointsComponent.spawningPointList
	{
		get
		{
			return spawningPoints;
		}
	}

	SpawnPointsType ISpawnPointsComponent.spawningPointsType
	{
		get
		{
			return spawnPointType;
		}
	}

	[Inject]
	internal ISpawnPointManager spawnPointManager
	{
		private get;
		set;
	}

	public SpawnPoints()
		: this()
	{
	}

	private void Start()
	{
		AddSpawnPoints();
	}

	private void AddSpawnPoints()
	{
		if (spawnPointManager != null)
		{
			spawnPointManager.AddSpawningPointList(spawnPointType, spawningPoints);
		}
	}
}
