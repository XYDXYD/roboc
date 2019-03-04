using Authentication;
using Avatars;
using Fabric;
using Mothership.GUI.Social;
using Robocraft.GUI;
using Services.Analytics;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal class YourClanController : ClanSectionControllerBase, IWaitForFrameworkDestruction
	{
		private class YourClanPreviousStateData
		{
			public string currentClanShown;

			public bool isInClan;

			public ClanViewMode currentDisplayMode;

			public YourClanPreviousStateData(string currentClanShown_, bool isInClan_, ClanViewMode currentDisplayMode_)
			{
				currentClanShown = currentClanShown_;
				isInClan = isInClan_;
				currentDisplayMode = currentDisplayMode_;
			}
		}

		public const string PLUS_BUTTON_NAME = "PlusButton";

		private const int MAX_CLAN_SIZE = 50;

		private YourClanInfoDataSource _yourClanInfoDataSource;

		private YourClanAvatarImageDataSource _yourClanAvatarImageDataSource;

		private PlayerListDataSource _playersListDataSource;

		private PlayerHeadersListDataSource _playersListHeadersDataSource;

		private YourClanPreviousStateData _previousStateData;

		private string _clanNameForAnalytics = string.Empty;

		private string _currentClanShown = string.Empty;

		private bool _isInClan;

		private ClanMemberRank _myRankInClan = ClanMemberRank.Leader;

		private ClanMember[] _members;

		private ClanViewMode _currentDisplayMode = ClanViewMode.NoClan;

		private IServiceEventContainer _socialEventContainer;

		private TextLabelComponentDataContainer _labelDataContainer = new TextLabelComponentDataContainer(string.Empty);

		private BubbleSignal<IChainRoot> _bubble;

		private BubbleSignal<ISocialRoot> _bubbleToSocialRoot;

		private bool _receivedInvite;

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory webRequestFactory
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
		internal YourClanLayoutFactory yourClanLayoutFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ClanController clansController
		{
			private get;
			set;
		}

		[Inject]
		internal ClanPopupMenuController clanPopupMenuController
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory CommandFactory
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
		internal IMultiAvatarLoader avatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarAvailableObserver avatarAvailableObserver
		{
			private get;
			set;
		}

		[Inject]
		internal IContextSensitiveXPRefresher contextSensitiveXPrefresher
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
		internal PresetAvatarMapProvider presetAvatarMapProvider
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

		public override ClanSectionType SectionType => ClanSectionType.YourClan;

		public ClanViewMode CurrentViewMode => _currentDisplayMode;

		public void OnFrameworkDestroyed()
		{
			_yourClanAvatarImageDataSource.Dispose();
			_playersListDataSource.Dispose();
			_socialEventContainer.Dispose();
		}

		public override void PushCurrentState()
		{
			_previousStateData = new YourClanPreviousStateData(_currentClanShown, _isInClan, _currentDisplayMode);
		}

		public override void PopPreviousState()
		{
			if (_previousStateData != null)
			{
				_currentClanShown = _previousStateData.currentClanShown;
				_isInClan = _previousStateData.isInClan;
				_currentDisplayMode = _previousStateData.currentDisplayMode;
				RefreshEverything(forceRefresh: false);
				_previousStateData = null;
			}
		}

		public override void HandleClanMessageDerived(SocialMessage message)
		{
			if (message.messageDispatched == SocialMessageType.ConfigureYourClanData)
			{
				_currentClanShown = message.extraDetails;
				if (string.Empty == message.extraDetails)
				{
					_currentDisplayMode = ClanViewMode.YourClan;
				}
				else
				{
					_currentDisplayMode = ClanViewMode.AnotherClan;
				}
				RefreshEverything(forceRefresh: true);
			}
			else if (message.messageDispatched == SocialMessageType.Refresh)
			{
				RefreshEverything(forceRefresh: false);
			}
			else if (message.messageDispatched == SocialMessageType.ActivateYourClanTab || message.messageDispatched == SocialMessageType.MaximizeClanScreen)
			{
				contextSensitiveXPrefresher.ClanScreenShown();
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "playerlist"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "root", string.Empty));
			}
			else if (message.messageDispatched == SocialMessageType.CloseSocialScreens || message.messageDispatched == SocialMessageType.MaximizeFriendScreen)
			{
				contextSensitiveXPrefresher.ClanScreenHidden();
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "playerlist"));
			}
		}

		public override void HandleGenericMessage(GenericComponentMessage receivedMessage)
		{
			if (receivedMessage.Message == MessageType.ButtonClicked)
			{
				if (receivedMessage.Originator == "LeaveButton")
				{
					HandleLeaveClanClicked();
				}
				else if (receivedMessage.Originator == "JoinButton")
				{
					HandleJoinClicked();
				}
				else if (receivedMessage.Originator == "InviteButton")
				{
					HandleInviteClicked();
				}
				else if (receivedMessage.Originator == "LeaveConfirmationDialogCancel")
				{
					HandleLeaveCancelClicked();
				}
				else if (receivedMessage.Originator == "LeaveConfirmationDialogConfirm")
				{
					HandleLeaveConfirmClicked();
				}
			}
			else if (receivedMessage.Message == MessageType.RefreshData && (receivedMessage.Originator == "ClanInvitationRoot" || receivedMessage.Target == "YourClanControllerRoot"))
			{
				RefreshEverything(forceRefresh: false);
			}
			else if (receivedMessage.Message == MessageType.ButtonWithinListClicked)
			{
				PlayerListItemComponentDataContainer.PlayerListItemInfo playerListItemInfo = receivedMessage.Data.UnpackData<PlayerListItemComponentDataContainer.PlayerListItemInfo>();
				if (playerListItemInfo.buttonClicked == "PlusButton")
				{
					InviteToParty(playerListItemInfo.nameOfPlayer);
				}
				else
				{
					HandleContextMenuRightClicked(playerListItemInfo.nameOfPlayer);
				}
			}
		}

		public override void HandleMessage(object message)
		{
			if (!(message is SocialReceivedPartyInviteMessage))
			{
				return;
			}
			SocialReceivedPartyInviteMessage socialReceivedPartyInviteMessage = message as SocialReceivedPartyInviteMessage;
			if (_receivedInvite != socialReceivedPartyInviteMessage.pendingInvite)
			{
				_receivedInvite = socialReceivedPartyInviteMessage.pendingInvite;
				if (_playersListDataSource.GetPartyData() != null)
				{
					UpdateCanBeInvitedToPartyData(_playersListDataSource.GetPartyData());
					DispatchGenericMessage(new GenericComponentMessage(MessageType.UpdateView, "root", string.Empty));
				}
			}
		}

		public void HandleJoinClicked()
		{
			loadingIconPresenter.NotifyLoading("Clans");
			IJoinClanRequest joinClanRequest = socialRequestFactory.Create<IJoinClanRequest>();
			joinClanRequest.Inject(_currentClanShown);
			joinClanRequest.SetAnswer(new ServiceAnswer<ClanInfoAndMembers>(delegate(ClanInfoAndMembers clanInfoAndMembers)
			{
				loadingIconPresenter.NotifyLoadingDone("Clans");
				HandleJoinClanSuccess(clanInfoAndMembers.ClanInfo.ClanName);
			}, HandleJoinClanError)).Execute();
		}

		public void HandleJoinClanSuccess(string newClanName)
		{
			base.clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.SingleTabOnTopBackClicked, string.Empty, new AdditionalClanSectionActivationInfo(shouldPushCurrentState_: false, shouldRestorePreviousState_: false)));
			base.clanController.DispatchSignalChainMessage(new ClanInviteListChangedMessage(null));
			_previousStateData = null;
			_isInClan = true;
			_currentClanShown = newClanName;
			_currentDisplayMode = ClanViewMode.YourClan;
			base.clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.ChangeTabTypeAndSelect, string.Empty, new ChangeTabTypeData(0, ClanSectionType.YourClan)));
			base.clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.ConfigureYourClanData, string.Empty));
			_clanView.BubbleSocialMessageUp(new SocialMessage(SocialMessageType.ClanJoined, string.Empty));
			CommandFactory.Build<JoinClanChatChannelCommand>().Inject(newClanName).Execute();
			TaskRunner.get_Instance().Run(HandleClanJoinedAnalytics(newClanName));
		}

		public void HandleInviteClicked()
		{
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "YourClanRoot", "ClanInvitationRoot"));
		}

		public override void OnSetupController()
		{
			_socialEventContainer = socialEventContainerFactory.Create();
			_yourClanInfoDataSource = new YourClanInfoDataSource(socialRequestFactory, _socialEventContainer);
			_yourClanAvatarImageDataSource = new YourClanAvatarImageDataSource(socialRequestFactory, _socialEventContainer, avatarLoader, avatarAvailableObserver, presetAvatarMapProvider.GetAvatarMap());
			_playersListDataSource = new PlayerListDataSource(socialRequestFactory, webRequestFactory, _socialEventContainer, avatarLoader, avatarAvailableObserver);
			_playersListHeadersDataSource = new PlayerHeadersListDataSource(socialRequestFactory, _socialEventContainer);
			_currentDisplayMode = ClanViewMode.NoClan;
			contextSensitiveXPrefresher.SetDataSources(new List<IDataSource>
			{
				_playersListDataSource
			});
			ConfigureDataSourcesSearchTerms();
		}

		public override void OnViewSet(ClanSectionViewBase view)
		{
			YourClanView view2 = view as YourClanView;
			_bubbleToSocialRoot = new BubbleSignal<ISocialRoot>(view.get_transform());
			yourClanLayoutFactory.BuildAll(view2, _yourClanInfoDataSource, _yourClanAvatarImageDataSource, _playersListDataSource, _playersListHeadersDataSource);
			RegisterToEventListeners();
			RegisterToDataSourceEvents();
			ConfigureDisplayForClanViewMode();
			RefreshEverything(forceRefresh: true);
		}

		private void RegisterToDataSourceEvents()
		{
			_playersListHeadersDataSource.onDataChanged += HandleOnDataChanged;
			YourClanInfoDataSource yourClanInfoDataSource = _yourClanInfoDataSource;
			yourClanInfoDataSource.onDataChanged = (Action)Delegate.Combine(yourClanInfoDataSource.onDataChanged, new Action(HandleOnDataChanged));
			_yourClanAvatarImageDataSource.onDataChanged += HandleOnDataChanged;
		}

		private void HandleOnDataChanged()
		{
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "root", string.Empty));
		}

		private void RegisterToEventListeners()
		{
			_socialEventContainer.ListenTo<IClanMemberDataChangedEventListener, ClanMember[], ClanMemberDataChangedEventContent>(HandleOnClanMemberDataChanged);
			_socialEventContainer.ListenTo<IRemovedFromClanEventListener>(HandleOnRemovedFromClan);
			_socialEventContainer.ListenTo<IClanRenamedEventListener, ClanRenameDependency>(HandleOnClanRenamed);
			_socialEventContainer.ListenTo<IPlatoonChangedEventListener, Platoon>(HandlePartyDataChanged);
			_socialEventContainer.ListenTo<IClanMemberJoinedEventListener, ClanMember[], ClanMember>(HandleOnClanMemberJoined);
			_socialEventContainer.ListenTo<IClanMemberLeftEventListener, ClanMember[], ClanMember>(HandleOnClanMemberLeft);
		}

		private void HandleOnClanMemberLeft(ClanMember[] clanMembers, ClanMember member)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[131], 0, (object)null, _clanView.get_gameObject());
			HandleOnClanMemberDataChanged(clanMembers, null);
		}

		private void HandleOnClanMemberJoined(ClanMember[] clanMembers, ClanMember member)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[130], 0, (object)null, _clanView.get_gameObject());
			HandleOnClanMemberDataChanged(clanMembers, null);
		}

		private void InviteToParty(string invitedName)
		{
			_bubbleToSocialRoot.TargetedDispatch<SocialScreenQuickInviteToPartyMessage>(new SocialScreenQuickInviteToPartyMessage(invitedName));
		}

		private void HandleOnClanRenamed(ClanRenameDependency data)
		{
			RefreshClanInfoOnly(forceRefresh: true);
			_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strClanRenamedNotification").Replace("{ADMINNAME}", data.AdminName));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, string.Empty, "ErrorDialog"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.SetData, "yourClanErrorLabel", string.Empty, _labelDataContainer));
		}

		private void HandleOnRemovedFromClan()
		{
			_previousStateData = null;
			_currentDisplayMode = ClanViewMode.NoClan;
			_yourClanInfoDataSource.SetSearchTerm(ClanViewMode.NoClan, string.Empty);
			_yourClanAvatarImageDataSource.SetSearchTerm(ClanViewMode.NoClan, string.Empty);
			_isInClan = false;
			SocialMessage message = new SocialMessage(SocialMessageType.ChangeTabTypeAndSelect, string.Empty, new ChangeTabTypeData(0, ClanSectionType.CreateClan));
			clansController.DispatchAnyClanMessage(message);
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "LeaveButton"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "InviteButton"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "JoinButton"));
			_clanView.BubbleSocialMessageUp(new SocialMessage(SocialMessageType.ClanLeft, string.Empty));
		}

		private void HandleOnClanMemberDataChanged(ClanMember[] clanMembers, ClanMemberDataChangedEventContent _)
		{
			bool flag = false;
			foreach (ClanMember clanMember in clanMembers)
			{
				if (clanMember.Name.Equals(User.Username))
				{
					flag = true;
					_myRankInClan = clanMember.ClanMemberRank;
					ConfigureButtonsForRank();
				}
			}
			if (!flag)
			{
				_isInClan = false;
				HandleOnRemovedFromClan();
			}
		}

		private void RefreshEverything(bool forceRefresh)
		{
			TaskRunner.get_Instance().Run(RefreshWithCache(forceRefresh));
		}

		private void RefreshClanInfoOnly(bool forceRefresh)
		{
			TaskRunner.get_Instance().Run(RefreshClanInfoOnlyWithCache(forceRefresh));
		}

		private IEnumerator HandleClanJoinedAnalytics(string clanName)
		{
			TaskService logClanJoinedRequest = analyticsRequestFactory.Create<ILogClanJoinedRequest, string>(clanName).AsTask();
			yield return logClanJoinedRequest;
			if (!logClanJoinedRequest.succeeded)
			{
				throw new Exception("Log Clan Joined Request failed", logClanJoinedRequest.behaviour.exceptionThrown);
			}
		}

		private IEnumerator HandleClanLeftAnalytics(string clanName)
		{
			TaskService logClanLeftRequest = analyticsRequestFactory.Create<ILogClanLeftRequest, string>(clanName).AsTask();
			yield return logClanLeftRequest;
			if (!logClanLeftRequest.succeeded)
			{
				throw new Exception("Log Clan Left Request failed", logClanLeftRequest.behaviour.exceptionThrown);
			}
		}

		private IEnumerator RefreshClanInfoOnlyWithCache(bool forceRefresh)
		{
			loadingIconPresenter.NotifyLoading("Clans");
			_yourClanInfoDataSource.SetForceRefresh(forceRefresh);
			ParallelTaskCollection loadClanData = new ParallelTaskCollection();
			loadClanData.Add(_yourClanInfoDataSource.RefreshData());
			loadClanData.Add(_yourClanAvatarImageDataSource.RefreshData());
			yield return loadClanData;
			loadingIconPresenter.NotifyLoadingDone("Clans");
			OnLoadTasksComplete();
		}

		private IEnumerator RefreshWithCache(bool forceRefresh)
		{
			ConfigureDataSourcesSearchTerms();
			loadingIconPresenter.NotifyLoading("Clans");
			yield return ConfigureDisplayForClanViewMode();
			_yourClanInfoDataSource.SetForceRefresh(forceRefresh);
			ParallelTaskCollection loadClanData = new ParallelTaskCollection();
			loadClanData.Add(_yourClanInfoDataSource.RefreshData());
			loadClanData.Add(_yourClanAvatarImageDataSource.RefreshData());
			loadClanData.Add(_playersListDataSource.RefreshData());
			loadClanData.Add(_playersListHeadersDataSource.RefreshData());
			yield return loadClanData;
			loadingIconPresenter.NotifyLoadingDone("Clans");
			OnLoadTasksComplete(_playersListDataSource.GetPartyData());
		}

		private void ConfigureDataSourcesSearchTerms()
		{
			_yourClanInfoDataSource.SetSearchTerm(_currentDisplayMode, _currentClanShown);
			_yourClanAvatarImageDataSource.SetSearchTerm(_currentDisplayMode, _currentClanShown);
			_playersListDataSource.SetSearchTerm(_currentDisplayMode, _currentClanShown);
			_playersListHeadersDataSource.SetSearchTerm(_currentDisplayMode, _currentClanShown);
		}

		private ITask ConfigureDisplayForClanViewMode()
		{
			switch (_currentDisplayMode)
			{
			case ClanViewMode.NoClan:
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "LeaveButton"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "InviteButton"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "JoinButton"));
				break;
			case ClanViewMode.AnotherClan:
			{
				loadingIconPresenter.NotifyLoading("Clans");
				IGetClanInfoAndMembersRequest getClanInfoAndMembersRequest = socialRequestFactory.Create<IGetClanInfoAndMembersRequest>();
				getClanInfoAndMembersRequest.Inject(_currentClanShown);
				getClanInfoAndMembersRequest.SetAnswer(new ServiceAnswer<ClanInfoAndMembers>(HandleOnOtherClanInfoRetrieved, HandleGetOtherClanInfoError));
				return getClanInfoAndMembersRequest;
			}
			case ClanViewMode.YourClan:
			{
				loadingIconPresenter.NotifyLoading("Clans");
				IGetMyClanInfoAndMembersRequest getMyClanInfoAndMembersRequest = socialRequestFactory.Create<IGetMyClanInfoAndMembersRequest>();
				getMyClanInfoAndMembersRequest.SetAnswer(new ServiceAnswer<ClanInfoAndMembers>(HandleOnClanInfoRetrieved, HandleGetClanInfoError));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "LeaveButton"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "InviteButton"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "JoinButton"));
				return getMyClanInfoAndMembersRequest;
			}
			}
			return null;
		}

		private void ConfigureButtonsForRank()
		{
			if (_myRankInClan == ClanMemberRank.Leader)
			{
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "clandescription"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "ClanDescriptionTextEntry"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "UploadAvatarImageButton"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "ClanTypePopUpList"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "claninvitestatus"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Enable, "root", "InviteButton"));
				return;
			}
			if (_myRankInClan == ClanMemberRank.Officer)
			{
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "clandescription"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "ClanDescriptionTextEntry"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Enable, "root", "InviteButton"));
			}
			else
			{
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "clandescription"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "ClanDescriptionTextEntry"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Disable, "root", "InviteButton"));
			}
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "UploadAvatarImageButton"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "ClanTypePopUpList"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "claninvitestatus"));
		}

		private void HandleOnOtherClanInfoRetrieved(ClanInfoAndMembers clanData)
		{
			int clanSize = clanData.ClanInfo.ClanSize;
			ClanType clanType = clanData.ClanInfo.ClanType;
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "clandescription"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "ClanDescriptionTextEntry"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "UploadAvatarImageButton"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "ClanTypePopUpList"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "claninvitestatus"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "LeaveButton"));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "InviteButton"));
			if (clanType != ClanType.Closed)
			{
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "JoinButton"));
				if (_isInClan || clanSize >= 50)
				{
					DispatchGenericMessage(new GenericComponentMessage(MessageType.Disable, "root", "JoinButton"));
				}
				else
				{
					DispatchGenericMessage(new GenericComponentMessage(MessageType.Enable, "root", "JoinButton"));
				}
			}
			else
			{
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "JoinButton"));
			}
			loadingIconPresenter.NotifyLoadingDone("Clans");
		}

		private void HandleGetOtherClanInfoError(ServiceBehaviour behaviour)
		{
			GenericErrorData error = new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, Localization.Get("strOK", true));
			ErrorWindow.ShowErrorWindow(error);
			loadingIconPresenter.NotifyLoadingDone("Clans");
		}

		private void HandleContextMenuRightClicked(string playerName)
		{
			if (_currentDisplayMode != 0)
			{
				return;
			}
			ClanMemberRank clickedMembersRank = ClanMemberRank.Member;
			bool clickedMemberWasInvited = false;
			bool clickedMemberWasOnline = false;
			for (int i = 0; i < _members.Length; i++)
			{
				if (_members[i].Name == playerName)
				{
					clickedMembersRank = _members[i].ClanMemberRank;
					clickedMemberWasInvited = (_members[i].ClanMemberState == ClanMemberState.Invited);
					clickedMemberWasOnline = _members[i].IsOnline;
					break;
				}
			}
			clanPopupMenuController.ShowPopupMenuForContext(playerName, _myRankInClan, clickedMembersRank, clickedMemberWasInvited, clickedMemberWasOnline);
		}

		private void HandleLeaveClanClicked()
		{
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "root", "LeaveConfirmationDialog"));
		}

		private void HandleLeaveConfirmClicked()
		{
			loadingIconPresenter.NotifyLoading("Clans");
			ILeaveClanRequest leaveClanRequest = socialRequestFactory.Create<ILeaveClanRequest>();
			leaveClanRequest.SetAnswer(new ServiceAnswer(delegate
			{
				_isInClan = false;
				_currentClanShown = string.Empty;
				_currentDisplayMode = ClanViewMode.NoClan;
				ConfigureDisplayForClanViewMode();
				SocialMessage message = new SocialMessage(SocialMessageType.ChangeTabTypeAndSelect, string.Empty, new ChangeTabTypeData(0, ClanSectionType.CreateClan));
				clansController.DispatchAnyClanMessage(message);
				_clanView.BubbleSocialMessageUp(new SocialMessage(SocialMessageType.ClanLeft, string.Empty));
				CommandFactory.Build<LeaveClanChatChannelCommand>().Execute();
				loadingIconPresenter.NotifyLoadingDone("Clans");
				TaskRunner.get_Instance().Run(HandleClanLeftAnalytics(_clanNameForAnalytics));
				_clanNameForAnalytics = string.Empty;
			}, HandleLeaveClanError)).Execute();
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "LeaveConfirmationDialog"));
		}

		private void HandleLeaveCancelClicked()
		{
			DispatchGenericMessage(new GenericComponentMessage(MessageType.Hide, "root", "LeaveConfirmationDialog"));
		}

		private void HandleOnClanInfoRetrieved(ClanInfoAndMembers data)
		{
			if (data != null)
			{
				int num = data.ClanMembers.Length;
				_members = new ClanMember[data.ClanMembers.Length];
				Array.Copy(data.ClanMembers, _members, data.ClanMembers.Length);
				_isInClan = true;
				_clanNameForAnalytics = data.ClanInfo.ClanName;
				_myRankInClan = ClanMemberRank.Member;
				for (int i = 0; i < data.ClanMembers.Length; i++)
				{
					if (data.ClanMembers[i].Name == User.Username)
					{
						_myRankInClan = data.ClanMembers[i].ClanMemberRank;
					}
				}
				if (num >= 50)
				{
					DispatchGenericMessage(new GenericComponentMessage(MessageType.Disable, "root", "InviteButton"));
				}
				else
				{
					DispatchGenericMessage(new GenericComponentMessage(MessageType.Enable, "root", "InviteButton"));
				}
				ConfigureButtonsForRank();
			}
			else
			{
				_isInClan = false;
			}
			loadingIconPresenter.NotifyLoadingDone("Clans");
		}

		private void OnLoadTasksComplete(Platoon partyData = null)
		{
			if (partyData != null)
			{
				UpdateCanBeInvitedToPartyData(partyData);
			}
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "root", string.Empty));
		}

		private void HandlePartyDataChanged(Platoon partyData)
		{
			_receivedInvite = false;
			_playersListDataSource.SetPartyData(partyData);
			UpdateCanBeInvitedToPartyData(partyData);
			DispatchGenericMessage(new GenericComponentMessage(MessageType.UpdateView, "root", string.Empty));
		}

		private void UpdateCanBeInvitedToPartyData(Platoon partyData)
		{
			for (int i = 0; i < _playersListDataSource.NumberOfDataItemsAvailable(0); i++)
			{
				ClanMemberWithContextInfo clanMemberWithContextInfo = _playersListDataSource.QueryData<ClanMemberWithContextInfo>(i, 0);
				bool flag = false;
				if (partyData.Size > 0 && !string.IsNullOrEmpty(clanMemberWithContextInfo.Member.Name))
				{
					flag = partyData.HasPlayer(clanMemberWithContextInfo.Member.Name);
				}
				clanMemberWithContextInfo.CanBeInvitedToParty = (partyData.GetIsPlatoonLeader() && !_receivedInvite && _currentDisplayMode == ClanViewMode.YourClan && guiInputController.GetActiveScreen() != GuiScreens.BattleCountdown && !flag && partyData.Size != 5 && clanMemberWithContextInfo.Member.ClanMemberState == ClanMemberState.Accepted && User.Username != clanMemberWithContextInfo.Member.Name && clanMemberWithContextInfo.OnlineStatus);
			}
		}

		private void HandleGetClanInfoError(ServiceBehaviour behaviour)
		{
			GenericErrorData error = new GenericErrorData(behaviour.errorTitle, behaviour.errorBody);
			ErrorWindow.ShowErrorWindow(error);
			loadingIconPresenter.NotifyLoadingDone("Clans");
		}

		private void HandleLeaveClanError(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			GenericErrorData error = new GenericErrorData(behaviour.errorTitle, behaviour.errorBody);
			ErrorWindow.ShowErrorWindow(error);
		}

		private void HandleJoinClanError(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			SocialErrorCode errorCode = (SocialErrorCode)behaviour.errorCode;
			bool flag = true;
			switch (errorCode)
			{
			case SocialErrorCode.STR_SOCIAL_REASON_CLAN_FULL:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorJoinClanFull"));
				break;
			case SocialErrorCode.STR_SOCIAL_REASON_CLAN_CLOSED:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorJoinClanBanned"));
				break;
			case SocialErrorCode.STR_SOCIAL_REASON_NO_INVITE:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorJoinClanBanned"));
				break;
			case SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR:
				_labelDataContainer.PackData(StringTableBase<StringTable>.Instance.GetString("strErrorClanCreationUnexpected"));
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, string.Empty, "ErrorDialog"));
				DispatchGenericMessage(new GenericComponentMessage(MessageType.SetData, "yourClanErrorLabel", string.Empty, _labelDataContainer));
			}
		}
	}
}
