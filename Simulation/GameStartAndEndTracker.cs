using Authentication;
using Battle;
using LobbyServiceLayer;
using Services.Analytics;
using Services.Requests.Interfaces;
using Services.Web.Photon;
using Simulation.Analytics;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Simulation
{
	internal class GameStartAndEndTracker : IInitialize, IWaitForFrameworkDestruction
	{
		private bool _quitRequestSent;

		private bool _quitTriggered;

		private DateTime _battleStartDateUTC = DateTime.MinValue;

		private int _localPlayerId = -1;

		private string _localPlayerName = string.Empty;

		private int _totalDamageReceived;

		private bool _gameWon;

		private string _levelName;

		private string _gameModeType;

		private uint _robotTier;

		private PlayerRobotStats _localPlayerMachineStats;

		[Inject]
		private GameStartDispatcher gameStartDispatcher
		{
			get;
			set;
		}

		[Inject]
		private IServiceRequestFactory serviceRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private GameEndedObserver gameEndedObserver
		{
			get;
			set;
		}

		[Inject]
		private GameStateClient gameStateClient
		{
			get;
			set;
		}

		[Inject]
		private WorldSwitching worldSwitching
		{
			get;
			set;
		}

		[Inject]
		private BattlePlayers battlePlayers
		{
			get;
			set;
		}

		[Inject]
		private PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		private PlayerNamesContainer playerNamesContainer
		{
			get;
			set;
		}

		[Inject]
		private BattleStatsPresenter battleStatsPresenter
		{
			get;
			set;
		}

		[Inject]
		private NetworkMachineManager machineManager
		{
			get;
			set;
		}

		[Inject]
		private PlayerMachinesContainer playerMachinesContainer
		{
			get;
			set;
		}

		[Inject]
		private HealthTracker healthTracker
		{
			get;
			set;
		}

		[Inject]
		private QuitListenerManager quitListenerManager
		{
			get;
			set;
		}

		[Inject]
		private ILobbyRequestFactory lobbyRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private MachineSpawnDispatcher spawnDispatcher
		{
			get;
			set;
		}

		[Inject]
		private WorldSwitchingSimulationAnalytics worldSwitchingAnalytics
		{
			get;
			set;
		}

		[Inject]
		private ITutorialController tutorialController
		{
			get;
			set;
		}

		[Inject]
		private IAnalyticsRequestFactory analyticsRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingPresenter
		{
			get;
			set;
		}

		[Inject]
		private MachinePreloader machinePreloader
		{
			get;
			set;
		}

		public void OnDependenciesInjected()
		{
			gameStartDispatcher.Register(SendGameStartRequest);
			gameEndedObserver.OnGameEnded += SendGameEndedRequest;
			worldSwitching.OnWorldIsSwitching.Add(CheckSendGameEndedRequest());
			spawnDispatcher.OnPlayerRegistered += CalculateLocalMachineStats;
			healthTracker.OnPlayerHealthChanged += AddToPlayerDamage;
			quitListenerManager.AddOnQuitTriggered(SendPlayerQuitRequest);
			SaveLevelAndMode();
		}

		public void OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(SendGameStartRequest);
			gameEndedObserver.OnGameEnded -= SendGameEndedRequest;
			spawnDispatcher.OnPlayerRegistered -= CalculateLocalMachineStats;
			healthTracker.OnPlayerHealthChanged -= AddToPlayerDamage;
			quitListenerManager.RemoveOnQuitTriggered(SendPlayerQuitRequest);
		}

		private void SendGameStartRequest()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SendGameStartRequestTask);
		}

		private void SendGameEndedRequest(bool won)
		{
			_gameWon = won;
		}

		private void CalculateLocalMachineStats(SpawnInParametersPlayer data)
		{
			if (data.isMe)
			{
				_localPlayerId = data.playerId;
				_localPlayerName = User.Username;
				Dictionary<string, PlayerDataDependency> expectedPlayersDict = battlePlayers.GetExpectedPlayersDict();
				PlayerDataDependency playerDataDependency = expectedPlayersDict[_localPlayerName];
				ILoadMovementStatsRequest service = serviceRequestFactory.Create<ILoadMovementStatsRequest>();
				TaskService<MovementStats> taskService = new TaskService<MovementStats>(service);
				taskService.Execute();
				int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, _localPlayerId);
				IMachineMap machineMap = machineManager.GetMachineMap(TargetType.Player, activeMachine);
				IGetDamageBoostRequest service2 = serviceRequestFactory.Create<IGetDamageBoostRequest>();
				TaskService<DamageBoostDeserialisedData> taskService2 = new TaskService<DamageBoostDeserialisedData>(service2);
				taskService2.Execute();
				ILoadPlatformConfigurationRequest request = serviceRequestFactory.Create<ILoadPlatformConfigurationRequest>();
				TaskService<PlatformConfigurationSettings> taskService3 = request.AsTask();
				taskService3.Execute();
				TLOG_RobotStatsCalculator_Tencent tLOG_RobotStatsCalculator_Tencent = new TLOG_RobotStatsCalculator_Tencent(taskService.result, taskService2.result, taskService3.result.UseDecimalSystem);
				_localPlayerMachineStats = tLOG_RobotStatsCalculator_Tencent.CalculateRobotStats(data.preloadedMachine.machineMap, playerDataDependency.RobotUniqueId, playerDataDependency.MasteryLevel);
				_localPlayerMachineStats.robotName = playerDataDependency.RobotName;
			}
		}

		private IEnumerator CheckSendGameEndedRequest()
		{
			yield return SendGameEndedRequestTask(_gameWon, gameStateClient.battleEndResult);
		}

		private IEnumerator SendGameStartRequestTask()
		{
			loadingPresenter.NotifyLoading("HandleAnalytics");
			_battleStartDateUTC = DateTime.UtcNow;
			int myTeamID = playerTeamsContainer.GetMyTeam();
			Dictionary<string, PlayerDataDependency> players = battlePlayers.GetExpectedPlayersDict();
			playerTeamsContainer.GetPlayersOnTeam(TargetType.Player, myTeamID);
			while (_localPlayerMachineStats == null)
			{
				yield return null;
			}
			TaskService<BattleParametersData> battleParametersRequest = lobbyRequestFactory.Create<IGetBattleParametersRequest>().AsTask();
			yield return battleParametersRequest;
			if (!battleParametersRequest.succeeded)
			{
				Console.LogError("Battle parameters request failed. " + battleParametersRequest.behaviour.exceptionThrown);
				loadingPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			BattleParametersData result = battleParametersRequest.result;
			LogPlayerStartedGameDependency logPlayerStartedGameDependency = new LogPlayerStartedGameDependency(myTeamID, _localPlayerMachineStats, gameStartDispatcher.isReconnecting);
			LogPlayerStartedGameDependency logPlayerStartedGameDependency2 = logPlayerStartedGameDependency;
			GameModeKey gameModeKey = result.GameModeKey;
			logPlayerStartedGameDependency2.gameModeType = gameModeKey.type.ToString();
			LogPlayerStartedGameDependency logPlayerStartedGameDependency3 = logPlayerStartedGameDependency;
			GameModeKey gameModeKey2 = result.GameModeKey;
			logPlayerStartedGameDependency3.isRanked = gameModeKey2.IsRanked;
			LogPlayerStartedGameDependency logPlayerStartedGameDependency4 = logPlayerStartedGameDependency;
			GameModeKey gameModeKey3 = result.GameModeKey;
			logPlayerStartedGameDependency4.isBrawl = gameModeKey3.IsBrawl;
			LogPlayerStartedGameDependency logPlayerStartedGameDependency5 = logPlayerStartedGameDependency;
			GameModeKey gameModeKey4 = result.GameModeKey;
			logPlayerStartedGameDependency5.isCustomGame = gameModeKey4.IsCustomGame;
			logPlayerStartedGameDependency.reconnectGameGUID = result.GameGuid;
			logPlayerStartedGameDependency.mapName = result.MapName;
			LogPlayerStartedGameDependency dependency = logPlayerStartedGameDependency;
			uint aiPlayersCount = 0u;
			GameModeKey gameModeKey5 = result.GameModeKey;
			if (gameModeKey5.type != GameModeType.Campaign)
			{
				GameModeKey gameModeKey6 = result.GameModeKey;
				if (gameModeKey6.type != GameModeType.PraticeMode)
				{
					foreach (KeyValuePair<string, PlayerDataDependency> item in players)
					{
						if (item.Value.AiPlayer)
						{
							aiPlayersCount++;
						}
					}
				}
			}
			dependency.aiPlayers = aiPlayersCount;
			TaskService<PlayerDataResponse> loadPlayerDataRequest = serviceRequestFactory.Create<ILoadPlayerDataRequest>().AsTask();
			yield return loadPlayerDataRequest;
			if (loadPlayerDataRequest.succeeded)
			{
				PlayerDataResponse playerData = loadPlayerDataRequest.result;
				string myName = User.Username;
				PreloadedMachine myMachine = machinePreloader.GetPreloadedMachine(myName);
				FasterList<InstantiatedCube> cubesList = myMachine.machineMap.GetAllInstantiatedCubes();
				dependency.sceneName = _levelName;
				dependency.gameModeTypeForAnalytics = _gameModeType;
				dependency.robotCPU = playerData.robotCPU;
				dependency.isCRFBot = (playerData.crfId != 0);
				dependency.controlType = playerData.controlSetting.controlType.ToString();
				dependency.verticalStrafing = playerData.controlSetting.verticalStrafing;
				dependency.totalCosmetics = GetCubeCategoryCount((IEnumerator<InstantiatedCube>)(object)cubesList.GetEnumerator(), CubeCategory.Cosmetic);
				dependency.sendShortVersion = false;
				ILoadPlayerRobotRankingRequest request = serviceRequestFactory.Create<ILoadPlayerRobotRankingRequest>();
				request.Inject(User.Username);
				request.ClearCache();
				TaskService<RankingAndCPU> playerRobotRankingTask = new TaskService<RankingAndCPU>(request);
				yield return playerRobotRankingTask;
				if (!playerRobotRankingTask.succeeded)
				{
					Console.LogError("Log Game Started request failed while retrieving Robot Ranking");
					loadingPresenter.NotifyLoadingDone("HandleAnalytics");
					yield break;
				}
				uint robotRanking = (uint)playerRobotRankingTask.result.Ranking;
				TaskService<TiersData> loadTiersBandingReq = serviceRequestFactory.Create<ILoadTiersBandingRequest>().AsTask();
				yield return loadTiersBandingReq;
				if (!loadTiersBandingReq.succeeded)
				{
					Console.LogError("Log Game Started request failed while retrieving Tiers Banding");
					loadingPresenter.NotifyLoadingDone("HandleAnalytics");
					yield break;
				}
				TiersData tiersData = loadTiersBandingReq.result;
				TaskService<CPUSettingsDependency> cpuRequest = serviceRequestFactory.Create<ILoadCpuSettingsRequest>().AsTask();
				yield return cpuRequest;
				if (!cpuRequest.succeeded)
				{
					Console.LogError("Log Game Started request failed while retrieving CPU setting");
					loadingPresenter.NotifyLoadingDone("HandleAnalytics");
					yield break;
				}
				bool isMegabot = playerData.robotCPU > cpuRequest.result.maxRegularCpu;
				_robotTier = RRAndTiers.ConvertRRToTierIndex(robotRanking, isMegabot, tiersData) + 1;
				dependency.tier = _robotTier;
			}
			else
			{
				dependency.sendShortVersion = true;
			}
			ILogPlayerStartedGameRequest service = analyticsRequestFactory.Create<ILogPlayerStartedGameRequest, LogPlayerStartedGameDependency>(dependency);
			TaskService task = new TaskService(service);
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Log Game Started request failed to send " + task.behaviour.errorBody);
			}
			loadingPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private IEnumerator SendGameEndedRequestTask(bool won, GameStateResult endReason)
		{
			loadingPresenter.NotifyLoading("HandleAnalytics");
			TimeSpan battleTime = (!(_battleStartDateUTC == DateTime.MinValue)) ? (DateTime.UtcNow - _battleStartDateUTC) : TimeSpan.Zero;
			string gameResult = (!won) ? "Lost" : "Won";
			int myTeamID = playerTeamsContainer.GetMyTeam();
			ReadOnlyCollection<int> teamPlayerIDs = playerTeamsContainer.GetPlayersOnTeam(TargetType.Player, myTeamID);
			TaskService<BattleParametersData> battleParametersRequest = lobbyRequestFactory.Create<IGetBattleParametersRequest>().AsTask();
			yield return battleParametersRequest;
			if (!battleParametersRequest.succeeded)
			{
				Console.LogError("Battle parameters request failed. " + battleParametersRequest.behaviour.exceptionThrown);
				loadingPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			BattleParametersData result = battleParametersRequest.result;
			LogPlayerEndedGameDependency logPlayerEndedGameDependency = new LogPlayerEndedGameDependency(myTeamID, (int)Math.Round(battleTime.TotalSeconds), gameResult, endReason.ToString(), _localPlayerMachineStats.totalCPU, _localPlayerMachineStats.ranking, Convert.ToInt32(Time.get_time() - worldSwitching.CurrentWorldStartTime))
			{
				totalDamageReceived = _totalDamageReceived,
				robotName = _localPlayerMachineStats.robotName,
				robotUniqueID = _localPlayerMachineStats.robotUniqueId,
				tier = _robotTier
			};
			LogPlayerEndedGameDependency logPlayerEndedGameDependency2 = logPlayerEndedGameDependency;
			GameModeKey gameModeKey = result.GameModeKey;
			logPlayerEndedGameDependency2.gameModeType = gameModeKey.type.ToString();
			LogPlayerEndedGameDependency logPlayerEndedGameDependency3 = logPlayerEndedGameDependency;
			GameModeKey gameModeKey2 = result.GameModeKey;
			logPlayerEndedGameDependency3.isRanked = gameModeKey2.IsRanked;
			LogPlayerEndedGameDependency logPlayerEndedGameDependency4 = logPlayerEndedGameDependency;
			GameModeKey gameModeKey3 = result.GameModeKey;
			logPlayerEndedGameDependency4.isBrawl = gameModeKey3.IsBrawl;
			LogPlayerEndedGameDependency logPlayerEndedGameDependency5 = logPlayerEndedGameDependency;
			GameModeKey gameModeKey4 = result.GameModeKey;
			logPlayerEndedGameDependency5.isCustomGame = gameModeKey4.IsCustomGame;
			logPlayerEndedGameDependency.reconnectGameGUID = result.GameGuid;
			logPlayerEndedGameDependency.mapName = result.MapName;
			logPlayerEndedGameDependency.sceneName = _levelName;
			logPlayerEndedGameDependency.gameModeTypeForAnalytics = _gameModeType;
			logPlayerEndedGameDependency.gameServerErrorCode = gameStateClient.gameServerErrorCode.ToString();
			logPlayerEndedGameDependency.battleEndResult = endReason;
			LogPlayerEndedGameDependency dependency = logPlayerEndedGameDependency;
			if (endReason != 0)
			{
				Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
				if (worldSwitchingAnalytics.tdmTeamKills != null)
				{
					IEnumerator<KeyValuePair<int, int>> enumerator = worldSwitchingAnalytics.tdmTeamKills.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, int> current = enumerator.Current;
						if (playerTeamsContainer.IsMyTeam(current.Key))
						{
							dictionary.Add("totalTeamKills", (uint)current.Value);
						}
						else
						{
							dictionary.Add("totalEnemyKills", (uint)current.Value);
						}
					}
				}
				for (int i = 0; i < teamPlayerIDs.Count; i++)
				{
					string playerName = playerNamesContainer.GetPlayerName(teamPlayerIDs[i]);
					BattleStatsData playerScore = battleStatsPresenter.GetPlayerScore(playerName);
					if (playerScore == null)
					{
						continue;
					}
					Dictionary<InGameStatId, uint> stats = playerScore.stats;
					if (WorldSwitching.GetGameModeType() == GameModeType.Normal)
					{
						dependency.scoreTeamScore = Convert.ToUInt32(playerTeamsContainer.GetPlayersCountOnTeam(TargetType.CapturePoint, myTeamID));
					}
					else
					{
						dependency.scoreTeamScore += stats[InGameStatId.Kill];
					}
					if (playerName == _localPlayerName)
					{
						uint value = (!stats.ContainsKey(InGameStatId.Score)) ? stats[InGameStatId.Points] : stats[InGameStatId.Score];
						dictionary.Add("userScore", value);
						if (stats.ContainsKey(InGameStatId.Kill))
						{
							dictionary.Add("scoreKills", stats[InGameStatId.Kill]);
						}
						if (stats.ContainsKey(InGameStatId.RobotDestroyed))
						{
							dictionary.Add("scoreRobotDestroyed", stats[InGameStatId.RobotDestroyed]);
						}
						if (stats.ContainsKey(InGameStatId.KillAssist))
						{
							dictionary.Add("scoreKillAssist", stats[InGameStatId.KillAssist]);
						}
						if (stats.ContainsKey(InGameStatId.DestroyedCubes))
						{
							dictionary.Add("scoreDestroyedCubes", stats[InGameStatId.DestroyedCubes]);
						}
						if (stats.ContainsKey(InGameStatId.HealCubes))
						{
							dictionary.Add("scoreHealCubes", stats[InGameStatId.HealCubes]);
						}
						if (stats.ContainsKey(InGameStatId.CurrentKillStreak))
						{
							dictionary.Add("scoreCurrentKillStreak", stats[InGameStatId.CurrentKillStreak]);
						}
						if (stats.ContainsKey(InGameStatId.BestKillStreak))
						{
							dictionary.Add("scoreBestKillStreak", stats[InGameStatId.BestKillStreak]);
						}
						if (stats.ContainsKey(InGameStatId.BattleArenaObjectives))
						{
							dictionary.Add("scoreBAObjectives", stats[InGameStatId.BattleArenaObjectives]);
						}
					}
				}
				dependency.playerBattleScore = dictionary;
			}
			ILogPlayerEndedGameRequest service = analyticsRequestFactory.Create<ILogPlayerEndedGameRequest, LogPlayerEndedGameDependency>(dependency);
			TaskService task = new TaskService(service);
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Game Ended failed to send " + task.behaviour.errorBody);
			}
			loadingPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private int GetCubeCategoryCount(IEnumerator<InstantiatedCube> cubesEnumerator, CubeCategory category)
		{
			int num = 0;
			while (cubesEnumerator.MoveNext())
			{
				PersistentCubeData persistentCubeData = cubesEnumerator.Current.persistentCubeData;
				if (persistentCubeData.category == category)
				{
					num++;
				}
			}
			return num;
		}

		private void SaveLevelAndMode()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			object levelName;
			if (WorldSwitching.GetGameModeType() == GameModeType.Campaign)
			{
				levelName = WorldSwitching.GetCampaignName() + " " + WorldSwitching.GetCampaignDifficulty();
			}
			else
			{
				Scene activeScene = SceneManager.GetActiveScene();
				levelName = activeScene.get_name();
			}
			_levelName = (string)levelName;
			_gameModeType = GetGameModeTypeForAnalytics();
			PlayerPrefs.SetString("analytics_gameMode_" + User.Username, _gameModeType);
			PlayerPrefs.SetString("analytics_levelName_" + User.Username, _levelName);
		}

		private string GetGameModeTypeForAnalytics()
		{
			GameModeType gameModeType = WorldSwitching.GetGameModeType();
			string result = gameModeType.ToString();
			if (gameModeType == GameModeType.TestMode)
			{
				result = ((!tutorialController.IsActive() && !WorldSwitching.IsTutorial()) ? "TestMode" : "Tutorial");
			}
			else if (WorldSwitching.IsCustomGame())
			{
				result = "CustomGame";
			}
			else if (WorldSwitching.IsBrawl())
			{
				result = "Brawl";
			}
			return result;
		}

		private void AddToPlayerDamage(HealthTracker.PlayerHealthChangedInfo info)
		{
			if (info.deltaHealth < 0 && _localPlayerId == info.shotPlayerId)
			{
				_totalDamageReceived += -info.deltaHealth;
			}
		}

		private bool SendPlayerQuitRequest()
		{
			if (!_quitTriggered)
			{
				_quitTriggered = true;
				TaskRunner.get_Instance().Run((Func<IEnumerator>)SendPlayerQuitRequestEnumerator);
			}
			return _quitRequestSent;
		}

		private IEnumerator SendPlayerQuitRequestEnumerator()
		{
			yield return SendGameEndedRequestTask(won: false, GameStateResult.Quit);
			_quitRequestSent = true;
		}
	}
}
