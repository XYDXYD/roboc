using Authentication;
using CustomGames;
using Fabric;
using ServerStateServiceLayer;
using ServerStateServiceLayer.EventListeners.Photon;
using Services.Web.Photon;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership.GUI.CustomGames
{
	internal sealed class CustomGamePartyGUIController : IInitialize, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private CustomGameTeamController _registeredTeamA;

		private CustomGameTeamController _registeredTeamB;

		private CustomGamePartyGUIView _view;

		private IServiceEventContainer _serverStateEventContainer;

		private int _currentRobotCPU;

		private int _currentRobotRanking;

		private uint _maxRegularCPU;

		private TiersData _tiersData;

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController guiInputController
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
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IServerStateEventContainerFactory serverStateEventContainerFactory
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
		internal CustomGameStateObservable customGameStateObservable
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameGameModeObserver gameModeChangeObserver
		{
			private get;
			set;
		}

		[Inject]
		internal GarageChangedObserver garageChangedObserver
		{
			get;
			set;
		}

		[Inject]
		internal SetBuildModeHintsAnchorsObserverable setBuildModeHintsObservable
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		public unsafe void OnDependenciesInjected()
		{
			guiInputController.OnScreenStateChange += delegate
			{
				ShowCustomGamePartyGUIIfRelevant();
			};
			gameModeChangeObserver.AddAction(new ObserverAction<GameModeType>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			BindEventListeners();
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			garageChangedObserver.RemoveAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			machineMap.OnAddCubeAt -= OnCubeAdded;
			machineMap.OnRemoveCubeAt -= OnCubeRemoved;
		}

		private IEnumerator LoadDependantDataForTiers()
		{
			loadingIconPresenter.NotifyLoading("LoadingTiersData");
			ILoadTiersBandingRequest loadTiersBandingReq = serviceFactory.Create<ILoadTiersBandingRequest>();
			TaskService<TiersData> loadTiersBandingTS = new TaskService<TiersData>(loadTiersBandingReq);
			HandleTaskServiceWithError handleTSWithError2 = new HandleTaskServiceWithError(loadTiersBandingTS, delegate
			{
				loadingIconPresenter.NotifyLoading("LoadingTiersData");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("LoadingTiersData");
			});
			yield return handleTSWithError2.GetEnumerator();
			if (loadTiersBandingTS.succeeded)
			{
				_tiersData = loadTiersBandingTS.result;
			}
			loadingIconPresenter.NotifyLoadingDone("LoadingTiersData");
			loadingIconPresenter.NotifyLoading("LoadingCPUSettings");
			ILoadCpuSettingsRequest loadCpuSettingsReq = serviceFactory.Create<ILoadCpuSettingsRequest>();
			TaskService<CPUSettingsDependency> loadCpuSettingsTS = new TaskService<CPUSettingsDependency>(loadCpuSettingsReq);
			handleTSWithError2 = new HandleTaskServiceWithError(loadCpuSettingsTS, delegate
			{
				loadingIconPresenter.NotifyLoading("LoadingCPUSettings");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("LoadingCPUSettings");
			});
			yield return handleTSWithError2.GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("LoadingCPUSettings");
			if (loadCpuSettingsTS.succeeded)
			{
				_maxRegularCPU = Convert.ToUInt32(loadCpuSettingsTS.result.maxRegularCpu);
			}
		}

		private void HandleCustomGameGameModeChanged(ref GameModeType newGameMode)
		{
			ShowCustomGamePartyGUIIfRelevant(refreshNeeded: true);
		}

		public void SetTeamControllers(CustomGameTeamController teamA, CustomGameTeamController teamB)
		{
			_registeredTeamA = teamA;
			_registeredTeamB = teamB;
		}

		private void BindEventListeners()
		{
			_serverStateEventContainer = serverStateEventContainerFactory.Create();
			_serverStateEventContainer.ListenTo<ICustomGameInvitationEventListener, CustomGameInvitationData>(OnCustomGameInvitationReceivedEvent);
			_serverStateEventContainer.ListenTo<ICustomGameRefreshEventListener, string>(OnCustomGameRefreshRequiredEvent);
			_serverStateEventContainer.ListenTo<ICustomGameKickedFromSessionEventListener, KickedFromCustomGameSessionData>(OnKickedFromCustomGameEvent);
			_serverStateEventContainer.ListenTo<ICustomGameLeaderChangedEventListener, string>(OnCustomGameLeaderChangedEvent);
			_serverStateEventContainer.ListenTo<ICustomGameRobotTierChangedEventListener, CustomGameRobotTierChangedEventData>(OnCustomGameRobotTierChangedEvent);
		}

		private void OnCustomGameLeaderChangedEvent(string newLeader)
		{
			UpdatePlayerInteractivityStatus(newLeader);
		}

		public IEnumerator LoadGUIData()
		{
			loadingIconPresenter.NotifyLoading("CheckWasInvited");
			ICheckIfHasBeenInvitedToCustomGameSessionRequest checkHasBeenInvitedRequest = serviceFactory.Create<ICheckIfHasBeenInvitedToCustomGameSessionRequest>();
			checkHasBeenInvitedRequest.ClearCache();
			TaskService<CheckIfHasBeenInvitedToCustomGameSessionRequestResponse> taskService = new TaskService<CheckIfHasBeenInvitedToCustomGameSessionRequestResponse>(checkHasBeenInvitedRequest);
			yield return new HandleTaskServiceWithError(taskService, delegate
			{
				loadingIconPresenter.NotifyLoading("CheckWasInvited");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CheckWasInvited");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("CheckWasInvited");
			if (taskService.succeeded && taskService.result.ResponseCode == CheckIfHasBeenInvitedToCustomGameResponseCode.HasPendingCustomGameSessionInvitation)
			{
				AvatarInfo avatarInfo = taskService.result.ResponseData.AvatarInfo;
				string senderName = taskService.result.ResponseData.SenderName;
				string senderDisplayName = taskService.result.ResponseData.SenderDisplayName;
				Console.Log("initial flow detects: user has received an invitation to a session");
				PartyInvitationReceivedMessage message = new PartyInvitationReceivedMessage(senderName, senderDisplayName, avatarInfo);
				_view.DeepBroadcast(message);
				ShowPartyInvitationDialogMessage message2 = new ShowPartyInvitationDialogMessage();
				_view.DeepBroadcast(message2);
			}
		}

		private void OnCustomGameRobotTierChangedEvent(CustomGameRobotTierChangedEventData data)
		{
			Console.Log("Robot Tier Change detected from " + data.TargetUsername + " new tier is " + data.NewTier);
			_view.DeepBroadcast(new CustomGameTierChangedMessage(data.TargetUsername));
		}

		private void OnKickedFromCustomGameEvent(KickedFromCustomGameSessionData data)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0, (object)null, null);
			UpdatePlayerInteractivityStatus(User.Username);
			HidePartyInvitationDialogMessage message = new HidePartyInvitationDialogMessage();
			_view.DeepBroadcast(message);
		}

		private void OnCustomGameInvitationReceivedEvent(CustomGameInvitationData invitationData)
		{
			Console.Log("user has received an invitation to a session.");
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[141], 0, (object)null, _view.get_gameObject());
			PartyInvitationReceivedMessage message = new PartyInvitationReceivedMessage(invitationData.InviterName, invitationData.DisplayName, invitationData.AvatarInfo);
			ShowPartyInvitationDialogMessage message2 = new ShowPartyInvitationDialogMessage();
			_view.DeepBroadcast(message);
			_view.DeepBroadcast(message2);
			ShowCustomGamePartyGUIIfRelevant();
		}

		private void OnCustomGameRefreshRequiredEvent(string sessionInfo)
		{
			Console.Log("Custom game refresh required for sesssion: " + sessionInfo);
			ShowCustomGamePartyGUIIfRelevant(refreshNeeded: true);
		}

		public void RegisterToContextNotifier(IContextNotifer contextNotifier)
		{
			contextNotifier.AddFrameworkInitializationListener(this);
		}

		public void SetView(CustomGamePartyGUIView view)
		{
			_view = view;
		}

		public unsafe void OnFrameworkInitialized()
		{
			_view.BuildSignal();
			TaskRunner.get_Instance().Run(LoadDependantDataForTiers());
			garageChangedObserver.AddAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			machineMap.OnAddCubeAt += OnCubeAdded;
			machineMap.OnRemoveCubeAt += OnCubeRemoved;
		}

		private void OnCubeAdded(Byte3 gridLoc, MachineCell machineCell)
		{
			_currentRobotCPU += (int)machineCell.info.persistentCubeData.cpuRating;
			_currentRobotRanking += machineCell.info.persistentCubeData.cubeRanking;
			bool isMegabot = _currentRobotCPU > cpuPower.MaxCpuPower;
			uint num = RRAndTiers.ConvertRRToTierIndex((uint)_currentRobotRanking, isMegabot, _tiersData);
			string tierString_ = RRAndTiers.ConvertTierIndexToTierString(num, _tiersData);
			_view.DeepBroadcast(new CustomGameTierChangedMessage((int)num, User.Username, tierString_));
		}

		private void OnCubeRemoved(Byte3 gridLoc, MachineCell machineCell)
		{
			_currentRobotCPU -= (int)machineCell.info.persistentCubeData.cpuRating;
			_currentRobotRanking -= machineCell.info.persistentCubeData.cubeRanking;
			bool isMegabot = _currentRobotCPU > cpuPower.MaxCpuPower;
			uint num = RRAndTiers.ConvertRRToTierIndex((uint)_currentRobotRanking, isMegabot, _tiersData);
			string tierString_ = RRAndTiers.ConvertTierIndexToTierString(num, _tiersData);
			_view.DeepBroadcast(new CustomGameTierChangedMessage((int)num, User.Username, tierString_));
		}

		private void OnGarageSlotChanged(ref GarageSlotDependency dependancy)
		{
			uint num = RobotCPUCalculator.CalculateRobotActualCPU(dependancy.totalRobotCPU, dependancy.totalCosmeticCPU, cpuPower.MaxCosmeticCpuPool);
			bool isMegabot = num > cpuPower.MaxCpuPower;
			uint num2 = RRAndTiers.ConvertRRToTierIndex(dependancy.totalRobotRanking, isMegabot, _tiersData);
			string tierString_ = RRAndTiers.ConvertTierIndexToTierString(num2, _tiersData);
			_view.DeepBroadcast(new CustomGameTierChangedMessage((int)num2, User.Username, tierString_));
			commandFactory.Build<CustomGameRobotTierChangedCommand>().Inject(num2).Execute();
		}

		private void ShowCustomGamePartyGUIIfRelevant(bool refreshNeeded = false)
		{
			switch (guiInputController.GetActiveScreen())
			{
			case GuiScreens.CustomGameScreen:
				TaskRunner.get_Instance().Run(RefreshDefaultStyle());
				_view.ShowTeamsPanel();
				_view.DeepBroadcast(new CustomGameTeamAssignmentsChangedMessage());
				_view.DeepBroadcast(new HidePartyInvitationDialogMessage());
				TaskRunner.get_Instance().Run(RefreshPartyWaitingStatusMessage());
				break;
			case GuiScreens.Garage:
				TaskRunner.get_Instance().Run(ApplyCorrectStyleToView());
				_view.DeepBroadcast(new HidePartyPopupMenuMessage());
				TaskRunner.get_Instance().Run(ShowOrHideInvitationDialogIfInvitedToSession());
				TaskRunner.get_Instance().Run(ShowCustomGamePartyGuiIfInParty(refreshNeeded));
				break;
			case GuiScreens.BattleCountdown:
				TaskRunner.get_Instance().Run(ApplyCorrectStyleToView());
				TaskRunner.get_Instance().Run(ShowCustomGamePartyGuiIfInParty(refreshNeeded));
				break;
			case GuiScreens.BuildMode:
				TaskRunner.get_Instance().Run(ApplyCorrectStyleToView());
				TaskRunner.get_Instance().Run(ShowOrHideInvitationDialogIfInvitedToSession());
				TaskRunner.get_Instance().Run(ShowCustomGamePartyGuiIfInParty(refreshNeeded));
				break;
			default:
				_view.HideTeamsPanel();
				_view.DeepBroadcast(new HidePartyPopupMenuMessage());
				_view.DeepBroadcast(new HidePartyInvitationDialogMessage());
				break;
			}
		}

		private IEnumerator ShowOrHideInvitationDialogIfInvitedToSession()
		{
			ICheckIfHasBeenInvitedToCustomGameSessionRequest checkHasBeenInvitedRequest = serviceFactory.Create<ICheckIfHasBeenInvitedToCustomGameSessionRequest>();
			checkHasBeenInvitedRequest.ClearCache();
			TaskService<CheckIfHasBeenInvitedToCustomGameSessionRequestResponse> taskService = new TaskService<CheckIfHasBeenInvitedToCustomGameSessionRequestResponse>(checkHasBeenInvitedRequest);
			yield return taskService;
			if (taskService.succeeded)
			{
				if (taskService.result.ResponseCode == CheckIfHasBeenInvitedToCustomGameResponseCode.HasPendingCustomGameSessionInvitation)
				{
					AvatarInfo avatarInfo = taskService.result.ResponseData.AvatarInfo;
					string senderName = taskService.result.ResponseData.SenderName;
					string senderDisplayName = taskService.result.ResponseData.SenderDisplayName;
					Console.Log("initial flow detects: user has received an invitation to a session");
					PartyInvitationReceivedMessage message = new PartyInvitationReceivedMessage(senderName, senderDisplayName, avatarInfo);
					_view.DeepBroadcast(message);
					ShowPartyInvitationDialogMessage message2 = new ShowPartyInvitationDialogMessage();
					_view.DeepBroadcast(message2);
				}
				else
				{
					HidePartyInvitationDialogMessage message3 = new HidePartyInvitationDialogMessage();
					_view.DeepBroadcast(message3);
				}
			}
		}

		private void UpdatePlayerInteractivityStatus(string sessionLeader)
		{
			GuiScreens activeScreen = guiInputController.GetActiveScreen();
			string username = User.Username;
			bool flag = username == sessionLeader;
			if (activeScreen == GuiScreens.BattleCountdown)
			{
				_view.DragAndDropTextIsAvailable(availabilitySetting: false);
			}
			else
			{
				_view.DragAndDropTextIsAvailable(flag);
			}
			_view.DeepBroadcast(new TeamLeaderChangedMessage(flag));
		}

		private IEnumerator ShowCustomGamePartyGuiIfInParty(bool cacheRequiresClear)
		{
			IRetrieveCustomGameSessionRequest refreshSessionRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			if (cacheRequiresClear)
			{
				refreshSessionRequest.ClearCache();
			}
			TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionRequest);
			yield return refreshTask;
			if (!refreshTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(refreshTask.behaviour);
				yield break;
			}
			if (cacheRequiresClear)
			{
				yield return RefreshPartyWaitingStatusMessage();
			}
			if (refreshTask.result.Data == null)
			{
				_view.HideTeamsPanel();
				yield break;
			}
			UpdatePlayerInteractivityStatus(refreshTask.result.Data.SessionLeader);
			_view.ShowTeamsPanel();
			_view.DeepBroadcast(new CustomGameTeamAssignmentsChangedMessage());
		}

		private IEnumerator RefreshDefaultStyle()
		{
			yield return ApplyCorrectStyleToView();
		}

		private IEnumerator ApplyCorrectStyleToView()
		{
			GuiScreens activeScreen = guiInputController.GetActiveScreen();
			CustomGamePartyGUIView.StyleVersion styleVersion = CustomGamePartyGUIView.StyleVersion.AllOtherLocations;
			switch (activeScreen)
			{
			case GuiScreens.CustomGameScreen:
				styleVersion = CustomGamePartyGUIView.StyleVersion.CustomGameScreen;
				break;
			case GuiScreens.Garage:
				styleVersion = CustomGamePartyGUIView.StyleVersion.Garage;
				break;
			case GuiScreens.BuildMode:
				styleVersion = CustomGamePartyGUIView.StyleVersion.EditMode;
				break;
			}
			BuildModeAnchors anchors = new BuildModeAnchors();
			int numTeams = 2;
			IRetrieveCustomGameSessionRequest refreshSessionRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			refreshSessionRequest.ClearCache();
			TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionRequest);
			yield return refreshTask;
			if (refreshTask.succeeded && refreshTask.result.Data != null)
			{
				CustomGameSessionData session = refreshTask.result.Data;
				if (session != null)
				{
					string value = session.Config["GameMode"];
					GameModeType gameModeType = (GameModeType)Enum.Parse(typeof(GameModeType), value);
					if (gameModeType == GameModeType.Pit)
					{
						numTeams = 1;
					}
				}
				_view.ApplyStyle(numTeams, styleVersion);
				yield return (object)new WaitForEndOfFrame();
				anchors = new BuildModeAnchors(_view.TeamAUIWidget, _view.TeamBUIWidget);
			}
			setBuildModeHintsObservable.Dispatch(ref anchors);
		}

		private IEnumerator RefreshPartyWaitingStatusMessage()
		{
			bool showPartyIsWaitingMessage = false;
			GuiScreens activeScreen = guiInputController.GetActiveScreen();
			if (activeScreen != GuiScreens.BattleCountdown)
			{
				IRetrieveCustomGameSessionRequest refreshSessionRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
				TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionRequest);
				yield return refreshTask;
				if (refreshTask.succeeded && refreshTask.result.Data != null)
				{
					string sessionLeader = refreshTask.result.Data.SessionLeader;
					PlatoonMember.MemberStatus memberStatus = refreshTask.result.Data.MemberStatus[sessionLeader];
					if (memberStatus == PlatoonMember.MemberStatus.InQueue)
					{
						showPartyIsWaitingMessage = true;
					}
				}
			}
			_view.SetYourPartyIsWaitingIndicatorVisibility(showPartyIsWaitingMessage);
		}

		public void HandleMessage(object message)
		{
			if (message is ShowPartyPopupMenuMessage)
			{
				ShowPartyPopupMenuMessage showPartyPopupMenuMessage = (ShowPartyPopupMenuMessage)message;
				if (!showPartyPopupMenuMessage.rebroadcastFromRoot)
				{
					showPartyPopupMenuMessage.rebroadcastFromRoot = true;
					_view.DeepBroadcast(message);
				}
			}
			else if (message is ShowPartyInviteDropDownMessageForCustomGame)
			{
				TaskRunner.get_Instance().Run(RejectOrShowPartyInviteDropDownIfNotLeader((ShowPartyInviteDropDownMessageForCustomGame)message));
			}
			else if (message is PartyPopupMenuClickMessage)
			{
				PartyPopupMenuClickMessage partyPopupMenuClickMessage = (PartyPopupMenuClickMessage)message;
				if (partyPopupMenuClickMessage.partyMamberName == User.Username)
				{
					if (partyPopupMenuClickMessage.itemClicked == PartyPopupMenuItems.LeaveParty)
					{
						_view.HideTeamsPanel();
						commandFactory.Build<LeaveCustomGameCommand>().Execute();
					}
				}
				else
				{
					TaskRunner.get_Instance().Run(KickSomeoneOrCancelInviteHandler(partyPopupMenuClickMessage.partyMamberName));
				}
			}
			else if (message is SendInviteToPartyMessage)
			{
				SendInviteToPartyMessage sendInviteToPartyMessage = (SendInviteToPartyMessage)message;
				TaskRunner.get_Instance().Run(SendInviteToCustomGameHandler(sendInviteToPartyMessage.invitee, sendInviteToPartyMessage.InvitationPartyType == InvitationToPartyType.CustomGameTeamA));
			}
			else if (message is PartyInvitationResponseMessage)
			{
				if (((PartyInvitationResponseMessage)message).acceptInvitation)
				{
					TaskRunner.get_Instance().Run(AcceptInvitation());
				}
				else
				{
					TaskRunner.get_Instance().Run(DeclineInvitation());
				}
			}
			else
			{
				if (!(message is UserRequestsTeamAssignmentChangeMessage))
				{
					return;
				}
				UserRequestsTeamAssignmentChangeMessage userRequestsTeamAssignmentChangeMessage = (UserRequestsTeamAssignmentChangeMessage)message;
				bool movingFromTeamA = userRequestsTeamAssignmentChangeMessage.MovingFromTeamA;
				string destinationPlayer = null;
				string memberNameForSlotIndex;
				if (movingFromTeamA)
				{
					memberNameForSlotIndex = _registeredTeamA.GetMemberNameForSlotIndex(userRequestsTeamAssignmentChangeMessage.SourceSlotIndex);
					if (userRequestsTeamAssignmentChangeMessage.DestinationSlotIndex < _registeredTeamB.TeamSize)
					{
						destinationPlayer = _registeredTeamB.GetMemberNameForSlotIndex(userRequestsTeamAssignmentChangeMessage.DestinationSlotIndex);
					}
				}
				else
				{
					memberNameForSlotIndex = _registeredTeamB.GetMemberNameForSlotIndex(userRequestsTeamAssignmentChangeMessage.SourceSlotIndex);
					if (userRequestsTeamAssignmentChangeMessage.DestinationSlotIndex < _registeredTeamA.TeamSize)
					{
						destinationPlayer = _registeredTeamA.GetMemberNameForSlotIndex(userRequestsTeamAssignmentChangeMessage.DestinationSlotIndex);
					}
				}
				TaskRunner.get_Instance().Run(ChangeTeamAssignments(memberNameForSlotIndex, destinationPlayer, movingFromTeamA));
			}
		}

		private IEnumerator RejectOrShowPartyInviteDropDownIfNotLeader(ShowPartyInviteDropDownMessageForCustomGame inviteMessage)
		{
			IRetrieveCustomGameSessionRequest refreshSessionRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionRequest);
			yield return refreshTask;
			if (!refreshTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(refreshTask.behaviour);
			}
			else if (refreshTask.result.Data.SessionLeader != User.Username)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameInviteMemberErrorHeader"), StringTableBase<StringTable>.Instance.GetString("strCustomGameInviteMemberErrorBody"), StringTableBase<StringTable>.Instance.GetString("strOK")));
			}
			else if (!inviteMessage.rebroadcastFromRoot)
			{
				inviteMessage.rebroadcastFromRoot = true;
				_view.DeepBroadcast(inviteMessage);
			}
		}

		private IEnumerator ChangeTeamAssignments(string sourcePlayer, string destinationPlayer, bool destinationIsTeamB)
		{
			loadingIconPresenter.NotifyLoading("SwapCustomGame");
			IChangeTeamAssignmentRequest assignRequest = serviceFactory.Create<IChangeTeamAssignmentRequest>();
			assignRequest.Inject(new ChangeCustomGameTeamAssignmentDependancy(sourcePlayer, destinationPlayer, destinationIsTeamB));
			TaskService<ChangeCustomGameTeamAssignmentResponse> task = new TaskService<ChangeCustomGameTeamAssignmentResponse>(assignRequest);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingIconPresenter.NotifyLoading("SwapCustomGame");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("SwapCustomGame");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("SwapCustomGame");
			if (task.result == ChangeCustomGameTeamAssignmentResponse.UserIsNotSessionLeader)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameOnlyLeaderDragAndDropTitle"), StringTableBase<StringTable>.Instance.GetString("strCustomGameOnlyLeaderDragAndDropBody"), StringTableBase<StringTable>.Instance.GetString("strOK")));
			}
			if (task.result == ChangeCustomGameTeamAssignmentResponse.DestinationTeamAlreadyFull || task.result == ChangeCustomGameTeamAssignmentResponse.SessionNoLongerExists || task.result == ChangeCustomGameTeamAssignmentResponse.SourceOrTargetPlayerNotFoundInCorrectTeam || task.result == ChangeCustomGameTeamAssignmentResponse.TeamAssignmentChangeError)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameDragAndDropFailTitle"), StringTableBase<StringTable>.Instance.GetString("strCustomGameDragAndDropFailBody"), StringTableBase<StringTable>.Instance.GetString("strOK")));
			}
		}

		private IEnumerator KickSomeoneOrCancelInviteHandler(string memberName)
		{
			_view.Broadcast(new HidePopupMenuMessage());
			loadingIconPresenter.NotifyLoading("KickOrCancelFromCustomGame");
			IKickFromCustomGameRequest kickRequest = serviceFactory.Create<IKickFromCustomGameRequest>();
			kickRequest.Inject(new KickFromCustomGameRequestDependancy(memberName));
			TaskService<KickFromCustomGameResponse> kickUserTask = new TaskService<KickFromCustomGameResponse>(kickRequest);
			yield return kickUserTask;
			loadingIconPresenter.NotifyLoadingDone("KickOrCancelFromCustomGame");
			if (!kickUserTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(kickUserTask.behaviour);
			}
			else
			{
				switch (kickUserTask.result)
				{
				case KickFromCustomGameResponse.KickTargetIsNotInsession:
				case KickFromCustomGameResponse.UserRemovedFromSession:
					Console.Log("succesfully removed user from session, or they have already left the session.");
					EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0, (object)null, _view.get_gameObject());
					break;
				case KickFromCustomGameResponse.UserIsNotSessionLeader:
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGamesMessageHeader"), StringTableBase<StringTable>.Instance.GetString("strCustomGameKickUserResponseNotLeader"), StringTableBase<StringTable>.Instance.GetString("strOK")));
					break;
				case KickFromCustomGameResponse.SessionNoLongerExists:
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGamesMessageHeader"), StringTableBase<StringTable>.Instance.GetString("strCustomGameKickSessionDoesntExist"), StringTableBase<StringTable>.Instance.GetString("strOK")));
					if (guiInputController.GetActiveScreen() == GuiScreens.CustomGameScreen)
					{
						guiInputController.CloseCurrentScreen();
					}
					break;
				default:
					RemoteLogger.Error("Failed kicking user from custom game session", "target username: " + memberName + " task result:" + kickUserTask.result, null);
					break;
				}
				_view.Broadcast(new SendInviteToPartyResponse());
			}
			yield return null;
		}

		private IEnumerator HandleJoinedToCustomGameSessionSuccesfully()
		{
			Console.Log("Succesfully accepted invite to session.");
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[139], 0, (object)null, _view.get_gameObject());
			loadingIconPresenter.NotifyLoading("RespondToCustomGameInvitation");
			IRetrieveCustomGameSessionRequest refreshSessionRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			refreshSessionRequest.ClearCache();
			TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionRequest);
			yield return refreshTask;
			loadingIconPresenter.NotifyLoadingDone("RespondToCustomGameInvitation");
			if (refreshTask.succeeded && refreshTask.result.Data != null)
			{
				CustomGameStateDependency dep = new CustomGameStateDependency(refreshTask.result.Data.SessionGUID);
				customGameStateObservable.Dispatch(ref dep);
				yield return RefreshDefaultStyle();
				yield return RefreshPartyWaitingStatusMessage();
				UpdatePlayerInteractivityStatus(refreshTask.result.Data.SessionLeader);
			}
		}

		private IEnumerator AcceptInvitation()
		{
			if (guiInputController.GetActiveScreen() == GuiScreens.BattleCountdown)
			{
				guiInputController.CloseCurrentScreen();
			}
			loadingIconPresenter.NotifyLoading("RespondToCustomGameInvitation");
			IRespondToCustomGameInvitationRequest acceptInviteRequest = serviceFactory.Create<IRespondToCustomGameInvitationRequest>();
			acceptInviteRequest.Inject(new RespondToCustomGameInvitationDependancy(isAccept_: true));
			acceptInviteRequest.ClearCache();
			TaskService<ReplyToCustomGameInviteResponseCode> acceptInvitationTask = new TaskService<ReplyToCustomGameInviteResponseCode>(acceptInviteRequest);
			yield return acceptInvitationTask;
			loadingIconPresenter.NotifyLoadingDone("RespondToCustomGameInvitation");
			if (!acceptInvitationTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(acceptInvitationTask.behaviour);
			}
			else
			{
				yield return RefreshPartyWaitingStatusMessage();
				switch (acceptInvitationTask.result)
				{
				default:
					yield return HandleJoinedToCustomGameSessionSuccesfully();
					break;
				case ReplyToCustomGameInviteResponseCode.ReplyToResponseError:
					ErrorWindow.ShowServiceErrorWindow(acceptInvitationTask.behaviour);
					break;
				case ReplyToCustomGameInviteResponseCode.SessionDoesntExist:
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGamesMessageHeader"), StringTableBase<StringTable>.Instance.GetString("strCustomGameInviteReplyAcceptSessionDoesntExist"), StringTableBase<StringTable>.Instance.GetString("strOK")));
					break;
				case ReplyToCustomGameInviteResponseCode.UserCouldNotAcceptInviteAsNotInvited:
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGamesMessageHeader"), StringTableBase<StringTable>.Instance.GetString("strCustomGameInviteReplyNoLongerInvited"), StringTableBase<StringTable>.Instance.GetString("strOK")));
					break;
				case ReplyToCustomGameInviteResponseCode.DeclinedInvitationSuccesfully:
					break;
				}
			}
			_view.Broadcast(new HidePartyInvitationDialogMessage());
			loadingIconPresenter.NotifyLoading("ForceLeaveMultiplayerParty");
			IRetrieveCustomGameSessionRequest refreshSessionRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			refreshSessionRequest.ClearCache();
			TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionRequest);
			yield return new HandleTaskServiceWithError(refreshTask, delegate
			{
				loadingIconPresenter.NotifyLoading("ForceLeaveMultiplayerParty");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("ForceLeaveMultiplayerParty");
			}).GetEnumerator();
			ILeavePlatoonRequest leaveRequest = socialRequestFactory.Create<ILeavePlatoonRequest>();
			TaskService leaveTask = new TaskService(leaveRequest);
			yield return new HandleTaskServiceWithError(leaveTask, delegate
			{
				loadingIconPresenter.NotifyLoading("ForceLeaveMultiplayerParty");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("ForceLeaveMultiplayerParty");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("ForceLeaveMultiplayerParty");
			_view.ShowTeamsPanel();
			_view.DeepBroadcast(new CustomGameTeamAssignmentsChangedMessage());
		}

		private IEnumerator DeclineInvitation()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0, (object)null, _view.get_gameObject());
			loadingIconPresenter.NotifyLoading("RespondToCustomGameInvitation");
			IRespondToCustomGameInvitationRequest respondToInvitationRequest = serviceFactory.Create<IRespondToCustomGameInvitationRequest>();
			respondToInvitationRequest.ClearCache();
			respondToInvitationRequest.Inject(new RespondToCustomGameInvitationDependancy(isAccept_: false));
			TaskService<ReplyToCustomGameInviteResponseCode> declineInvitationTask = new TaskService<ReplyToCustomGameInviteResponseCode>(respondToInvitationRequest);
			yield return declineInvitationTask;
			if (!declineInvitationTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(declineInvitationTask.behaviour);
			}
			else
			{
				switch (declineInvitationTask.result)
				{
				case ReplyToCustomGameInviteResponseCode.DeclinedInvitationSuccesfully:
					Console.Log("Succesfully declined invite to session");
					break;
				case ReplyToCustomGameInviteResponseCode.ReplyToResponseError:
					ErrorWindow.ShowServiceErrorWindow(declineInvitationTask.behaviour);
					break;
				case ReplyToCustomGameInviteResponseCode.SessionDoesntExist:
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGamesMessageHeader"), StringTableBase<StringTable>.Instance.GetString("strCustomGameInviteReplyDeclineSessionDoesntExist"), StringTableBase<StringTable>.Instance.GetString("strOK")));
					break;
				}
			}
			loadingIconPresenter.NotifyLoadingDone("RespondToCustomGameInvitation");
			_view.Broadcast(new HidePartyInvitationDialogMessage());
		}

		private IEnumerator SendInviteToCustomGameHandler(string invitee, bool isTeamA)
		{
			loadingIconPresenter.NotifyLoading("DispatchCustomGameInvite");
			IGetPlayerCanBeInvitedToCustomGameRequest canBeInvitedCheck = socialRequestFactory.Create<IGetPlayerCanBeInvitedToCustomGameRequest>();
			canBeInvitedCheck.Inject(invitee);
			TaskService<bool> canBeInvitedTask = new TaskService<bool>(canBeInvitedCheck);
			yield return canBeInvitedTask;
			loadingIconPresenter.NotifyLoadingDone("DispatchCustomGameInvite");
			if (!canBeInvitedTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(canBeInvitedTask.behaviour);
			}
			else if (!canBeInvitedTask.result)
			{
				string @string = StringTableBase<StringTable>.Instance.GetString("strCustomGameErrorCannotInviteIfInParty");
				commandFactory.Build<ReportSocialEventCommand>().Inject(@string).Execute();
				_view.Broadcast(new SendInviteToPartyResponse(@string));
				EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0, (object)null, _view.get_gameObject());
			}
			else
			{
				yield return DispatchInvitation(invitee, isTeamA);
			}
		}

		private IEnumerator DispatchInvitation(string invitee, bool isTeamA)
		{
			IDispatchCustomGameInvitationRequest dispatchInvitationRequest = serviceFactory.Create<IDispatchCustomGameInvitationRequest>();
			dispatchInvitationRequest.Inject(new DispatchCustomGameInviteDependancy(invitee, isTeamA));
			TaskService<DispatchCustomGameInviteResponse> dispatchInvitationTask = new TaskService<DispatchCustomGameInviteResponse>(dispatchInvitationRequest);
			yield return dispatchInvitationTask;
			loadingIconPresenter.NotifyLoadingDone("DispatchCustomGameInvite");
			if (!dispatchInvitationTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(dispatchInvitationTask.behaviour);
				yield break;
			}
			string text = null;
			switch (dispatchInvitationTask.result)
			{
			case DispatchCustomGameInviteResponse.InviteeHasAlreadyBeenInvited:
			case DispatchCustomGameInviteResponse.UserInvited:
				Console.Log("succesfully invited user to session, or they were already in the session.");
				EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[142], 0, (object)null, _view.get_gameObject());
				break;
			case DispatchCustomGameInviteResponse.UserIsNotOnline:
				text = StringTableBase<StringTable>.Instance.GetString("strCustomGameCannotInviteUserNotOnline");
				break;
			case DispatchCustomGameInviteResponse.InviteeIsInAnotherCustomGame:
				text = StringTableBase<StringTable>.Instance.GetString("strCustomGameCannotInviteUserAlreadyInCustomGameSession");
				break;
			case DispatchCustomGameInviteResponse.InviteeIsAlreadyInvitedToAnotherCustomGame:
				text = StringTableBase<StringTable>.Instance.GetString("strCustomGameCannotInviteUserAlreadyInvitedToCustomGameSession");
				break;
			case DispatchCustomGameInviteResponse.UserDoesNotExist:
				text = StringTableBase<StringTable>.Instance.GetString("strUserNotExist");
				break;
			case DispatchCustomGameInviteResponse.UserOnlyAcceptsInvitesFromFriendsAndClanmates:
				text = StringTableBase<StringTable>.Instance.GetString("STR_SOCIAL_REASON_USER_ACCEPTS_PLATOON_INVITES_FROM_FRIENDS_AND_CLANS_ONLY");
				break;
			case DispatchCustomGameInviteResponse.UserBlockedYou:
				text = StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_USER_BLOCKED_YOU.ToString());
				break;
			default:
				RemoteLogger.Error("Failed dispatching a custom game invitation to user", "target username:" + invitee + " reason: " + dispatchInvitationTask.result, null);
				break;
			}
			if (text != null)
			{
				commandFactory.Build<ReportSocialEventCommand>().Inject(text).Execute();
				EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0, (object)null, _view.get_gameObject());
				_view.Broadcast(new SendInviteToPartyResponse(text));
			}
			else
			{
				_view.Broadcast(new SendInviteToPartyResponse());
			}
		}
	}
}
