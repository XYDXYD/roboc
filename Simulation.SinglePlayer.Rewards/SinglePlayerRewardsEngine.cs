using Authentication;
using GameServer;
using Simulation.SinglePlayerCampaign.EntityViews;
using SinglePlayerCampaign.Simulation.EntityViews;
using SinglePlayerServiceLayer;
using SocialServiceLayer;
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
using Utility;

namespace Simulation.SinglePlayer.Rewards
{
	internal class SinglePlayerRewardsEngine : MultiEntityViewsEngine<PlayerTargetNode, AIAgentDataComponentsNode>, IWaitForFrameworkDestruction, IInitialize, IQueryingEntityViewEngine, IEngine
	{
		private readonly Dictionary<int, AIAgentDataComponentsNode> _agentsIdLookupTable = new Dictionary<int, AIAgentDataComponentsNode>();

		private readonly Dictionary<PlayerAwardId, int> _humanPlayerBonusAmount = new Dictionary<PlayerAwardId, int>();

		private readonly StatsUpdatedObserver _statsUpdatedObserver;

		private readonly FinalStatsUpdatedObserver _finalStatsUpdatedObserver;

		private readonly TDMPracticeGamEndedObserver _tdmPracticeGamEndedObserver;

		private bool _bonusFlushRequestComplete;

		private bool _gameCompleted;

		private PlayerTargetNode _playerTargetNode;

		private int _winningTeamId = -1;

		[Inject]
		public WorldSwitching worldSwitching
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
		public ISinglePlayerRequestFactory singlePlayerRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public unsafe SinglePlayerRewardsEngine(StatsUpdatedObserver statsUpdatedObserver, FinalStatsUpdatedObserver finalStatsUpdatedObserver, TDMPracticeGamEndedObserver tdmPracticeGamEndedObserver)
		{
			_finalStatsUpdatedObserver = finalStatsUpdatedObserver;
			_statsUpdatedObserver = statsUpdatedObserver;
			_tdmPracticeGamEndedObserver = tdmPracticeGamEndedObserver;
			_finalStatsUpdatedObserver.AddAction(new ObserverAction<StatsUpdatedEvent>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_statsUpdatedObserver.AddAction(new ObserverAction<StatsUpdatedEvent>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_tdmPracticeGamEndedObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Ready()
		{
		}

		public void OnDependenciesInjected()
		{
			worldSwitching.OnWorldIsSwitching.Add(OnSwitchWorld());
		}

		protected override void Add(AIAgentDataComponentsNode entityView)
		{
			_agentsIdLookupTable[entityView.aiBotIdData.playerId] = entityView;
		}

		protected override void Remove(AIAgentDataComponentsNode entityView)
		{
			_agentsIdLookupTable.Remove(entityView.aiBotIdData.playerId);
		}

		protected override void Add(PlayerTargetNode entityView)
		{
			_playerTargetNode = entityView;
		}

		protected override void Remove(PlayerTargetNode entityView)
		{
			_playerTargetNode = null;
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_statsUpdatedObserver.RemoveAction(new ObserverAction<StatsUpdatedEvent>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_tdmPracticeGamEndedObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_finalStatsUpdatedObserver.RemoveAction(new ObserverAction<StatsUpdatedEvent>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private IEnumerator OnSwitchWorld()
		{
			if (!_gameCompleted)
			{
				_bonusFlushRequestComplete = true;
			}
			while (!_bonusFlushRequestComplete)
			{
				yield return null;
			}
		}

		private void HandleOnFinalStatsUpdated(ref StatsUpdatedEvent statsUpdatedEvent)
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleOnFinalStatsUpdatedEnumerator);
		}

		private IEnumerator HandleOnFinalStatsUpdatedEnumerator()
		{
			float longPlayMultiplier = 1f;
			IGetLongPlayMultiplierRequest getLongPlayMultiplierRequest = serviceRequestFactory.Create<IGetLongPlayMultiplierRequest>();
			getLongPlayMultiplierRequest.SetAnswer(new ServiceAnswer<float>(delegate(float longPlayMultiplier_)
			{
				longPlayMultiplier = longPlayMultiplier_;
			}));
			getLongPlayMultiplierRequest.Execute();
			FasterReadOnlyList<WavesDataEntityView> wavesDataEntityViews = entityViewsDB.QueryEntityViews<WavesDataEntityView>();
			int singleWaveCompletionBonus = -1;
			if (wavesDataEntityViews.get_Count() > 0)
			{
				FasterListEnumerator<WavesDataEntityView> enumerator = wavesDataEntityViews.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						WavesDataEntityView current = enumerator.get_Current();
						CampaignDifficultySetting campaignDifficulty2 = current.wavesData.wavesData.CampaignDifficulty;
						PlayerSetting playerDifficultySetting = campaignDifficulty2.PlayerDifficultySetting;
						singleWaveCompletionBonus = playerDifficultySetting.SingleWaveCompletionBonus;
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
			GameResult gameResult = BuildGameResult(_winningTeamId, singleWaveCompletionBonus);
			GameModeType gameModeType = WorldSwitching.GetGameModeType();
			SaveGameAwardsRequestDependency saveGameAwardsRequestDependency = new SaveGameAwardsRequestDependency(campaignId_: WorldSwitching.GetCampaignID(), campaignDifficulty_: WorldSwitching.GetCampaignDifficulty(), gameResult_: gameResult, longPlayMultiplier_: longPlayMultiplier);
			if (gameModeType == GameModeType.Campaign)
			{
				ISaveCampaignGameAwardsRequest singlePlayerSaveResultReq2 = serviceRequestFactory.Create<ISaveCampaignGameAwardsRequest>();
				singlePlayerSaveResultReq2.Inject(saveGameAwardsRequestDependency);
				TaskService singlePlayerSaveResultTS2 = singlePlayerSaveResultReq2.AsTask();
				yield return singlePlayerSaveResultTS2;
				if (singlePlayerSaveResultTS2.succeeded)
				{
					Console.Log("Flushed campaign results");
				}
				else
				{
					HandleServiceError(singlePlayerSaveResultTS2.behaviour);
				}
			}
			else
			{
				ISinglePlayerSaveResultRequest singlePlayerSaveResultReq = singlePlayerRequestFactory.Create<ISinglePlayerSaveResultRequest>();
				singlePlayerSaveResultReq.Inject(saveGameAwardsRequestDependency);
				TaskService singlePlayerSaveResultTS = singlePlayerSaveResultReq.AsTask();
				yield return singlePlayerSaveResultTS;
				if (singlePlayerSaveResultTS.succeeded)
				{
					Console.Log("Flushed tdm results");
				}
				else
				{
					HandleServiceError(singlePlayerSaveResultTS.behaviour);
				}
			}
			HandleBonusFlushAllRequestsComplete();
		}

		private void HandleOnGameWon(ref int winningTeamId)
		{
			_gameCompleted = true;
			_winningTeamId = winningTeamId;
		}

		private GameResult BuildGameResult(int winningTeamId, int singleWaveCompletionBonus)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (!_humanPlayerBonusAmount.ContainsKey(PlayerAwardId.XpScore))
			{
				_humanPlayerBonusAmount[PlayerAwardId.XpScore] = 0;
			}
			_humanPlayerBonusAmount[PlayerAwardId.CampaignScoreBonus] = 0;
			GameModeType gameModeType = WorldSwitching.GetGameModeType();
			if (singleWaveCompletionBonus > 0)
			{
				int value = entityViewsDB.QueryEntityViews<CurrentWaveTrackerEntityView>().get_Item(0).CurrentWaveCounterComponent.counterValue.get_value();
				if (value > 0)
				{
					_humanPlayerBonusAmount[PlayerAwardId.CampaignScoreBonus] = value * singleWaveCompletionBonus;
				}
			}
			int num = _humanPlayerBonusAmount[PlayerAwardId.XpScore];
			int num2 = 0;
			int num3 = 0;
			foreach (KeyValuePair<int, AIAgentDataComponentsNode> item2 in _agentsIdLookupTable)
			{
				AIAgentDataComponentsNode value2 = item2.Value;
				if (num < value2.aiScoreComponent.score)
				{
					if (value2.aiBotIdData.teamId == _playerTargetNode.playerTargetGameObjectComponent.teamId)
					{
						num3++;
					}
					num2++;
				}
			}
			bool flag = _playerTargetNode.playerTargetGameObjectComponent.teamId == winningTeamId;
			PlayerAwardsContainer item = new PlayerAwardsContainer(User.Username, flag, (uint)num, _humanPlayerBonusAmount, num2, num3, 0, _isPlayerAlive: true, _playerTargetNode.playerRobotMasteryComponent.masteryLevel, _playerTargetNode.playerTargetGameObjectComponent.teamId);
			List<PlayerAwardsContainer> list = new List<PlayerAwardsContainer>();
			List<PlayerAwardsContainer> list2 = new List<PlayerAwardsContainer>();
			if (flag)
			{
				list.Add(item);
			}
			else
			{
				list2.Add(item);
			}
			GameModeKey gameModeType2 = new GameModeKey(gameModeType);
			return new GameResult(list, list2, gameModeType2);
		}

		private void HandleServiceError(ServiceBehaviour behaviour)
		{
			GenericErrorData error = new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, behaviour.mainText, behaviour.alternativeText, delegate
			{
				_bonusFlushRequestComplete = true;
			}, delegate
			{
				_bonusFlushRequestComplete = true;
			});
			ErrorWindow.ShowErrorWindow(error);
		}

		private void HandleBonusFlushAllRequestsComplete()
		{
			_bonusFlushRequestComplete = true;
		}

		private void HandleStatsUpdated(ref StatsUpdatedEvent statsUpdatedEvent)
		{
			if (statsUpdatedEvent.playerId == _playerTargetNode.playerTargetGameObjectComponent.playerId)
			{
				if (TryGetRewardId(statsUpdatedEvent.gameStatId, out PlayerAwardId playerAwardId))
				{
					if (!_humanPlayerBonusAmount.ContainsKey(playerAwardId))
					{
						_humanPlayerBonusAmount[playerAwardId] = 0;
					}
					Dictionary<PlayerAwardId, int> humanPlayerBonusAmount;
					PlayerAwardId key;
					(humanPlayerBonusAmount = _humanPlayerBonusAmount)[key = playerAwardId] = humanPlayerBonusAmount[key] + statsUpdatedEvent.deltaAmount;
				}
			}
			else if (statsUpdatedEvent.gameStatId == InGameStatId.Score)
			{
				_agentsIdLookupTable[statsUpdatedEvent.playerId].aiScoreComponent.score += statsUpdatedEvent.deltaAmount;
			}
		}

		private static bool TryGetRewardId(InGameStatId gameStatsId, out PlayerAwardId playerAwardId)
		{
			playerAwardId = PlayerAwardId.None;
			bool result = true;
			switch (gameStatsId)
			{
			case InGameStatId.DestroyedCubes:
				playerAwardId = PlayerAwardId.DestroyedCubes;
				break;
			case InGameStatId.DestroyedCubesDefendingTheBase:
				playerAwardId = PlayerAwardId.DestroyedCubesDefendingTheBase;
				break;
			case InGameStatId.DestroyedCubesInProtection:
				playerAwardId = PlayerAwardId.DestroyedCubesInProtection;
				break;
			case InGameStatId.HealAssist:
				playerAwardId = PlayerAwardId.HealAssist;
				break;
			case InGameStatId.HealCubes:
				playerAwardId = PlayerAwardId.HealCubes;
				break;
			case InGameStatId.Kill:
				playerAwardId = PlayerAwardId.Kill;
				break;
			case InGameStatId.KillAssist:
				playerAwardId = PlayerAwardId.KillAssist;
				break;
			case InGameStatId.Score:
				playerAwardId = PlayerAwardId.XpScore;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}
	}
}
