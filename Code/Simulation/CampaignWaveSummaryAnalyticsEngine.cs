using Authentication;
using Battle;
using LobbyServiceLayer;
using Services.Analytics;
using SinglePlayerCampaign;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.Context;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Simulation
{
	internal class CampaignWaveSummaryAnalyticsEngine : MultiEntityViewsEngine<CampaignWaveVictoryCheckEntityView, CampaignWaveDefeatCheckEntityView>, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private bool _quitRequestSent;

		private bool _quitTriggered;

		private readonly GameStateClient _gameStateClient;

		private readonly ILobbyRequestFactory _lobbyRequestFactory;

		private readonly IServiceRequestFactory _serviceRequestFactory;

		private readonly IAnalyticsRequestFactory _analyticsRequestFactory;

		private readonly BattlePlayers _battlePlayers;

		private readonly QuitListenerManager _quitListenerManager;

		private readonly LoadingIconPresenter _loadingPresenter;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		internal CampaignWaveSummaryAnalyticsEngine(WorldSwitching worldSwitching, ILobbyRequestFactory lobbyRequestFactory, IServiceRequestFactory serviceRequestFactory, IAnalyticsRequestFactory analyticsRequestFactory, BattlePlayers battlePlayers, QuitListenerManager quitListenerManager, GameStateClient gameStateClient, LoadingIconPresenter loadingPresenter)
		{
			worldSwitching.OnWorldIsSwitching.Add(OnWorldSwitching());
			_lobbyRequestFactory = lobbyRequestFactory;
			_serviceRequestFactory = serviceRequestFactory;
			_analyticsRequestFactory = analyticsRequestFactory;
			_battlePlayers = battlePlayers;
			_quitListenerManager = quitListenerManager;
			_quitListenerManager.AddOnQuitTriggered(SendPlayerQuitRequest);
			_gameStateClient = gameStateClient;
			_loadingPresenter = loadingPresenter;
		}

		public void Ready()
		{
		}

		public void OnFrameworkDestroyed()
		{
			_quitListenerManager.RemoveOnQuitTriggered(SendPlayerQuitRequest);
		}

		protected override void Add(CampaignWaveVictoryCheckEntityView entityView)
		{
			entityView.waveVictoryComponent.victoryAchieved.NotifyOnValueSet((Action<int, bool>)OnVictoryAchieved);
		}

		protected override void Remove(CampaignWaveVictoryCheckEntityView entityView)
		{
			entityView.waveVictoryComponent.victoryAchieved.StopNotify((Action<int, bool>)OnVictoryAchieved);
		}

		protected override void Add(CampaignWaveDefeatCheckEntityView entityView)
		{
			entityView.waveDefeatComponent.defeatHappened.NotifyOnValueSet((Action<int, bool>)OnDefeatHappened);
		}

		protected override void Remove(CampaignWaveDefeatCheckEntityView entityView)
		{
			entityView.waveDefeatComponent.defeatHappened.StopNotify((Action<int, bool>)OnDefeatHappened);
		}

		private void OnVictoryAchieved(int entityId, bool victoryAchieved)
		{
			TaskRunner.get_Instance().Run(SendLogData(victoryAchieved: true, GameStateResult.Won));
		}

		private void OnDefeatHappened(int entityId, bool victoryAchieved)
		{
			TaskRunner.get_Instance().Run(SendLogData(victoryAchieved: false, GameStateResult.Lost));
		}

		private IEnumerator SendLogData(bool victoryAchieved, GameStateResult endReason)
		{
			_loadingPresenter.NotifyLoading("HandleAnalytics");
			CampaignWaveVictoryCheckEntityView campaignWaveVictoryCheckEntityView = entityViewsDB.QueryEntityView<CampaignWaveVictoryCheckEntityView>(207);
			CampaignWaveLostLivesEntityView campaignWaveLostLivesEntityView = entityViewsDB.QueryEntityView<CampaignWaveLostLivesEntityView>(207);
			CampaignUpdateRemainingLivesEntityView campaignUpdateRemainingLivesEntityView = entityViewsDB.QueryEntityView<CampaignUpdateRemainingLivesEntityView>(208);
			CurrentWaveTrackerEntityView currentWaveTrackerEntityView = entityViewsDB.QueryEntityView<CurrentWaveTrackerEntityView>(208);
			int currentWave = currentWaveTrackerEntityView.CurrentWaveCounterComponent.counterValue.get_value();
			int elapsedTime = Convert.ToInt32(campaignWaveVictoryCheckEntityView.timeComponent.elapsedTime);
			int killCount = Convert.ToInt32(campaignWaveVictoryCheckEntityView.killCountComponent.killCount.get_value());
			int lostLives = Convert.ToInt32(campaignWaveLostLivesEntityView.currentWaveLostLivesComponent.counterValue.get_value());
			int remainingLives = Convert.ToInt32(campaignUpdateRemainingLivesEntityView.remainingLivesComponent.remainingLives.get_value());
			Dictionary<string, PlayerDataDependency> expectedPlayers = _battlePlayers.GetExpectedPlayersDict();
			TaskService<BattleParametersData> battleParametersRequest = _lobbyRequestFactory.Create<IGetBattleParametersRequest>().AsTask();
			yield return battleParametersRequest;
			if (!battleParametersRequest.succeeded)
			{
				Console.LogError("Battle parameters request failed. " + battleParametersRequest.behaviour.exceptionThrown);
				_loadingPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			BattleParametersData battleParametersResult = battleParametersRequest.result;
			TaskService<PlayerDataResponse> loadPlayerDataRequest = _serviceRequestFactory.Create<ILoadPlayerDataRequest>().AsTask();
			yield return loadPlayerDataRequest;
			if (!loadPlayerDataRequest.succeeded)
			{
				Console.LogError("Load Player Data Request failed. " + loadPlayerDataRequest.behaviour.exceptionThrown);
				_loadingPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			PlayerDataResponse playerDataResult = loadPlayerDataRequest.result;
			LogPlayerCampaignWaveSummaryDependency summaryDependency = new LogPlayerCampaignWaveSummaryDependency(StringTableBase<StringTable>.Instance.GetString(WorldSwitching.GetCampaignName()), WorldSwitching.GetCampaignDifficulty(), StringTableBase<StringTable>.Instance.GetString(CampaignDifficultyHelper.difficultyNameKeys[WorldSwitching.GetCampaignDifficulty()]), battleParametersResult.MapName, WorldSwitching.GetCampaignID(), WorldSwitching.GetCampaignName(), currentWave + 1, elapsedTime, (!victoryAchieved) ? "fail" : "pass", killCount, lostLives, remainingLives, expectedPlayers[User.Username].RobotName, expectedPlayers[User.Username].RobotUniqueId, Convert.ToInt32(playerDataResult.robotCPU), endReason.ToString());
			TaskService logPlayerCampaignWaveSummaryRequest = _analyticsRequestFactory.Create<ILogPlayerCampaignWaveSummaryRequest, LogPlayerCampaignWaveSummaryDependency>(summaryDependency).AsTask();
			yield return logPlayerCampaignWaveSummaryRequest;
			if (!logPlayerCampaignWaveSummaryRequest.succeeded)
			{
				Console.LogError("Log Player Campaign Wave Summary Request failed. " + logPlayerCampaignWaveSummaryRequest.behaviour.exceptionThrown);
			}
			_loadingPresenter.NotifyLoadingDone("HandleAnalytics");
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
			yield return SendLogData(victoryAchieved: false, GameStateResult.Quit);
			_quitRequestSent = true;
		}

		private IEnumerator OnWorldSwitching()
		{
			if (_gameStateClient.battleEndResult != 0 && _gameStateClient.battleEndResult != GameStateResult.Won && _gameStateClient.battleEndResult != GameStateResult.Lost)
			{
				yield return SendLogData(victoryAchieved: false, _gameStateClient.battleEndResult);
			}
		}
	}
}
