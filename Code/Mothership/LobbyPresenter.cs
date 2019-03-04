using Authentication;
using Battle;
using ChatServiceLayer;
using CustomGames;
using LobbyServiceLayer;
using Mothership.Network;
using Services;
using Services.Analytics;
using Services.Web;
using Services.Web.Photon;
using SocialServiceLayer;
using SocialServiceLayer.Photon;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class LobbyPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private const string PlayerprefAfkWarningKey = "afkWarningShown";

		private Platoon _platoon = new Platoon();

		private bool _isInQueue;

		private bool _leavingQueue;

		private IServiceEventContainer _lobbyEventContainer;

		private IServiceEventContainer _socialEventContainer;

		private bool _gameGuidValidated;

		private bool _gameStarted;

		private bool _errorConnectingToGame;

		private bool _gameServerRepliedWithError;

		[Inject]
		internal IChatRequestFactory chatRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal ILobbyEventContainerFactory lobbyEventContainerFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ILobbyRequestFactory lobbyRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialEventContainerFactory socialEventContainerFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal BattleCountdownScreenController battleCountdownScreenController
		{
			private get;
			set;
		}

		[Inject]
		internal BattleFoundObserver battleFoundObserver
		{
			private get;
			set;
		}

		[Inject]
		internal BattlePlayersMothership battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		internal DesiredGameMode desiredGameMode
		{
			private get;
			set;
		}

		[Inject]
		internal GaragePresenter garagePresenter
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter LoadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal LobbyView lobbyView
		{
			private get;
			set;
		}

		private int GarageSlot => (int)garagePresenter.currentGarageSlot;

		private LobbyType DesiredMode => desiredGameMode.DesiredMode;

		public int PlatoonSize => (!_platoon.isInPlatoon) ? 1 : _platoon.Size;

		public event Action EnteredQueueEvent;

		public event Action LeftQueueEvent;

		public event Action<long> EntryBlockedEvent;

		public void OnDependenciesInjected()
		{
			SetAfkWarningFlag(hasSeenWarning: false);
			_socialEventContainer = socialEventContainerFactory.Create();
			_socialEventContainer.ListenTo<IPlatoonChangedEventListener, Platoon>(PlatoonChanged);
			_socialEventContainer.ListenTo<IPlatoonRobotTierChangedEventListener, PlatoonRobotTierEventData>(OnPlatoonRobotTierChanged);
		}

		public void OnFrameworkDestroyed()
		{
			if (_lobbyEventContainer != null)
			{
				_lobbyEventContainer.Dispose();
				_lobbyEventContainer = null;
			}
			if (_socialEventContainer != null)
			{
				_socialEventContainer.Dispose();
				_socialEventContainer = null;
			}
		}

		public void Reconnect(EnterBattleDependency dependency)
		{
			OnBattleStart(dependency);
		}

		public void TryEnterMatchmakingQueue()
		{
			_gameStarted = false;
			Console.Log("Joining matchmaking queue");
			ITaskRoutine obj = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator(EnterMatchMakingFlow());
			Action action = lobbyView.HideLoadingScreen;
			obj.Start((Action<PausableTaskException>)null, action);
		}

		public void LeaveMatchmakingQueue()
		{
			LeaveMatchmakingQueue(null);
		}

		private IEnumerator EnterMatchMakingFlow()
		{
			lobbyView.ShowLoadingScreen();
			ICheckPendingSanctionRequest checkPendingRequest = chatRequestFactory.Create<ICheckPendingSanctionRequest>();
			TaskService<bool> task = new TaskService<bool>(checkPendingRequest);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				lobbyView.ShowLoadingScreen();
			}, delegate
			{
				lobbyView.HideLoadingScreen();
			}).GetEnumerator();
			if (task.succeeded)
			{
				lobbyView.HideLoadingScreen();
				yield return OnPendingSanctionsChecked(task.result);
			}
			else
			{
				OnFailedToJoinMatchmakingQueue(task.behaviour);
				lobbyView.HideLoadingScreen();
				yield return null;
			}
		}

		private IEnumerator OnPendingSanctionsChecked(bool isSuspended)
		{
			if (isSuspended)
			{
				battleCountdownScreenController.ChangeStateToExited();
				guiInputController.CloseCurrentScreen();
				yield return Break.It;
			}
			_leavingQueue = false;
			bool isInCustomGame = false;
			IRetrieveCustomGameSessionRequest retrieveRequest = serviceRequestFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> retrieveTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveRequest);
			yield return retrieveTask;
			if (!retrieveTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(retrieveTask.behaviour);
				yield break;
			}
			RetrieveCustomGameSessionRequestData customGame = retrieveTask.result;
			if (customGame.Response == CustomGameSessionRetrieveResponse.SessionRetrieved)
			{
				isInCustomGame = true;
			}
			if (isInCustomGame)
			{
				yield return EnterQueue_Internal();
			}
			else if (GameModeUtils.HasQuitterPenalty(DesiredMode))
			{
				IGetQuitterPenaltyInfo getQuitterPenaltyInfo = lobbyRequestFactory.Create<IGetQuitterPenaltyInfo>();
				TaskService<QuitterInfo> task = new TaskService<QuitterInfo>(getQuitterPenaltyInfo);
				yield return new HandleTaskServiceWithError(task, delegate
				{
					lobbyView.ShowLoadingScreen();
				}, delegate
				{
					lobbyView.HideLoadingScreen();
				}).GetEnumerator();
				if (task.succeeded)
				{
					yield return OnGotQuitterInfo(task.result);
					yield return EnterQueue_Internal();
				}
				else
				{
					OnFailedToJoinMatchmakingQueue(task.behaviour);
				}
			}
			else
			{
				yield return EnterQueue_Internal();
			}
		}

		private IEnumerator OnGotQuitterInfo(QuitterInfo quitterInfo)
		{
			if (!quitterInfo.QuitLastGame)
			{
				yield break;
			}
			if (quitterInfo.QuitterBlockTime == 0)
			{
				if (!GetAfkWarningFlag())
				{
					commandFactory.Build<AFKWarningClientCommand>().Execute();
					yield return Break.It;
				}
			}
			else
			{
				if (this.EntryBlockedEvent != null)
				{
					this.EntryBlockedEvent(quitterInfo.QuitterBlockTime);
				}
				yield return Break.It;
			}
		}

		private IEnumerator EnterQueue_Internal()
		{
			_gameGuidValidated = false;
			LobbyType gameMode = desiredGameMode.DesiredMode;
			if (_lobbyEventContainer == null)
			{
				_lobbyEventContainer = lobbyEventContainerFactory.Create();
				_lobbyEventContainer.ListenTo<IErrorJoiningQueueEventListener, LobbyReasonCode>(OnGroupFailedToJoinMatchmakingQueue);
				_lobbyEventContainer.ListenTo<IErrorInQueueEventListener, LobbyReasonCode>(OnGroupErrorDuringMatchmakingQueue);
				_lobbyEventContainer.ListenTo<IErrorJoiningBattleEventListener, LobbyReasonCode>(OnGroupFailedToJoinBattle);
				_lobbyEventContainer.ListenTo<IStartConnectionTestEventListener, Host, NetworkConfig>(OnStartConnectionTestReceived);
				_lobbyEventContainer.ListenTo<IBattleFoundEventListener>(OnBattleFound);
				_lobbyEventContainer.ListenTo<IBattleCancelledEventListener>(OnBattleCancelled);
				_lobbyEventContainer.ListenTo<IEnterBattleEventListener, EnterBattleDependency>(OnBattleStart, OnBattleFoundMessageFailed);
				_lobbyEventContainer.DisconnectedEvent += OnDisconnectedFromLobby;
			}
			IGetBrawlParametersRequest getBrawlParametersRequest = serviceRequestFactory.Create<IGetBrawlParametersRequest>();
			TaskService<GetBrawlRequestResult> brawlTask = new TaskService<GetBrawlRequestResult>(getBrawlParametersRequest);
			yield return new HandleTaskServiceWithError(brawlTask, delegate
			{
				lobbyView.ShowLoadingScreen();
			}, delegate
			{
				lobbyView.HideLoadingScreen();
			}).GetEnumerator();
			if (!brawlTask.succeeded)
			{
				OnFailedToJoinMatchmakingQueue(brawlTask.behaviour);
				yield break;
			}
			GetBrawlRequestResult brawlResult = brawlTask.result;
			int brawlVersionNum = brawlResult.BrawlParameters.VersionNumber;
			GameModePreferences gameModePreferences = default(GameModePreferences);
			if (GameModeUtils.HasRandomGameTypes(gameMode))
			{
				IGetGameModePreferencesRequest request = serviceRequestFactory.Create<IGetGameModePreferencesRequest>();
				TaskService<GameModePreferences> task = new TaskService<GameModePreferences>(request);
				yield return new HandleTaskServiceWithError(task, delegate
				{
					lobbyView.ShowLoadingScreen();
				}, delegate
				{
					lobbyView.HideLoadingScreen();
				}).GetEnumerator();
				if (!task.succeeded)
				{
					OnFailedToJoinMatchmakingQueue(task.behaviour);
					yield break;
				}
				gameModePreferences = task.result;
			}
			EnterMatchmakingQueueDependency dependency = new EnterMatchmakingQueueDependency(_platoon, GarageSlot, gameMode, PlatoonSize, _platoon.GetIsPlatoonLeader(), brawlVersionNum, gameModePreferences);
			IEnterMatchmakingQueueRequest enterMatchmakingQueueRequest = lobbyRequestFactory.Create<IEnterMatchmakingQueueRequest, EnterMatchmakingQueueDependency>(dependency);
			TaskService<int> enterQueueTask = new TaskService<int>(enterMatchmakingQueueRequest);
			yield return new HandleTaskServiceWithError(enterQueueTask, delegate
			{
				lobbyView.ShowLoadingScreen();
			}, delegate
			{
				lobbyView.HideLoadingScreen();
			}).GetEnumerator();
			if (!enterQueueTask.succeeded)
			{
				OnFailedToJoinMatchmakingQueue(enterQueueTask.behaviour);
				yield break;
			}
			int estimatedWaitTime = enterQueueTask.result;
			Console.Log("Joined matchmaking queue");
			if (_leavingQueue)
			{
				yield return Break.It;
			}
			battleCountdownScreenController.ChangeStateToJoinedLobby();
			battleCountdownScreenController.SetEstimatedQueueTime(estimatedWaitTime);
			if (this.EnteredQueueEvent != null)
			{
				this.EnteredQueueEvent();
			}
			_isInQueue = true;
			LocalPlayerQueueStateChanged(_isInQueue, PlatoonMember.MemberStatus.InQueue);
			yield return UpdatePlayerStatusForCustomGame(CustomGamePlayerSessionStatus.Queuing);
		}

		private void OnStartConnectionTestReceived(Host host, NetworkConfig networkConfig)
		{
			Console.Log("Starting test connection (without encryption)");
			string hostAddress = host.hostAddress;
			if (string.IsNullOrEmpty(hostAddress))
			{
				string text = "Received invalid server address from lobby for connection test.";
				if (hostAddress != null)
				{
					Console.LogError($"{text} Address was: {hostAddress}");
				}
				else
				{
					Console.LogError($"{text} Address was null");
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("address", hostAddress);
				Dictionary<string, string> data = dictionary;
				RemoteLogger.Error(text, null, null, data);
			}
			byte[] encryptionParams = new byte[2];
			TestConnectionParameters dependency = new TestConnectionParameters(hostAddress, host.hostPort, networkConfig, encryptionParams);
			commandFactory.Build<StartConnectionTest>().Inject(dependency).Execute();
		}

		private void LeaveMatchmakingQueue(Action continueWith)
		{
			Console.Log("Leaving matchmaking queue");
			lobbyView.ShowLoadingScreen();
			_leavingQueue = true;
			lobbyRequestFactory.Create<ILeaveMatchmakingQueueRequest>().Execute();
			OnLeftMatchmaking(continueWith);
			lobbyView.HideLoadingScreen();
		}

		public void SetAfkWarningFlag(bool hasSeenWarning)
		{
			PlayerPrefs.SetInt("afkWarningShown", hasSeenWarning ? 1 : 0);
		}

		public bool GetAfkWarningFlag()
		{
			return PlayerPrefs.GetInt("afkWarningShown") == 1;
		}

		internal void GameGuidValidated()
		{
			_gameGuidValidated = true;
		}

		internal void DisconnectedFromGameServer()
		{
			Console.LogError("Disconnected from game server before entering game");
			_errorConnectingToGame = true;
		}

		internal void ReceivedGameServerWarning()
		{
			_errorConnectingToGame = true;
			_gameServerRepliedWithError = true;
		}

		internal void FailedToConnectToGameServer()
		{
			Console.LogError("Failed to connect to game server before entering game");
			_errorConnectingToGame = true;
		}

		internal void GameServerConnectionLost()
		{
			Console.LogError("Game server connection lost before entering game");
			_errorConnectingToGame = true;
		}

		private void OnPlatoonRobotTierChanged(PlatoonRobotTierEventData platoonRobotTierData)
		{
			if (_isInQueue && _platoon != null && !_platoon.GetIsPlatoonLeader())
			{
				string userName = platoonRobotTierData.UserName;
				if (userName == _platoon.leader)
				{
					string windowTitle = Localization.Get("strPlatoonLeaderChangedRobotTitle", true);
					string windowBody = Localization.Get("strPlatoonLeaderChangedRobotBody", true);
					LeaveMatchmakingQueue(delegate
					{
						ErrorWindow.ShowErrorWindow(new GenericErrorData(windowTitle, windowBody));
					});
				}
			}
		}

		private void PlatoonChanged(Platoon platoon)
		{
			if (_isInQueue)
			{
				if (platoon.isInPlatoon)
				{
					if (platoon.GetIsPlatoonLeader())
					{
						lobbyRequestFactory.Create<ISendPlatoonUpdateRequest, Platoon>(platoon).Execute();
					}
				}
				else
				{
					string windowTitle;
					string windowBody;
					if (_platoon.GetIsPlatoonLeader())
					{
						windowTitle = Localization.Get("strPlatoonDisbandedTitle", true);
						windowBody = Localization.Get("strPlatoonDisbandedBody", true);
					}
					else
					{
						windowTitle = Localization.Get("strRemovedFromPlatoonTitle", true);
						windowBody = Localization.Get("strRemoverFromPlatoonBody", true);
					}
					LeaveMatchmakingQueue(delegate
					{
						ErrorWindow.ShowErrorWindow(new GenericErrorData(windowTitle, windowBody));
					});
				}
			}
			_platoon = platoon;
		}

		private void OnBattleFound()
		{
			Console.Log("Battle found");
			battleCountdownScreenController.GameFound();
		}

		private void OnBattleCancelled()
		{
			Console.Log("Battle cancelled");
			battleCountdownScreenController.GameCancelled();
		}

		internal void SendConnectionTestResult(bool result)
		{
			IConnectionTestResultRequest connectionTestResultRequest = lobbyRequestFactory.Create<IConnectionTestResultRequest>();
			connectionTestResultRequest.Inject(result);
			connectionTestResultRequest.SetAnswer(new ServiceAnswer(OnSendConnectionTestResultFailed));
			connectionTestResultRequest.Execute();
		}

		private void OnSendConnectionTestResultFailed(ServiceBehaviour serviceBehaviour)
		{
			lobbyRequestFactory.Create<ILeaveMatchmakingQueueRequest>().Execute();
			RemoteLogger.Error(serviceBehaviour.exceptionThrown);
			LeftQueue();
			guiInputController.CloseCurrentScreen();
			ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
		}

		private void OnBattleStart(EnterBattleDependency enterBattleData)
		{
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			Console.Log("OnBattleStart called");
			if (_lobbyEventContainer != null)
			{
				_lobbyEventContainer.DisconnectedEvent -= OnDisconnectedFromLobby;
			}
			_gameGuidValidated = false;
			_errorConnectingToGame = false;
			_leavingQueue = true;
			_isInQueue = false;
			LocalPlayerQueueStateChanged(_isInQueue, PlatoonMember.MemberStatus.InBattle);
			ConnectToGameServerCommand connectToGameServerCommand = commandFactory.Build<ConnectToGameServerCommand>();
			connectToGameServerCommand.SetValues(enterBattleData.BattleParameters.HostIP, enterBattleData.BattleParameters.HostPort, enterBattleData.BattleParameters.NetworkConfigs);
			connectToGameServerCommand.Execute();
			battlePlayers.SetExpectedPlayersForMatches(enterBattleData.Players);
			TaskRunner.get_Instance().Run(SwitchToBattle(enterBattleData));
		}

		private IEnumerator SwitchToBattle(EnterBattleDependency enterBattleData)
		{
			LoadingIconPresenter.NotifyLoading("EnterBattle");
			battleFoundObserver.BattleFound(enterBattleData);
			lobbyRequestFactory.Create<ILeaveMatchmakingQueueRequest>().Execute();
			while (!_gameGuidValidated)
			{
				if (_errorConnectingToGame)
				{
					commandFactory.Build<ErrorConnectingToGameServerCommand>().Execute();
					LoadingIconPresenter.NotifyLoadingDone("EnterBattle");
					if (_gameServerRepliedWithError)
					{
						_gameServerRepliedWithError = false;
					}
					else
					{
						commandFactory.Build<DisplayFailedToConnectToServerInfoCommand>().Inject(StringTableBase<StringTable>.Instance.GetString("strConnectServerFail")).Execute();
					}
					yield return Break.It;
				}
				yield return null;
			}
			ILogPlayerEnteredGameRequest service = analyticsRequestFactory.Create<ILogPlayerEnteredGameRequest, EnterBattleDependency>(enterBattleData);
			TaskService task = new TaskService(service);
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Game entered request failed to send " + task.behaviour.errorBody);
			}
			yield return UpdatePlayerStatusForCustomGame(CustomGamePlayerSessionStatus.InBattle);
			guiInputController.CloseCurrentScreen();
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			battleFoundObserver.EnterBattle(enterBattleData);
			_gameStarted = true;
			string mapName = enterBattleData.BattleParameters.MapName;
			GameModeKey gameModeKey = enterBattleData.BattleParameters.GameModeKey;
			bool isRanked = gameModeKey.IsRanked;
			GameModeKey gameModeKey2 = enterBattleData.BattleParameters.GameModeKey;
			bool isBrawl = gameModeKey2.IsBrawl;
			GameModeKey gameModeKey3 = enterBattleData.BattleParameters.GameModeKey;
			bool isCustomGame = gameModeKey3.IsCustomGame;
			GameModeKey gameModeKey4 = enterBattleData.BattleParameters.GameModeKey;
			SwitchWorldDependency switchWorldDependency = new SwitchWorldDependency(mapName, isRanked, isBrawl, isCustomGame, gameModeKey4.type);
			yield return CustomGameOverridesPreloader.PreloadCustomGameOverrides(LoadingIconPresenter);
			LoadingIconPresenter.NotifyLoadingDone("EnterBattle");
			commandFactory.Build<SwitchToMultiplayerPlanetCommand>().Inject(switchWorldDependency).Execute();
		}

		private IEnumerator UpdatePlayerStatusForCustomGame(CustomGamePlayerSessionStatus status)
		{
			ICustomGamePlayerStateChangedRequest updateStateRequest = serviceRequestFactory.Create<ICustomGamePlayerStateChangedRequest>();
			updateStateRequest.Inject(status);
			TaskService updateStateTask = new TaskService(updateStateRequest);
			yield return updateStateTask;
			if (!updateStateTask.succeeded)
			{
				RemoteLogger.Error(updateStateTask.behaviour.exceptionThrown);
			}
		}

		private void LocalPlayerQueueStateChanged(bool inQueue, PlatoonMember.MemberStatus memberStatus)
		{
			GetPlatoonData(delegate(Platoon platoon)
			{
				if (platoon.isInPlatoon)
				{
					SetPlatoonMemberStatusDependency param = new SetPlatoonMemberStatusDependency(User.Username, memberStatus);
					ISetPlatoonMemberStatusRequest setPlatoonMemberStatusRequest = socialRequestFactory.Create<ISetPlatoonMemberStatusRequest, SetPlatoonMemberStatusDependency>(param);
					ServiceAnswer answer = new ServiceAnswer(LogServiceFailed);
					IServiceRequest serviceRequest = setPlatoonMemberStatusRequest.SetAnswer(answer);
					serviceRequest.Execute();
				}
			});
		}

		private void GetPlatoonData(Action<Platoon> onGetData)
		{
			socialRequestFactory.Create<IGetPlatoonDataRequest>().SetAnswer(new ServiceAnswer<Platoon>(onGetData, LogServiceFailed)).Execute();
		}

		private void LogServiceFailed(ServiceBehaviour behaviour)
		{
			RemoteLogger.Error(behaviour.exceptionThrown);
		}

		private void OnBattleFoundMessageFailed(Exception exception)
		{
			RemoteLogger.Error(new Exception("Corrupted battle found event " + exception));
			LeftQueue();
			guiInputController.CloseCurrentScreen();
			ErrorWindow.ShowErrorWindow(new GenericErrorData(Localization.Get("strLobbyError", true), Localization.Get("strFailedToJoinBattle", true)));
		}

		private void OnFailedToJoinMatchmakingQueue(ServiceBehaviour serviceBehaviour)
		{
			RemoteLogger.Error("Error received from enter queue request", serviceBehaviour.exceptionThrown.ToString(), serviceBehaviour.exceptionThrown.StackTrace);
			lobbyView.HideLoadingScreen();
			battleCountdownScreenController.ChangeStateToExited();
			guiInputController.CloseCurrentScreen();
		}

		private void OnGroupFailedToJoinMatchmakingQueue(LobbyReasonCode reason)
		{
			lobbyView.HideLoadingScreen();
			_leavingQueue = true;
			LeftQueue();
			GroupErrorWindow("strLobbyError", reason);
		}

		private void OnGroupErrorDuringMatchmakingQueue(LobbyReasonCode reason)
		{
			_leavingQueue = true;
			LeftQueue();
			GroupErrorWindow("strLobbyError", reason);
		}

		private void OnGroupFailedToJoinBattle(LobbyReasonCode reason)
		{
			_leavingQueue = true;
			LeftQueue();
			GroupErrorWindow("strFailedToJoinBattle", reason);
		}

		private void GroupErrorWindow(string errorTitle, LobbyReasonCode reason)
		{
			ErrorWindow.ShowErrorWindow(new GenericErrorData(Localization.Get(errorTitle, true), Localization.Get(StringTableBase<StringTable>.Instance.GetString(reason.ToString()), true), Localization.Get("strOK", true), delegate
			{
				guiInputController.CloseCurrentScreen();
			}));
		}

		private void OnLeftMatchmaking(Action continueWith)
		{
			LeftQueue();
			continueWith?.Invoke();
		}

		private void OnDisconnectedFromLobby()
		{
			_lobbyEventContainer.DisconnectedEvent -= OnDisconnectedFromLobby;
			LeftQueue();
			if (!_leavingQueue)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(Localization.Get("strLobbyError", true), Localization.Get("strDisconnected", true)));
			}
		}

		private void LeftQueue()
		{
			if (!_gameStarted)
			{
				TaskRunner.get_Instance().Run(UpdatePlayerStatusForCustomGame(CustomGamePlayerSessionStatus.Ready));
			}
			_isInQueue = false;
			if (_lobbyEventContainer != null)
			{
				_lobbyEventContainer.Dispose();
				_lobbyEventContainer = null;
			}
			if (this.LeftQueueEvent != null)
			{
				this.LeftQueueEvent();
			}
			LocalPlayerQueueStateChanged(inQueue: false, PlatoonMember.MemberStatus.Ready);
		}
	}
}
