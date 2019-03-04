using Achievements;
using RCNetwork.Events;
using RCNetwork.Server;
using Simulation;
using Simulation.SinglePlayer;
using Simulation.SinglePlayer.Rewards;
using Simulation.SinglePlayer.ServerMock;
using SinglePlayerCampaign.GUI.Simulation.Components;
using SinglePlayerCampaign.GUI.Simulation.EntityDescriptors;
using SinglePlayerCampaign.Simulation.Components;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignServerMock : MultiEntityViewsEngine<CampaignVictoryCheckEntityView, CampaignDefeatCheckEntityView>, IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private GameModeSettings _teamDeathMatchGameModeSettings;

		private bool _gameEnded;

		private IAchievementManager _achievementManager;

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

		[Inject]
		public ICommandFactory commandFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public CampaignServerMock(IObserver allPlayersSpawnedObserver, IAchievementManager achievementManager)
		{
			_achievementManager = achievementManager;
			allPlayersSpawnedObserver.AddAction((Action)StartTheGame);
		}

		public void Ready()
		{
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

		protected override void Add(CampaignVictoryCheckEntityView entityView)
		{
			entityView.currentWaveComponent.counterValue.NotifyOnValueSet((Action<int, int>)ClearEnemyStatsOnWaveStart);
			entityView.campaignVictoryComponent.victoryAchieved.NotifyOnValueSet((Action<int, bool>)OnVictory);
		}

		protected override void Remove(CampaignVictoryCheckEntityView entityView)
		{
			entityView.currentWaveComponent.counterValue.StopNotify((Action<int, int>)ClearEnemyStatsOnWaveStart);
			entityView.campaignVictoryComponent.victoryAchieved.StopNotify((Action<int, bool>)OnVictory);
		}

		protected override void Add(CampaignDefeatCheckEntityView entityView)
		{
			entityView.campaignDefeatComponent.defeatHappened.NotifyOnValueSet((Action<int, bool>)OnDefeat);
		}

		protected override void Remove(CampaignDefeatCheckEntityView entityView)
		{
			entityView.campaignDefeatComponent.defeatHappened.StopNotify((Action<int, bool>)OnDefeat);
		}

		private void HandleGameSettingsLoaded(GameModeSettingsDependency gameModeSettingsDependency)
		{
			_teamDeathMatchGameModeSettings = new GameModeSettings(gameModeSettingsDependency.TeamDeathmatch);
		}

		private void StartTheGame()
		{
			gameStartDispatcher.OnGameStart();
			UpdateGameModeSettingsDependency dependency = new UpdateGameModeSettingsDependency(_teamDeathMatchGameModeSettings.respawnHealDuration, _teamDeathMatchGameModeSettings.respawnFullHealDuration);
			networkEventManagerServer.BroadcastEventToAllPlayers(NetworkEvent.GameModeSettings, dependency);
		}

		private void ClearEnemyStatsOnWaveStart(int entityID, int currentWave)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			int maxValue = entityViewsDB.QueryEntityView<CurrentWaveTrackerEntityView>(entityID).CurrentWaveCounterComponent.maxValue;
			if (currentWave > 0 && currentWave < maxValue)
			{
				int playerId = entityViewsDB.QueryEntityViews<PlayerTargetNode>().get_Item(0).playerTargetGameObjectComponent.playerId;
				teamDeathMatchAIScoreServerMock.ClearEnemyStats(playerId);
			}
		}

		private void OnDefeat(int entitytId, bool defeatHappened)
		{
			if (defeatHappened)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)OnPlayerDefeat);
			}
		}

		private void HandleOnPlayerSelfDestructs(int playerId)
		{
			HandleOnMachineKilled(playerId, playerId);
		}

		private void HandleOnMachineKilled(int victimId, int shooterId)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (_gameEnded)
			{
				return;
			}
			teamDeathMatchAIScoreServerMock.UpdateStats(victimId, InGameStatId.RobotDestroyed, 1);
			if (shooterId != victimId)
			{
				teamDeathMatchAIScoreServerMock.UpdateStats(shooterId, InGameStatId.Kill, 1);
				int playerId = entityViewsDB.QueryEntityViews<PlayerTargetNode>().get_Item(0).playerTargetGameObjectComponent.playerId;
				if (shooterId == playerId)
				{
					networkEventManagerServer.SendEventToPlayer(NetworkEvent.ConfirmedKill, 0, new KillDependency(victimId, shooterId));
				}
			}
		}

		private void OnVictory(int entityId, bool victoryAchieved)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (victoryAchieved)
			{
				FasterReadOnlyList<GameOverNoMoreWavesEntityView> val = entityViewsDB.QueryEntityViews<GameOverNoMoreWavesEntityView>();
				_achievementManager.EarnFirstCampaign();
				ITransitionAnimationComponent animationComponent = val.get_Item(0).AnimationComponent;
				animationComponent.PlayEndBattle(won: true);
				TriggerGameEnded(NetworkEvent.GameWon);
				TaskRunner.get_Instance().Run(GoToMothership(animationComponent));
			}
		}

		private IEnumerator OnPlayerDefeat()
		{
			ITimeComponent timeComponent = entityViewsDB.QueryEntityView<CampaignWaveUpdateTimeEntityView>(207).timeComponent;
			timeComponent.timeRunning.set_value(false);
			FasterReadOnlyList<GameOverPlayerEntityView> gameLostEntityViews = entityViewsDB.QueryEntityViews<GameOverPlayerEntityView>();
			yield return null;
			ITransitionAnimationComponent animation = gameLostEntityViews.get_Item(0).AnimationComponent;
			animation.PlayEndBattle(won: false);
			TriggerGameEnded(NetworkEvent.GameLost);
			TaskRunner.get_Instance().Run(GoToMothership(animation));
		}

		private void TriggerGameEnded(NetworkEvent networkEvent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<PlayerTargetNode> val = entityViewsDB.QueryEntityViews<PlayerTargetNode>();
			int playerId = val.get_Item(0).playerTargetGameObjectComponent.playerId;
			int winningTeam = (networkEvent != NetworkEvent.GameWon) ? entityViewsDB.QueryEntityViews<AIAgentDataComponentsNode>().get_Item(0).aiBotIdData.teamId : val.get_Item(0).playerTargetGameObjectComponent.teamId;
			List<int> list = new List<int>();
			list.Add(playerId);
			GameLostDependency dependency = new GameLostDependency(list, winningTeam, GameEndReason.OneTeamRemaining);
			networkEventManagerServer.SendEventToPlayer(networkEvent, playerId, dependency);
			_gameEnded = true;
			tdmPracticeGamEndedObservable.Dispatch(ref winningTeam);
		}

		private IEnumerator GoToMothership(ITransitionAnimationComponent animation)
		{
			while (animation.IsPlaying)
			{
				yield return null;
			}
			commandFactory.Build<SwitchToMothershipCommand>().Execute();
		}
	}
}
