using RCNetwork.Events;
using RCNetwork.Server;
using Simulation.SinglePlayer.Rewards;
using Simulation.SinglePlayer.Spawn;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;

namespace Simulation.SinglePlayer.ServerMock
{
	internal class SinglePlayerTeamDeathMatchServerMock : MultiEntityViewsEngine<AIAgentDataComponentsNode, PlayerTargetNode>, IInitialize, IWaitForFrameworkDestruction, IPhysicallyTickable, ITickableBase
	{
		private readonly Dictionary<int, int> _teamScores = new Dictionary<int, int>();

		private bool _enableSuddenDeath;

		private GameModeSettings _teamDeathMatchGameModeSettings;

		private bool _gameStarted;

		private float _gameTimeSeconds;

		private bool _gameEnded;

		private readonly Dictionary<int, IAIBotIdDataComponent> _agentsLookupTable = new Dictionary<int, IAIBotIdDataComponent>();

		private IPlayerTargetGameObjectComponent _humanPlayerIdInfo;

		[Inject]
		public DestructionReporter destructionReporter
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
		public GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public TDMPracticeGamEndedObservable tdmPracticeGamEndedObservable
		{
			private get;
			set;
		}

		[Inject]
		public TeamDeathMatchAIScoreServerMock teamDeathMatchAIScoreServerMock
		{
			private get;
			set;
		}

		public SinglePlayerTeamDeathMatchServerMock(AllPlayersSpawnedObserver observer)
		{
			observer.AddAction((Action)StartTheGame);
		}

		protected override void Add(AIAgentDataComponentsNode obj)
		{
			_agentsLookupTable.Add(obj.aiBotIdData.playerId, obj.aiBotIdData);
			if (!_teamScores.ContainsKey(obj.aiBotIdData.teamId))
			{
				_teamScores[obj.aiBotIdData.teamId] = 0;
			}
		}

		protected override void Remove(AIAgentDataComponentsNode obj)
		{
			_agentsLookupTable.Remove(obj.aiBotIdData.playerId);
		}

		protected override void Add(PlayerTargetNode obj)
		{
			_humanPlayerIdInfo = obj.playerTargetGameObjectComponent;
			if (!_teamScores.ContainsKey(_humanPlayerIdInfo.teamId))
			{
				_teamScores[_humanPlayerIdInfo.teamId] = 0;
			}
		}

		protected override void Remove(PlayerTargetNode obj)
		{
			_humanPlayerIdInfo = null;
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnMachineKilled += HandleOnMachineKilled;
			destructionReporter.OnPlayerSelfDestructs += HandleOnPlayerSelfDestructs;
			IGetGameModeSettingsRequest getGameModeSettingsRequest = serviceRequestFactory.Create<IGetGameModeSettingsRequest>();
			getGameModeSettingsRequest.SetAnswer(new ServiceAnswer<GameModeSettingsDependency>(HandleGameSettingsLoaded));
			getGameModeSettingsRequest.Execute();
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineKilled -= HandleOnMachineKilled;
			destructionReporter.OnPlayerSelfDestructs -= HandleOnPlayerSelfDestructs;
		}

		public void PhysicsTick(float deltaSec)
		{
			if (_gameEnded || !_gameStarted)
			{
				return;
			}
			_gameTimeSeconds -= deltaSec;
			if (_gameTimeSeconds <= 0f)
			{
				int teamId = _humanPlayerIdInfo.teamId;
				int enemyTeamId;
				bool flag = FindHumanPlayerEnemyTeam(out enemyTeamId);
				if (_teamScores[teamId] > _teamScores[enemyTeamId])
				{
					TriggerGameEnded(teamId);
					SendHumanPlayerWonEvent(_humanPlayerIdInfo.playerId, teamId);
				}
				else if (_teamScores[teamId] < _teamScores[enemyTeamId])
				{
					TriggerGameEnded(enemyTeamId);
					SendHumanPlayerLostEvent(_humanPlayerIdInfo.playerId, enemyTeamId);
				}
				else
				{
					_enableSuddenDeath = true;
					networkEventManagerServer.BroadcastEventToAllPlayers(NetworkEvent.TeamDeathMatchState, new UpdateTeamDeathMatchDependency(_teamScores, _enableSuddenDeath));
				}
			}
		}

		private bool FindHumanPlayerEnemyTeam(out int enemyTeamId)
		{
			enemyTeamId = -1;
			foreach (int key in _teamScores.Keys)
			{
				if (key != _humanPlayerIdInfo.teamId)
				{
					enemyTeamId = key;
					return true;
				}
			}
			return false;
		}

		private bool FindEnemyTeam(int playerId, out int enemyTeamId)
		{
			int teamId = _humanPlayerIdInfo.teamId;
			if (playerId != _humanPlayerIdInfo.playerId && _agentsLookupTable.TryGetValue(playerId, out IAIBotIdDataComponent value))
			{
				teamId = value.teamId;
			}
			enemyTeamId = -1;
			using (Dictionary<int, int>.Enumerator enumerator = _teamScores.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Key != teamId)
					{
						enemyTeamId = enumerator.Current.Key;
						return true;
					}
				}
			}
			return false;
		}

		private void HandleGameSettingsLoaded(GameModeSettingsDependency gameModeSettingsDependency)
		{
			_teamDeathMatchGameModeSettings = new GameModeSettings(gameModeSettingsDependency.TeamDeathmatch);
		}

		private void HandleOnPlayerSelfDestructs(int playerId)
		{
			if (!_gameEnded && playerId == _humanPlayerIdInfo.playerId)
			{
				teamDeathMatchAIScoreServerMock.UpdateStats(_humanPlayerIdInfo.playerId, InGameStatId.RobotDestroyed, 1);
				int enemyTeamId;
				bool flag = FindHumanPlayerEnemyTeam(out enemyTeamId);
				IncrementTeamScoreAndCheckGameEnd(enemyTeamId);
			}
		}

		private void HandleOnMachineKilled(int owner, int shooterId)
		{
			if (!_gameEnded)
			{
				teamDeathMatchAIScoreServerMock.UpdateStats(owner, InGameStatId.RobotDestroyed, 1);
				teamDeathMatchAIScoreServerMock.UpdateStats(shooterId, InGameStatId.Kill, 1);
				int enemyTeamId;
				bool flag = FindEnemyTeam(owner, out enemyTeamId);
				IncrementTeamScoreAndCheckGameEnd(enemyTeamId);
				networkEventManagerServer.SendEventToPlayer(NetworkEvent.ConfirmedKill, shooterId, new KillDependency(owner, shooterId));
			}
		}

		private void IncrementTeamScoreAndCheckGameEnd(int enemyTeam)
		{
			Dictionary<int, int> teamScores;
			int key;
			(teamScores = _teamScores)[key = enemyTeam] = teamScores[key] + 1;
			networkEventManagerServer.BroadcastEventToAllPlayers(NetworkEvent.TeamDeathMatchState, new UpdateTeamDeathMatchDependency(_teamScores, _enableSuddenDeath));
			if (_teamScores[enemyTeam] == _teamDeathMatchGameModeSettings.killLimit || _enableSuddenDeath)
			{
				if (_humanPlayerIdInfo.teamId == enemyTeam)
				{
					SendHumanPlayerWonEvent(_humanPlayerIdInfo.playerId, enemyTeam);
				}
				else
				{
					SendHumanPlayerLostEvent(_humanPlayerIdInfo.playerId, enemyTeam);
				}
				TriggerGameEnded(enemyTeam);
			}
		}

		private void SendHumanPlayerWonEvent(int humanPlayerId, int winningTeamId)
		{
			networkEventManagerServer.SendEventToPlayer(NetworkEvent.GameWon, humanPlayerId, new GameWonDependency(new List<int>
			{
				humanPlayerId
			}, winningTeamId, GameEndReason.OneTeamRemaining));
		}

		private void SendHumanPlayerLostEvent(int humanPlayerId, int winningTeamId)
		{
			networkEventManagerServer.SendEventToPlayer(NetworkEvent.GameLost, humanPlayerId, new GameLostDependency(new List<int>
			{
				humanPlayerId
			}, winningTeamId, GameEndReason.OneTeamRemaining));
		}

		private void StartTheGame()
		{
			foreach (int key in _teamScores.Keys)
			{
				_teamScores[key] = 0;
			}
			_gameStarted = true;
			gameStartDispatcher.OnGameStart();
			_gameTimeSeconds = _teamDeathMatchGameModeSettings.gameTimeMinutes * 60;
			UpdateTeamDeathmatchSettingsDependency dependency = new UpdateTeamDeathmatchSettingsDependency(_teamDeathMatchGameModeSettings);
			networkEventManagerServer.BroadcastEventToAllPlayers(NetworkEvent.GameModeSettings, dependency);
			networkEventManagerServer.BroadcastEventToAllPlayers(NetworkEvent.CurrentGameTime, new GameTimeDependency(_gameTimeSeconds));
		}

		private void TriggerGameEnded(int winningTeamId)
		{
			_gameEnded = true;
			tdmPracticeGamEndedObservable.Dispatch(ref winningTeamId);
		}
	}
}
