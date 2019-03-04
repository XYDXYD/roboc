using Simulation.Hardware.Weapons;
using Simulation.SinglePlayerCampaign;
using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerServiceLayer;
using SinglePlayerServiceLayer.Requests.Photon;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;

namespace Simulation
{
	internal sealed class SinglePlayerCampaignSpawnPointManager : ISpawnPointManager, IInitialize
	{
		private int _currentWaveIndex;

		private bool _playerSpawnPointRemoved;

		private SpawningPoint[] _spawnPoints;

		private Random _random = new Random(DateTime.UtcNow.Millisecond);

		private FasterList<int> _availableIndices = new FasterList<int>();

		private CampaignWavesDifficultyData _campaignWavesDifficulty;

		[Inject]
		private PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		private ISinglePlayerRequestFactory serviceFactory
		{
			get;
			set;
		}

		[Inject]
		private CurrentWaveObserver currentWaveObserver
		{
			get;
			set;
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			currentWaveObserver.AddAction(new ObserverAction<WaveCounterInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void AddSpawningPointList(SpawnPoints.SpawnPointsType spawnPointType, SpawningPoint[] spawnPoints)
		{
			if (spawnPointType == SpawnPoints.SpawnPointsType.PitModeStartLocations)
			{
				_spawnPoints = spawnPoints;
				for (int i = 0; i < _spawnPoints.Length; i++)
				{
					_availableIndices.Add(i);
				}
			}
		}

		public SpawningPoint GetNextFreeSpawnPoint(SpawnPoints.SpawnPointsType spawnPointType, int playerId)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, playerId))
			{
				return GetPlayerSpawnPoint();
			}
			return GetEnemySpawnPoint();
		}

		private void StartNewWave(ref WaveCounterInfo waveCounterInfo)
		{
			int waveJustStartedIndex = waveCounterInfo.WaveJustStartedIndex;
			if (waveJustStartedIndex != 0)
			{
				_availableIndices.Clear();
				for (int i = 0; i < _spawnPoints.Length; i++)
				{
					_availableIndices.Add(i);
				}
				_playerSpawnPointRemoved = false;
				_currentWaveIndex = waveJustStartedIndex;
			}
		}

		private SpawningPoint GetPlayerSpawnPoint()
		{
			IGetCampaignWavesDataRequest getCampaignWavesDataRequest = serviceFactory.Create<IGetCampaignWavesDataRequest>();
			getCampaignWavesDataRequest.Inject(new GetCampaignWavesDependency(WorldSwitching.GetCampaignID(), WorldSwitching.GetCampaignDifficulty()));
			getCampaignWavesDataRequest.SetAnswer(new ServiceAnswer<CampaignWavesDifficultyData>(OnWavesDataReceived)).Execute();
			RemovePlayerSpawnPoint();
			WaveData waveData = _campaignWavesDifficulty.WavesData.get_Item(_currentWaveIndex);
			int playerSpawnLocation = waveData.PlayerSpawnLocation;
			return _spawnPoints[playerSpawnLocation];
		}

		private SpawningPoint GetEnemySpawnPoint()
		{
			if (_availableIndices.get_Count() == 0)
			{
				return _spawnPoints[0];
			}
			RemovePlayerSpawnPoint();
			int num = _random.Next(0, _availableIndices.get_Count());
			int num2 = _availableIndices.get_Item(num);
			_availableIndices.Remove(num2);
			return _spawnPoints[num2];
		}

		private void OnWavesDataReceived(CampaignWavesDifficultyData wavesDifficultyData)
		{
			_campaignWavesDifficulty = wavesDifficultyData;
		}

		private void RemovePlayerSpawnPoint()
		{
			if (!_playerSpawnPointRemoved && _campaignWavesDifficulty != null)
			{
				FasterList<WaveData> wavesData = _campaignWavesDifficulty.WavesData;
				_playerSpawnPointRemoved = true;
				WaveData waveData = wavesData.get_Item(_currentWaveIndex);
				int playerSpawnLocation = waveData.PlayerSpawnLocation;
				_availableIndices.Remove(playerSpawnLocation);
			}
		}
	}
}
