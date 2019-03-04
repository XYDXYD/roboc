using Authentication;
using CustomGames;
using Fabric;
using Mothership.GUI.CustomGames;
using Mothership.GUI.Social;
using Services.Analytics;
using Services.Web.Photon;
using SocialServiceLayer;
using SocialServiceLayer.Photon;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Utility;

namespace Mothership.GUI.Party
{
	internal class PartyGUIController : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private bool _localPlayerIsLeader = true;

		private PartyGUIView _guiView;

		private IServiceEventContainer _socialEventContainer;

		private HidePartyInvitationDialogMessage _hideInvitationDialogMessage;

		private PartyMemberDataChangedMessage[] _setEmptyPlatoonSlotMessages;

		private HidePopupMenuMessage _hidePopupMenuMessage;

		private DisableButtonSignalMessage _disableButtonsMessage;

		private EnableButtonSignalMessage _enableButtonsMessage;

		private PartyGUIStyle _currentGuiStyle;

		private uint _currentRobotCPU;

		private uint _currentCosmeticCPU;

		private int _currentRobotRanking;

		private TiersData _tiersData;

		private uint _previousTierIndex;

		[CompilerGenerated]
		private static Comparison<PlatoonMember> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<ServiceBehaviour> _003C_003Ef__mg_0024cache1;

		[Inject]
		internal ICPUPower cpuPower
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
		internal LoadingIconPresenter loadingIconPresenter
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
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameStateObserver customGameStateObserver
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
		internal IMachineMap machineMap
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

		private void UpdateVisibility()
		{
			_guiView.DeepBroadcast(new HidePopupMenuMessage());
			if (guiInputController.IsTutorialScreenActive())
			{
				Hide();
				return;
			}
			GuiScreens activeScreen = guiInputController.GetActiveScreen();
			bool flag = false;
			if (WorldSwitching.IsInBuildMode() && guiInputController.GetActiveScreen() != GuiScreens.BuildMode)
			{
				Hide();
				return;
			}
			_currentGuiStyle = null;
			for (int i = 0; i < _guiView.styles.Length; i++)
			{
				PartyGUIStyle partyGUIStyle = _guiView.styles[i];
				if (partyGUIStyle.context == activeScreen)
				{
					flag = true;
					_currentGuiStyle = partyGUIStyle;
					if (_currentGuiStyle.allowMouseInteraction)
					{
						_guiView.DeepBroadcast(_enableButtonsMessage);
					}
					else
					{
						_guiView.DeepBroadcast(_disableButtonsMessage);
					}
					_guiView.panelBounds.topAnchor.absolute = -partyGUIStyle.distanceFromTheTop;
					_guiView.panelBounds.bottomAnchor.absolute = -partyGUIStyle.distanceFromTheTop;
					break;
				}
			}
			if (flag)
			{
				TaskRunner.get_Instance().Run(ShowIfNotInCustomGameParty());
			}
			else
			{
				Hide();
			}
		}

		private IEnumerator ShowIfNotInCustomGameParty(bool onlyHides = false)
		{
			IRetrieveCustomGameSessionRequest refreshSessionService = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionService);
			yield return refreshTask;
			if (!refreshTask.succeeded)
			{
				Console.LogError("Cannot retrieve custom game session");
			}
			else if (refreshTask.result.Response == CustomGameSessionRetrieveResponse.SessionRetrieved)
			{
				Hide();
			}
			else if (!onlyHides)
			{
				switch (guiInputController.GetActiveScreen())
				{
				case GuiScreens.LevelRewards:
				case GuiScreens.BundleAwardDialog:
					break;
				case GuiScreens.Garage:
				case GuiScreens.BattleCountdown:
				case GuiScreens.BuildMode:
					Show();
					break;
				default:
					SetVisibleOnlyInvitation();
					break;
				}
			}
		}

		unsafe void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().Run(LoadDependantDataForTiers());
			garageChangedObserver.AddAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_hideInvitationDialogMessage = new HidePartyInvitationDialogMessage();
			_setEmptyPlatoonSlotMessages = new PartyMemberDataChangedMessage[4];
			for (int i = 0; i < 4; i++)
			{
				_setEmptyPlatoonSlotMessages[i] = new PartyMemberDataChangedMessage(i);
			}
			_hidePopupMenuMessage = new HidePopupMenuMessage();
			_disableButtonsMessage = new DisableButtonSignalMessage();
			_enableButtonsMessage = new EnableButtonSignalMessage();
			_guiView.BuildSignal();
			GetPlatoonData(OnPlatoonChanged, null);
			_socialEventContainer = socialEventContainerFactory.Create();
			_socialEventContainer.ReconnectedEvent += CheckForPendingInvitation;
			_socialEventContainer.DisconnectedEvent += OnDisconnected;
			_socialEventContainer.ListenTo<IPlatoonInviteEventListener, PlatoonInvite>(OnInvitedToPlatoon);
			_socialEventContainer.ListenTo<IPlatoonInviteCancelledEventListener, string>(OnPlatoonInviteCancelled);
			_socialEventContainer.ListenTo<IPlatoonChangedEventListener, Platoon>(OnPlatoonChanged);
			_socialEventContainer.ListenTo<IPlatoonMemberStatusChangedEventListener, string, PlatoonStatusChangedData>(OnPartyMemberChangedStatus);
			_socialEventContainer.ListenTo<IPlatoonMemberLeftEventListener, string, PlatoonMember.MemberStatus>(OnPartyMemberLeft);
			_socialEventContainer.ListenTo<IPlatoonDisbandedEventListener>(OnPartyDisbanded);
			_socialEventContainer.ListenTo<IPlatoonRobotTierChangedEventListener, PlatoonRobotTierEventData>(OnPlatoonRobotTierChanged);
			guiInputController.OnScreenStateChange += UpdateVisibility;
			customGameStateObserver.AddAction(new ObserverAction<CustomGameStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			CheckForPendingInvitation();
			SetLocalPlayerAsReady();
			machineMap.OnAddCubeAt += OnCubeAdded;
			machineMap.OnRemoveCubeAt += OnCubeRemoved;
		}

		private void OnCubeAdded(Byte3 gridLoc, MachineCell machineCell)
		{
			bool flag = machineCell.info.persistentCubeData.category == CubeCategory.Cosmetic;
			_currentRobotCPU += machineCell.info.persistentCubeData.cpuRating;
			_currentCosmeticCPU += (flag ? machineCell.info.persistentCubeData.cpuRating : 0);
			_currentRobotRanking += machineCell.info.persistentCubeData.cubeRanking;
			uint num = RobotCPUCalculator.CalculateRobotActualCPU(_currentRobotCPU, _currentCosmeticCPU, cpuPower.MaxCosmeticCpuPool);
			bool isMegabot = num > cpuPower.MaxCpuPower;
			uint num2 = RRAndTiers.ConvertRRToTierIndex((uint)_currentRobotRanking, isMegabot, _tiersData);
			string displayString_ = RRAndTiers.ConvertTierIndexToTierString(num2, _tiersData);
			SetOwnPartyRobotTierMessage message = new SetOwnPartyRobotTierMessage((int)num2, displayString_);
			_guiView.DeepBroadcast(message);
		}

		private void OnCubeRemoved(Byte3 gridLoc, MachineCell machineCell)
		{
			bool flag = machineCell.info.persistentCubeData.category == CubeCategory.Cosmetic;
			_currentRobotCPU -= machineCell.info.persistentCubeData.cpuRating;
			_currentCosmeticCPU -= (flag ? machineCell.info.persistentCubeData.cpuRating : 0);
			_currentRobotRanking -= machineCell.info.persistentCubeData.cubeRanking;
			uint num = RobotCPUCalculator.CalculateRobotActualCPU(_currentRobotCPU, _currentCosmeticCPU, cpuPower.MaxCosmeticCpuPool);
			bool isMegabot = num > cpuPower.MaxCpuPower;
			uint num2 = RRAndTiers.ConvertRRToTierIndex((uint)_currentRobotRanking, isMegabot, _tiersData);
			string displayString_ = RRAndTiers.ConvertTierIndexToTierString(num2, _tiersData);
			SetOwnPartyRobotTierMessage message = new SetOwnPartyRobotTierMessage((int)num2, displayString_);
			_guiView.DeepBroadcast(message);
		}

		private void OnCustomGameStateChanged(ref CustomGameStateDependency dep)
		{
			if (dep.sessionId == null)
			{
				UpdateVisibility();
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			if (_socialEventContainer != null)
			{
				_socialEventContainer.Dispose();
				_socialEventContainer = null;
			}
			guiInputController.OnScreenStateChange -= UpdateVisibility;
			machineMap.OnAddCubeAt -= OnCubeAdded;
			machineMap.OnRemoveCubeAt -= OnCubeRemoved;
			garageChangedObserver.RemoveAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void OnDisconnected()
		{
			ResetPartyIconsViews();
		}

		private void OnPlatoonRobotTierChanged(PlatoonRobotTierEventData platoonRobotTierData)
		{
			string userName = platoonRobotTierData.UserName;
			PartyIconTierChangeMessage message = new PartyIconTierChangeMessage(userName);
			_guiView.DeepBroadcast(message);
		}

		private void OnPartyMemberChangedStatus(string playerDisplayName, PlatoonStatusChangedData statusChangeData)
		{
			if (statusChangeData.oldStatus == PlatoonMember.MemberStatus.Invited && statusChangeData.newStatus != 0)
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.GUI_Party_Request_Accepted));
				TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleAnalytics);
			}
		}

		private void OnPartyMemberLeft(string playerName, PlatoonMember.MemberStatus status)
		{
			if (status == PlatoonMember.MemberStatus.Invited)
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.GUI_Party_Request_Declined));
			}
		}

		private void OnPartyDisbanded()
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.GUI_Party_Request_Declined));
			_localPlayerIsLeader = true;
			_guiView.Broadcast(new PartyIconCannotInteractMessage(preventInteraction_: false));
		}

		internal void ReceiveMessage(object message)
		{
			if (message is PartyMemberButtonClickMessage)
			{
				if (_currentGuiStyle == null || !_currentGuiStyle.allowMouseInteraction)
				{
					return;
				}
				PartyMemberButtonClickMessage partyMemberButtonClickMessage = (PartyMemberButtonClickMessage)message;
				if (partyMemberButtonClickMessage.SlotIndex == -1)
				{
					ShowContextMenu(partyMemberButtonClickMessage.UIElement, string.Empty);
				}
				else if (_localPlayerIsLeader)
				{
					if (partyMemberButtonClickMessage.IconStateWhenClicked == PartyIconState.AddMemberState)
					{
						_guiView.Broadcast(_hidePopupMenuMessage);
						_guiView.DeepBroadcast(new ShowPartyInviteDropDownMessage(partyMemberButtonClickMessage.UIElement, _guiView.inviteDropDownArea));
					}
					else
					{
						ShowContextMenu(partyMemberButtonClickMessage.UIElement, partyMemberButtonClickMessage.PlayerAssignedToSlot);
					}
				}
			}
			else if (message is PartyInvitationResponseMessage)
			{
				PartyInvitationResponseMessage partyInvitationResponseMessage = (PartyInvitationResponseMessage)message;
				if (partyInvitationResponseMessage.acceptInvitation)
				{
					AcceptInvitation();
				}
				else
				{
					DeclineInvitation();
				}
			}
			else if (message is SendInviteToPartyMessage)
			{
				SendInviteToPartyMessage sendInviteToPartyMessage = (SendInviteToPartyMessage)message;
				loadingIconPresenter.NotifyLoading("Checkingpartystatus");
				SendPlatoonInvite(sendInviteToPartyMessage.invitee, delegate
				{
					loadingIconPresenter.NotifyLoadingDone("Checkingpartystatus");
					commandFactory.Build<ReportSocialEventCommand>().Inject(StringTableBase<StringTable>.Instance.GetReplaceString("strPartyInviteSentTo", "{PLAYER}", sendInviteToPartyMessage.invitee)).Execute();
					_guiView.Broadcast(new SendInviteToPartyResponse());
				}, delegate(string errorMsg)
				{
					loadingIconPresenter.NotifyLoadingDone("Checkingpartystatus");
					_guiView.Broadcast(new SendInviteToPartyResponse(errorMsg));
				});
			}
			else if (message is PartyPopupMenuClickMessage)
			{
				PartyPopupMenuClickMessage partyPopupMenuClickMessage = (PartyPopupMenuClickMessage)message;
				switch (partyPopupMenuClickMessage.itemClicked)
				{
				case PartyPopupMenuItems.CancelPendingInvitation | PartyPopupMenuItems.RemoveFromParty:
				case PartyPopupMenuItems.CancelPendingInvitation | PartyPopupMenuItems.LeaveParty:
				case PartyPopupMenuItems.RemoveFromParty | PartyPopupMenuItems.LeaveParty:
				case PartyPopupMenuItems.CancelPendingInvitation | PartyPopupMenuItems.RemoveFromParty | PartyPopupMenuItems.LeaveParty:
					break;
				case PartyPopupMenuItems.CancelPendingInvitation:
					CancelSentInvitation(partyPopupMenuClickMessage.partyMamberName);
					break;
				case PartyPopupMenuItems.RemoveFromParty:
					RemoveFromParty(partyPopupMenuClickMessage.partyMamberName);
					break;
				case PartyPopupMenuItems.LeaveParty:
					LeaveParty();
					break;
				case PartyPopupMenuItems.ChangeAvatar:
					_guiView.DeepBroadcast(ButtonType.ShowAPanel);
					break;
				}
			}
		}

		private void SetPlatoonLeaderStatus(Platoon platoon)
		{
			if (platoon == null)
			{
				_localPlayerIsLeader = true;
			}
			else if (string.IsNullOrEmpty(platoon.leader))
			{
				_localPlayerIsLeader = true;
			}
			else
			{
				_localPlayerIsLeader = IsMe(platoon.leader);
			}
			_guiView.Broadcast(new PartyIconCannotInteractMessage(!_localPlayerIsLeader));
		}

		private void ShowContextMenu(UIWidget uiElement, string partyMemberName)
		{
			_guiView.Broadcast(_hidePopupMenuMessage);
			loadingIconPresenter.NotifyLoading("Checkingpartystatus");
			socialRequestFactory.Create<IGetPlatoonDataRequest>().SetAnswer(new ServiceAnswer<Platoon>(delegate(Platoon platoon)
			{
				SetPlatoonLeaderStatus(platoon);
				loadingIconPresenter.NotifyLoadingDone("Checkingpartystatus");
				if (partyMemberName == string.Empty)
				{
					_guiView.DeepBroadcast(new ShowPartyPopupMenuMessage(uiElement, string.Empty, (PartyPopupMenuItems)((platoon.isInPlatoon ? 4 : 0) | 8)));
				}
				else
				{
					PlatoonMember platoonMember = FindMember(partyMemberName, platoon);
					if (platoonMember == null)
					{
						Console.LogWarning("Cannot Create popup menu for party member. Player Not In Party.");
					}
					else
					{
						ShowPartyPopupMenuMessage message = (platoonMember.Status != 0) ? new ShowPartyPopupMenuMessage(uiElement, partyMemberName, PartyPopupMenuItems.RemoveFromParty) : new ShowPartyPopupMenuMessage(uiElement, partyMemberName, PartyPopupMenuItems.CancelPendingInvitation);
						_guiView.DeepBroadcast(message);
					}
				}
			}, delegate(ServiceBehaviour error)
			{
				loadingIconPresenter.NotifyLoadingDone("Checkingpartystatus");
				HandleGetPlatoonDataRequestFailed(error, null);
			})).Execute();
		}

		private PlatoonMember FindMember(string memberName, Platoon platoon)
		{
			for (int i = 0; i < platoon.members.Length; i++)
			{
				if (platoon.members[i].Name == memberName)
				{
					return platoon.members[i];
				}
			}
			return null;
		}

		private void HandleGetPlatoonDataRequestFailed(ServiceBehaviour serviceBehaviour, Action<string> errorAction)
		{
			SocialErrorCode errorCode = (SocialErrorCode)serviceBehaviour.errorCode;
			errorAction?.Invoke(StringTableBase<StringTable>.Instance.GetString(errorCode.ToString()));
			if (errorAction == null || errorCode == SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR)
			{
				serviceBehaviour.SetAlternativeBehaviour(delegate
				{
				}, StringTableBase<StringTable>.Instance.GetString("strCancel"));
				ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
			}
		}

		private void HandleSendInviteRequestFailed(ServiceBehaviour serviceBehaviour, string inviteeName, Action<string> errorAction)
		{
			SocialErrorCode errorCode = (SocialErrorCode)serviceBehaviour.errorCode;
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0, (object)null, _guiView.get_gameObject());
			string inviteToPartyFailedMessage = SocialStaticUtilities.GetInviteToPartyFailedMessage(inviteeName, errorCode);
			SafeEvent.SafeRaise<string>(errorAction, inviteToPartyFailedMessage);
			if (errorCode == SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR)
			{
				serviceBehaviour.SetAlternativeBehaviour(delegate
				{
				}, StringTableBase<StringTable>.Instance.GetString("strCancel"));
				ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
			}
			else
			{
				commandFactory.Build<ReportSocialEventCommand>().Inject(inviteToPartyFailedMessage).Execute();
			}
		}

		private void OnGarageSlotChanged(ref GarageSlotDependency dependancy)
		{
			uint totalRobotRanking = dependancy.totalRobotRanking;
			uint num = RobotCPUCalculator.CalculateRobotActualCPU(dependancy.totalRobotCPU, dependancy.totalCosmeticCPU, cpuPower.MaxCosmeticCpuPool);
			bool isMegabot = num > cpuPower.MaxCpuPower;
			uint num2 = RRAndTiers.ConvertRRToTierIndex(totalRobotRanking, isMegabot, _tiersData);
			string displayString_ = RRAndTiers.ConvertTierIndexToTierString(num2, _tiersData);
			SetOwnPartyRobotTierMessage message = new SetOwnPartyRobotTierMessage((int)num2, displayString_);
			_guiView.DeepBroadcast(message);
			if (_previousTierIndex != num2)
			{
				_previousTierIndex = num2;
				commandFactory.Build<PartyRobotTierChangedCommand>().Inject(num2).Execute();
			}
		}

		private IEnumerator LoadDependantDataForTiers()
		{
			loadingIconPresenter.NotifyLoading("LoadingTiersData");
			ILoadTiersBandingRequest loadTiersBandingReq = serviceFactory.Create<ILoadTiersBandingRequest>();
			TaskService<TiersData> loadTiersBandingTS = new TaskService<TiersData>(loadTiersBandingReq);
			HandleTaskServiceWithError handleTSWithError = new HandleTaskServiceWithError(loadTiersBandingTS, delegate
			{
				loadingIconPresenter.NotifyLoading("LoadingTiersData");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("LoadingTiersData");
			});
			yield return handleTSWithError.GetEnumerator();
			if (loadTiersBandingTS.succeeded)
			{
				_tiersData = loadTiersBandingTS.result;
			}
			loadingIconPresenter.NotifyLoadingDone("LoadingTiersData");
		}

		private void GetPlatoonData(Action<Platoon> onGetData, Action<string> errorAction)
		{
			socialRequestFactory.Create<IGetPlatoonDataRequest>().SetAnswer(new ServiceAnswer<Platoon>(delegate(Platoon platoon)
			{
				SetPlatoonLeaderStatus(platoon);
				onGetData(platoon);
			}, delegate(ServiceBehaviour behavior)
			{
				HandleGetPlatoonDataRequestFailed(behavior, errorAction);
			})).Execute();
		}

		internal void SendPlatoonInvite(string playerName, Action successAction, Action<string> errorAction)
		{
			string @string;
			if (guiInputController.GetActiveScreen() == GuiScreens.BattleCountdown)
			{
				@string = StringTableBase<StringTable>.Instance.GetString("strNoPartyInviteInQueueOrBattle");
				commandFactory.Build<ReportSocialEventCommand>().Inject(@string).Execute();
				SafeEvent.SafeRaise<string>(errorAction, @string);
			}
			else if (!SocialStaticUtilities.ValidateInviteeName(ref playerName, out @string))
			{
				commandFactory.Build<ReportSocialEventCommand>().Inject(@string).Execute();
				SafeEvent.SafeRaise<string>(errorAction, @string);
			}
			else
			{
				loadingIconPresenter.NotifyLoading("CheckCanBeInvited");
				IGetPlayerCanBeInvitedToRegularPartyRequest getPlayerCanBeInvitedToRegularPartyRequest = socialRequestFactory.Create<IGetPlayerCanBeInvitedToRegularPartyRequest>();
				getPlayerCanBeInvitedToRegularPartyRequest.Inject(playerName);
				getPlayerCanBeInvitedToRegularPartyRequest.SetAnswer(new ServiceAnswer<GetPlayerCanBeInvitedToRegularPartyResponseCode>(delegate(GetPlayerCanBeInvitedToRegularPartyResponseCode canBeInvitedResponse)
				{
					loadingIconPresenter.NotifyLoadingDone("CheckCanBeInvited");
					HandleCanPlayerBeInvitedToRegularPartyRequestComplete(canBeInvitedResponse, playerName, successAction, errorAction);
				}, delegate(ServiceBehaviour serviceBehaviour)
				{
					loadingIconPresenter.NotifyLoadingDone("CheckCanBeInvited");
					HandleSendInviteRequestFailed(serviceBehaviour, playerName, errorAction);
				}));
				getPlayerCanBeInvitedToRegularPartyRequest.Execute();
			}
		}

		private void HandleCanPlayerBeInvitedToRegularPartyRequestComplete(GetPlayerCanBeInvitedToRegularPartyResponseCode responseCode, string playerName, Action successAction, Action<string> errorAction)
		{
			switch (responseCode)
			{
			case GetPlayerCanBeInvitedToRegularPartyResponseCode.PlayerCanBeInvited:
				socialRequestFactory.Create<IInviteToPlatoonRequest, string>(playerName).SetAnswer(new ServiceAnswer(delegate
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[142], 0, (object)null, _guiView.get_gameObject());
					SafeEvent.SafeRaise(successAction);
				}, delegate(ServiceBehaviour serviceBehaviour)
				{
					HandleSendInviteRequestFailed(serviceBehaviour, playerName, errorAction);
				})).Execute();
				break;
			case GetPlayerCanBeInvitedToRegularPartyResponseCode.TargetIsAlreadyInCustomGame:
			case GetPlayerCanBeInvitedToRegularPartyResponseCode.TargetAlreadyHasOutstandingInvitationToCustomGame:
			case GetPlayerCanBeInvitedToRegularPartyResponseCode.NoNameProvided:
			{
				string text = "strCustomGameError";
				if (responseCode == GetPlayerCanBeInvitedToRegularPartyResponseCode.TargetIsAlreadyInCustomGame)
				{
					text = StringTableBase<StringTable>.Instance.GetString("strCustomGameCannotInviteToRegularPartyIsInCustomGame");
				}
				if (responseCode == GetPlayerCanBeInvitedToRegularPartyResponseCode.TargetAlreadyHasOutstandingInvitationToCustomGame)
				{
					text = StringTableBase<StringTable>.Instance.GetString("strCustomGameCannotInviteToRegularPartyOutstandingCustomInvite");
				}
				if (responseCode == GetPlayerCanBeInvitedToRegularPartyResponseCode.NoNameProvided)
				{
					text = StringTableBase<StringTable>.Instance.GetString("strError");
				}
				commandFactory.Build<ReportSocialEventCommand>().Inject(text).Execute();
				errorAction(text);
				break;
			}
			}
		}

		internal void RegisterView(PartyGUIView guiView)
		{
			_guiView = guiView;
		}

		private void Show()
		{
			_guiView.mainContent.SetActive(true);
			_guiView.partyInvitation.SetActive(true);
		}

		private void Hide()
		{
			_guiView.mainContent.SetActive(false);
			_guiView.partyInvitation.SetActive(false);
		}

		private void SetVisibleOnlyInvitation()
		{
			_guiView.mainContent.SetActive(false);
			_guiView.partyInvitation.SetActive(true);
		}

		private void OnPlatoonChanged(Platoon platoon)
		{
			SetPlatoonLeaderStatus(platoon);
			TaskRunner.get_Instance().Run(ShowIfNotInCustomGameParty(onlyHides: true));
			if (platoon.Size == 0)
			{
				ResetPartyIconsViews();
				return;
			}
			bool isInQueue_ = false;
			PlatoonMember[] array = new PlatoonMember[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = null;
			}
			int num = 0;
			for (int j = 0; j < platoon.members.Length; j++)
			{
				if (IsMe(platoon.members[j].Name))
				{
					isInQueue_ = (platoon.members[j].Status == PlatoonMember.MemberStatus.InQueue);
				}
				else
				{
					array[num++] = platoon.members[j];
				}
			}
			Array.Sort(array, CompareMember);
			for (int k = 0; k < 4; k++)
			{
				PartyMemberDataChangedMessage message;
				if (array[k] == null)
				{
					message = _setEmptyPlatoonSlotMessages[k];
				}
				else
				{
					PlatoonMember platoonMember = array[k];
					message = new PartyMemberDataChangedMessage(k, platoonMember.Name, platoonMember.DisplayName, platoonMember.Status, platoon.leader == platoonMember.Name, platoonMember.AvatarInfo, platoon.leader);
				}
				_guiView.DeepBroadcast(message);
			}
			_guiView.DeepBroadcast(new SetOwnPartyStatusMessage(platoon.GetIsPlatoonLeader(), isInQueue_));
		}

		private void ResetPartyIconsViews()
		{
			for (int i = 0; i < 4; i++)
			{
				_guiView.DeepBroadcast(_setEmptyPlatoonSlotMessages[i]);
			}
			_guiView.DeepBroadcast(new SetOwnPartyStatusMessage(isLeader_: false, isInQueue_: false));
		}

		private static int CompareMember(PlatoonMember a, PlatoonMember b)
		{
			if (a == null)
			{
				return 1;
			}
			if (b == null)
			{
				return -1;
			}
			return a.AddedTimestamp.CompareTo(b.AddedTimestamp);
		}

		private void OnInvitedToPlatoon(PlatoonInvite invite)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.GUI_Party_Request_Received));
			PartyInvitationReceivedMessage message = new PartyInvitationReceivedMessage(invite.InviterName, invite.DisplayName, invite.AvatarInfo);
			_guiView.DeepBroadcast(message);
			ShowPartyInvitationDialogMessage message2 = new ShowPartyInvitationDialogMessage();
			_guiView.DeepBroadcast(message2);
		}

		private void OnPlatoonInviteCancelled(string inviter)
		{
			HideInviteDialog();
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.GUI_Party_Request_Declined));
		}

		private void SetLocalPlayerAsReady()
		{
			GetPlatoonData(delegate(Platoon platoon)
			{
				if (platoon.isInPlatoon)
				{
					SetPlatoonMemberStatusDependency param = new SetPlatoonMemberStatusDependency(User.Username, PlatoonMember.MemberStatus.Ready);
					socialRequestFactory.Create<ISetPlatoonMemberStatusRequest, SetPlatoonMemberStatusDependency>(param).SetAnswer(new ServiceAnswer(LogServiceFailed)).Execute();
				}
			}, null);
		}

		private static void LogServiceFailed(ServiceBehaviour behaviour)
		{
			RemoteLogger.Error(behaviour.exceptionThrown);
		}

		private void CheckForPendingInvitation()
		{
			loadingIconPresenter.NotifyLoading("CheckPendingInvitation");
			IGetPlatoonPendingInviteRequest getPlatoonPendingInviteRequest = socialRequestFactory.Create<IGetPlatoonPendingInviteRequest>();
			getPlatoonPendingInviteRequest.ForceRefresh();
			getPlatoonPendingInviteRequest.SetAnswer(new ServiceAnswer<PlatoonInvite>(delegate(PlatoonInvite invite)
			{
				if (invite != null)
				{
					OnInvitedToPlatoon(invite);
				}
				loadingIconPresenter.NotifyLoadingDone("CheckPendingInvitation");
			}, delegate(ServiceBehaviour ServiceBehaviour)
			{
				loadingIconPresenter.NotifyLoadingDone("CheckPendingInvitation");
				ShowPopupError(ServiceBehaviour);
			}));
			getPlatoonPendingInviteRequest.Execute();
		}

		private void AcceptInvitation()
		{
			loadingIconPresenter.NotifyLoading("AcceptInvitation");
			if (guiInputController.GetActiveScreen() == GuiScreens.BattleCountdown)
			{
				guiInputController.CloseCurrentScreen();
			}
			socialRequestFactory.Create<IAcceptPlatoonInviteRequest>().SetAnswer(new ServiceAnswer(delegate
			{
				HideInviteDialog();
				loadingIconPresenter.NotifyLoadingDone("AcceptInvitation");
			}, delegate(ServiceBehaviour ServiceBehaviour)
			{
				HideInviteDialog();
				loadingIconPresenter.NotifyLoadingDone("AcceptInvitation");
				ShowPopupError(ServiceBehaviour);
				CheckForPendingInvitation();
			})).Execute();
		}

		private void DeclineInvitation()
		{
			loadingIconPresenter.NotifyLoading("DeclineInvitation");
			socialRequestFactory.Create<IDeclinePlatoonInviteRequest>().SetAnswer(new ServiceAnswer(delegate
			{
				HideInviteDialog();
				loadingIconPresenter.NotifyLoadingDone("DeclineInvitation");
			}, delegate(ServiceBehaviour ServiceBehaviour)
			{
				HideInviteDialog();
				loadingIconPresenter.NotifyLoadingDone("DeclineInvitation");
				ShowPopupError(ServiceBehaviour);
				CheckForPendingInvitation();
			})).Execute();
		}

		private void CancelSentInvitation(string memberName)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0, (object)null, _guiView.get_gameObject());
			RemoveFromParty(memberName);
		}

		private void RemoveFromParty(string memberName)
		{
			loadingIconPresenter.NotifyLoading("RemoveFromParty");
			socialRequestFactory.Create<IKickFromPlatoonRequest, string>(memberName).SetAnswer(new ServiceAnswer(delegate
			{
				loadingIconPresenter.NotifyLoadingDone("RemoveFromParty");
			}, delegate(ServiceBehaviour ServiceBehaviour)
			{
				loadingIconPresenter.NotifyLoadingDone("RemoveFromParty");
				ShowPopupError(ServiceBehaviour);
			})).Execute();
		}

		private void LeaveParty()
		{
			loadingIconPresenter.NotifyLoading("LeaveParty");
			socialRequestFactory.Create<ILeavePlatoonRequest>().SetAnswer(new ServiceAnswer(delegate
			{
				loadingIconPresenter.NotifyLoadingDone("LeaveParty");
			}, delegate(ServiceBehaviour ServiceBehaviour)
			{
				loadingIconPresenter.NotifyLoadingDone("LeaveParty");
				ShowPopupError(ServiceBehaviour);
			})).Execute();
		}

		private void HideInviteDialog()
		{
			_guiView.DeepBroadcast(_hideInvitationDialogMessage);
		}

		private static void ShowPopupError(ServiceBehaviour serviceBehaviour)
		{
			if (serviceBehaviour.errorCode != 39)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(serviceBehaviour.errorTitle, serviceBehaviour.errorBody));
			}
		}

		private static bool IsMe(string name)
		{
			return string.Compare(name, User.Username, StringComparison.OrdinalIgnoreCase) == 0;
		}

		private IEnumerator HandleAnalytics()
		{
			TaskService logFriendAddedToPartyRequest = analyticsRequestFactory.Create<ILogFriendAddedToPartyRequest>().AsTask();
			yield return logFriendAddedToPartyRequest;
			if (!logFriendAddedToPartyRequest.succeeded)
			{
				throw new Exception("Log Friend Added to Party Request failed", logFriendAddedToPartyRequest.behaviour.exceptionThrown);
			}
		}
	}
}
