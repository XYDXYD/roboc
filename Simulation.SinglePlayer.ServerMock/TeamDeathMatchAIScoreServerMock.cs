using GameServerServiceLayer;
using RCNetwork.Events;
using RCNetwork.Server;
using Simulation.SinglePlayer.Rewards;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Simulation.SinglePlayer.ServerMock
{
	internal class TeamDeathMatchAIScoreServerMock : IInitialize, IWaitForFrameworkDestruction
	{
		private class PlayerScoreData
		{
			public float baseScoreRatio;

			public float bonusScoreRatio;

			public bool gameWon;
		}

		private bool _gameEnded;

		private ScoreMultipliers _scoreMultipliers;

		private UpdateGameStatsDependency _updateGameStatsDependency = new UpdateGameStatsDependency(0, InGameStatId.None, 0u, 0u, 0u);

		private Dictionary<int, Dictionary<InGameStatId, int>> _inGameStats = new Dictionary<int, Dictionary<InGameStatId, int>>();

		private Dictionary<int, PlayerScoreData> _scoreData = new Dictionary<int, PlayerScoreData>();

		private StatsUpdatedEvent _statsUpdatedEvent;

		private TDMPracticeGamEndedObserver _tdmPracticeGamEndedObserver;

		private Dictionary<int, FasterList<int>> _teams = new Dictionary<int, FasterList<int>>();

		[Inject]
		public MachineSpawnDispatcher machineSpawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public INetworkEventManagerServer networkEventManagerServer
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public StatsUpdatedObservable statsUpdatedObservable
		{
			private get;
			set;
		}

		[Inject]
		public FinalStatsUpdatedObservable finalStatsUpdatedObservable
		{
			private get;
			set;
		}

		[Inject]
		public TDMPracticeGamEndedObservable tdmGameEndedObservable
		{
			private get;
			set;
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			_tdmPracticeGamEndedObserver = new TDMPracticeGamEndedObserver(tdmGameEndedObservable);
			_tdmPracticeGamEndedObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			machineSpawnDispatcher.OnPlayerSpawnedIn += HandleOnPlayerSpawned;
			IGetScoreMultipliersTeamDeathMatchAIRequest getScoreMultipliersTeamDeathMatchAIRequest = serviceRequestFactory.Create<IGetScoreMultipliersTeamDeathMatchAIRequest>();
			getScoreMultipliersTeamDeathMatchAIRequest.SetAnswer(new ServiceAnswer<ScoreMultipliers>(HandleScoreMultipliersDataLoaded));
			getScoreMultipliersTeamDeathMatchAIRequest.Execute();
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineSpawnDispatcher.OnPlayerSpawnedIn -= HandleOnPlayerSpawned;
			_tdmPracticeGamEndedObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void UpdateStats(int playerId, InGameStatId gameStatId, int deltaAmount)
		{
			if (!_gameEnded && _inGameStats.ContainsKey(playerId))
			{
				int num = _inGameStats[playerId][gameStatId];
				num += deltaAmount;
				_inGameStats[playerId][gameStatId] = num;
				float deltaBaseRatio = 0f;
				float deltaBonousRatio = 0f;
				ComputeScoreRatio(playerId, gameStatId, (uint)deltaAmount, out deltaBaseRatio, out deltaBonousRatio);
				_scoreData[playerId].baseScoreRatio += deltaBaseRatio;
				_scoreData[playerId].bonusScoreRatio += deltaBonousRatio;
				int num2 = _inGameStats[playerId][InGameStatId.Score];
				PlayerScoreData playerScoreData = _scoreData[playerId];
				uint num3 = ComputePlayerScore(playerScoreData.baseScoreRatio, playerScoreData.bonusScoreRatio, playerScoreData.gameWon);
				uint deltaScore = (uint)((int)num3 - num2);
				_inGameStats[playerId][InGameStatId.Score] = (int)num3;
				_updateGameStatsDependency.SetFields(playerId, gameStatId, (uint)num, num3, deltaScore);
				networkEventManagerServer.BroadcastEventToAllPlayers(NetworkEvent.UpdateGameStats, _updateGameStatsDependency);
				_statsUpdatedEvent.deltaAmount = deltaAmount;
				_statsUpdatedEvent.gameStatId = gameStatId;
				_statsUpdatedEvent.playerId = playerId;
				statsUpdatedObservable.Dispatch(ref _statsUpdatedEvent);
				_statsUpdatedEvent.deltaAmount = (int)num3 - num2;
				_statsUpdatedEvent.gameStatId = InGameStatId.Score;
				_statsUpdatedEvent.playerId = playerId;
				statsUpdatedObservable.Dispatch(ref _statsUpdatedEvent);
			}
		}

		public void ClearEnemyStats(int playerId)
		{
			Dictionary<InGameStatId, int> value = _inGameStats[playerId];
			PlayerScoreData value2 = _scoreData[playerId];
			FasterList<int> value3 = _teams[0];
			_inGameStats.Clear();
			_scoreData.Clear();
			_teams.Clear();
			_inGameStats.Add(playerId, value);
			_scoreData.Add(playerId, value2);
			_teams.Add(0, value3);
		}

		private unsafe void HandleOnGameWon(ref int winningTeamId)
		{
			_gameEnded = true;
			_tdmPracticeGamEndedObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			machineSpawnDispatcher.OnPlayerSpawnedIn -= HandleOnPlayerSpawned;
			FasterList<int> val = _teams[winningTeamId];
			for (int i = 0; i < val.get_Count(); i++)
			{
				int num = val.get_Item(i);
				int num2 = _inGameStats[num][InGameStatId.Score];
				PlayerScoreData playerScoreData = _scoreData[num];
				playerScoreData.gameWon = true;
				uint num3 = ComputePlayerScore(playerScoreData.baseScoreRatio, playerScoreData.bonusScoreRatio, playerScoreData.gameWon);
				_inGameStats[num][InGameStatId.Score] = (int)num3 - num2;
				uint deltaScore = (uint)((int)num3 - num2);
				_updateGameStatsDependency.SetFields(num, InGameStatId.Score, (uint)_inGameStats[num][InGameStatId.Score], num3, deltaScore);
				networkEventManagerServer.BroadcastEventToAllPlayers(NetworkEvent.UpdateGameStats, _updateGameStatsDependency);
				_statsUpdatedEvent.deltaAmount = num2;
				_statsUpdatedEvent.gameStatId = InGameStatId.Score;
				_statsUpdatedEvent.playerId = num;
				statsUpdatedObservable.Dispatch(ref _statsUpdatedEvent);
			}
			finalStatsUpdatedObservable.Dispatch(ref _statsUpdatedEvent);
		}

		private uint ComputePlayerScore(float baseRatio, float bonusRatio, bool gameWon)
		{
			uint victoryScore = _scoreMultipliers.victoryScore;
			uint defeatScore = _scoreMultipliers.defeatScore;
			float deltaScalar = _scoreMultipliers.deltaScalar;
			uint num = defeatScore;
			if (gameWon)
			{
				num = victoryScore;
			}
			baseRatio = (float)Math.Min(1.0, baseRatio);
			double val = baseRatio + bonusRatio * deltaScalar;
			val = Math.Min(val, _scoreMultipliers.maxScoreRatio);
			return (uint)Math.Ceiling(val * (double)num);
		}

		private void ComputeScoreRatio(int playerId, InGameStatId gameStatsId, uint delta, out float deltaBaseRatio, out float deltaBonousRatio)
		{
			float num = 0f;
			float num2 = 0f;
			float value = 0f;
			if (_scoreMultipliers.baseMultipliers.TryGetValue(gameStatsId, out value))
			{
				num = (float)(double)delta * value;
			}
			if (_scoreMultipliers.bonusMultipliers.TryGetValue(gameStatsId, out value))
			{
				num2 = (float)(double)delta * value;
			}
			deltaBaseRatio = num;
			deltaBonousRatio = num2;
		}

		private void HandleScoreMultipliersDataLoaded(ScoreMultipliers scoreMultipliers)
		{
			_scoreMultipliers = scoreMultipliers;
		}

		private void HandleOnPlayerSpawned(SpawnInParametersPlayer spawnInParameters)
		{
			Dictionary<InGameStatId, int> dictionary = new Dictionary<InGameStatId, int>();
			_inGameStats[spawnInParameters.playerId] = dictionary;
			InGameStatId[] array = (InGameStatId[])Enum.GetValues(typeof(InGameStatId));
			for (int i = 0; i < array.Length; i++)
			{
				dictionary[array[i]] = 0;
			}
			_scoreData[spawnInParameters.playerId] = new PlayerScoreData();
			if (!_teams.TryGetValue(spawnInParameters.teamId, out FasterList<int> value))
			{
				value = new FasterList<int>();
				_teams[spawnInParameters.teamId] = value;
			}
			value.Add(spawnInParameters.playerId);
		}
	}
}
