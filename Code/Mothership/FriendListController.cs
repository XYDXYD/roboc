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
	internal class FriendListController : FriendSectionControllerBase, IWaitForFrameworkDestruction
	{
		private BubbleSignal<ISocialRoot> _bubble;

		private FriendListView _friendListView;

		private FriendListDataSource _friendListDataSource;

		private FriendListHeadersDataSource _friendListHeadersDataSource;

		private IServiceEventContainer _socialEventContainer;

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
		internal FriendListLayoutFactory friendListLayoutFactory
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
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IContextNotifer contextNotifier
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
		internal LocalisationWrapper localiseWrapper
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
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal FriendPopupMenuController popupMenuController
		{
			private get;
			set;
		}

		public override FriendSectionType SectionType => FriendSectionType.FriendList;

		public void OnFrameworkDestroyed()
		{
			if (_socialEventContainer != null)
			{
				_socialEventContainer.Dispose();
				_socialEventContainer = null;
			}
			guiInputController.OnScreenStateChange -= OnGuiScreenChanged;
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Remove(localiseWrapper.OnLocalisationChanged, new Action(RefreshData));
			_friendListDataSource.Dispose();
		}

		public override void HandleFriendMessageDerived(SocialMessage message)
		{
			if (message.messageDispatched == SocialMessageType.Refresh)
			{
				IList<Friend> list = message.extraData as IList<Friend>;
				if (list != null)
				{
					UpdateFriendList(list);
				}
			}
		}

		public override void HandleGenericMessage(GenericComponentMessage receivedMessage)
		{
			if (receivedMessage.Originator == "AddFriendButton" && receivedMessage.Message == MessageType.ButtonClicked)
			{
				DispatchGenericMessage(new GenericComponentMessage(MessageType.Show, "FriendListRoot", "FriendInvitationRoot"));
				receivedMessage.Consume();
			}
			else if (receivedMessage.Originator == "CloseFriendScreenButton" && receivedMessage.Message == MessageType.ButtonClicked)
			{
				base.friendController.HandleSocialMessage(new SocialMessage(SocialMessageType.MaximizeFriendScreen, string.Empty));
				receivedMessage.Consume();
			}
			else if (receivedMessage.Message == MessageType.RefreshData && receivedMessage.Originator == "FriendInvitationRoot")
			{
				RefreshData();
			}
			else if (receivedMessage.Message == MessageType.ButtonWithinListClicked)
			{
				FriendListItemComponentDataContainer.ListItemInfo listItemInfo = receivedMessage.Data.UnpackData<FriendListItemComponentDataContainer.ListItemInfo>();
				HandleItemButtons(listItemInfo.friend, listItemInfo.buttonClicked);
			}
		}

		public override void HandleMessage(object message)
		{
			if (message is SocialReceivedPartyInviteMessage)
			{
				SocialReceivedPartyInviteMessage socialReceivedPartyInviteMessage = message as SocialReceivedPartyInviteMessage;
				_friendListDataSource.SetPendingPartyInvite(socialReceivedPartyInviteMessage.pendingInvite);
				DispatchGenericMessage(new GenericComponentMessage(MessageType.UpdateView, "root", string.Empty));
			}
		}

		private void OnGuiScreenChanged()
		{
			bool isInQueue = guiInputController.GetActiveScreen() == GuiScreens.BattleCountdown;
			_friendListDataSource.SetIsInQueue(isInQueue);
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "root", string.Empty));
		}

		public void AcceptInvite(Friend friend)
		{
			if (IsMe(friend.Name))
			{
				ShowError(StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_USER_IS_SELF.ToString()));
				return;
			}
			if (friend == null || friend.InviteStatus != FriendInviteStatus.InvitePending)
			{
				ShowError(StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR.ToString()));
				return;
			}
			IServiceRequest serviceRequest = socialRequestFactory.Create<IAcceptFriendRequest, string>(friend.Name).SetAnswer(new ServiceAnswer<IList<Friend>>(UpdateFriendList, OnError));
			serviceRequest.Execute();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleAnalytics);
		}

		private IEnumerator HandleAnalytics()
		{
			TaskService logFriendAcceptedRequest = analyticsRequestFactory.Create<ILogFriendAcceptInviteRequest>().AsTask();
			yield return logFriendAcceptedRequest;
			if (!logFriendAcceptedRequest.succeeded)
			{
				throw new Exception("Log Friend Accepted Request failed", logFriendAcceptedRequest.behaviour.exceptionThrown);
			}
		}

		public void DeclineInvite(Friend friend)
		{
			if (IsMe(friend.Name))
			{
				ShowError(StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_USER_IS_SELF.ToString()));
				return;
			}
			if (friend == null || friend.InviteStatus != FriendInviteStatus.InvitePending)
			{
				ShowError(StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR.ToString()));
				return;
			}
			IServiceRequest serviceRequest = socialRequestFactory.Create<IDeclineFriendRequest, string>(friend.Name).SetAnswer(new ServiceAnswer<IList<Friend>>(UpdateFriendList, OnError));
			serviceRequest.Execute();
		}

		public void CancelInvite(Friend friend)
		{
			if (IsMe(friend.Name))
			{
				ShowError(StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_USER_IS_SELF.ToString()));
				return;
			}
			if (friend == null || friend.InviteStatus != 0)
			{
				ShowError(StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR.ToString()));
				return;
			}
			IServiceRequest serviceRequest = socialRequestFactory.Create<ICancelFriendRequest, string>(friend.Name).SetAnswer(new ServiceAnswer<IList<Friend>>(UpdateFriendList, OnError));
			serviceRequest.Execute();
		}

		public void RemoveFriend(Friend friend)
		{
			if (IsMe(friend.Name))
			{
				ShowError(StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_USER_IS_SELF.ToString()));
				return;
			}
			if (friend == null || friend.InviteStatus != FriendInviteStatus.Accepted)
			{
				ShowError(StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR.ToString()));
				return;
			}
			IServiceRequest serviceRequest = socialRequestFactory.Create<IRemoveFriendRequest, string>(friend.Name).SetAnswer(new ServiceAnswer<IList<Friend>>(UpdateFriendList, OnError));
			serviceRequest.Execute();
		}

		private void InviteToPlatoon(Friend friend)
		{
			_bubble.TargetedDispatch<SocialScreenQuickInviteToPartyMessage>(new SocialScreenQuickInviteToPartyMessage(friend.Name));
		}

		public override void OnSetupController()
		{
			_friendListDataSource = new FriendListDataSource(socialRequestFactory, webRequestFactory, avatarLoader, avatarAvailableObserver);
			_friendListHeadersDataSource = new FriendListHeadersDataSource(socialRequestFactory, webRequestFactory, avatarLoader, avatarAvailableObserver);
			_socialEventContainer = socialEventContainerFactory.Create();
			_socialEventContainer.ReconnectedEvent += RefreshData;
			_socialEventContainer.ListenTo<IFriendInviteEventListener, FriendListUpdate>(RefreshData);
			_socialEventContainer.ListenTo<IFriendDeclineEventListener, FriendListUpdate>(RefreshData);
			_socialEventContainer.ListenTo<IFriendRemovedEventListener, FriendListUpdate>(RefreshData);
			_socialEventContainer.ListenTo<IFriendCancelledEventListener, FriendListUpdate>(RefreshData);
			_socialEventContainer.ListenTo<IFriendAcceptEventListener, FriendListUpdate>(OnFriendAcceptEvent);
			_socialEventContainer.ListenTo<IFriendStatusEventListener, Friend, IList<Friend>>(RefreshData);
			_socialEventContainer.ListenTo<IAllFriendsOfflineEventListener, IList<Friend>>(RefreshData);
			_socialEventContainer.ListenTo<IAvatarUpdatedEventListener, AvatarUpdatedUpdate>(OnFriendAvatarUpdated);
			_socialEventContainer.ListenTo<IFriendClanChangedEventListener, FriendClanChangedEventArgs>(RefreshData);
			_socialEventContainer.ListenTo<IPlatoonChangedEventListener, Platoon>(HandlePartyChangedEvent);
			guiInputController.OnScreenStateChange += OnGuiScreenChanged;
			RefreshData();
			contextNotifier.AddFrameworkDestructionListener(this);
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Combine(localiseWrapper.OnLocalisationChanged, new Action(RefreshData));
		}

		private void RefreshData(IList<Friend> obj)
		{
			RefreshData();
		}

		private void RefreshData(Friend arg1, IList<Friend> arg2)
		{
			RefreshData();
		}

		private void RefreshData(FriendListUpdate obj)
		{
			RefreshData();
		}

		private void RefreshData(FriendClanChangedEventArgs obj)
		{
			RefreshData();
		}

		private void OnFriendAvatarUpdated(AvatarUpdatedUpdate data)
		{
			if (data.avatarInfo.UseCustomAvatar)
			{
				avatarLoader.ForceRequestAvatar(AvatarType.PlayerAvatar, data.user);
			}
			RefreshData();
		}

		private void OnFriendAcceptEvent(FriendListUpdate update)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[134], 0, (object)null, _friendListView.get_gameObject());
			RefreshData();
		}

		public override void OnViewSet(FriendSectionViewBase view)
		{
			_friendListView = (view as FriendListView);
		}

		public override void BuildLayout(IContainer container)
		{
			friendListLayoutFactory.BuildAll(_friendListView, _friendListDataSource, _friendListHeadersDataSource, container);
			_bubble = new BubbleSignal<ISocialRoot>(_friendListView.get_transform());
			RefreshData();
		}

		private void HandleItemButtons(Friend friend, string buttonName)
		{
			if (buttonName == null)
			{
				return;
			}
			if (!(buttonName == "TickButton"))
			{
				if (!(buttonName == "CrossButton"))
				{
					if (!(buttonName == "PlusButton"))
					{
						if (buttonName == "ListItemBackgroundClick" && friend.InviteStatus != FriendInviteStatus.InvitePending && friend.InviteStatus != 0)
						{
							popupMenuController.ShowPopupMenuForContext(friend);
						}
					}
					else
					{
						InviteToPlatoon(friend);
					}
				}
				else if (friend.InviteStatus == FriendInviteStatus.InvitePending)
				{
					DeclineInvite(friend);
				}
				else if (friend.InviteStatus == FriendInviteStatus.InviteSent)
				{
					CancelInvite(friend);
				}
				else
				{
					ShowError(StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR.ToString()));
				}
			}
			else
			{
				AcceptInvite(friend);
			}
		}

		private void UpdateFriendList(IList<Friend> FriendList)
		{
			_bubble.Dispatch<FriendListChangedMessage>(new FriendListChangedMessage(FriendList));
			RefreshData();
		}

		private void OnError(ServiceBehaviour serviceBehaviour)
		{
			SocialErrorCode errorCode = (SocialErrorCode)serviceBehaviour.errorCode;
			if (errorCode == SocialErrorCode.STR_SOCIAL_REASON_UNEXPECTED_ERROR)
			{
				serviceBehaviour.SetAlternativeBehaviour(delegate
				{
				}, StringTableBase<StringTable>.Instance.GetString("strCancel"));
				ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
			}
			else
			{
				ShowError(StringTableBase<StringTable>.Instance.GetString(errorCode.ToString()));
			}
		}

		private void ShowError(string errorMessage)
		{
			GenericErrorData error = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strUnexpectedError"), errorMessage);
			ErrorWindow.ShowErrorWindow(error);
		}

		private bool IsMe(string name)
		{
			return string.Compare(name, User.Username, ignoreCase: true) == 0;
		}

		private void HandlePartyChangedEvent(Platoon platoon)
		{
			_friendListDataSource.SetPartyData(platoon);
			_friendListDataSource.SetPendingPartyInvite(receivedPending: false);
			DispatchGenericMessage(new GenericComponentMessage(MessageType.UpdateView, "root", string.Empty));
		}

		private void RefreshData()
		{
			TaskRunner.get_Instance().Run(RefreshDataWithEnumerator());
		}

		private IEnumerator RefreshDataWithEnumerator()
		{
			loadingIconPresenter.NotifyLoading("Friends");
			ServiceBehaviour errorBehaviour = null;
			ParallelTaskCollection loadFriendsData = new ParallelTaskCollection();
			loadFriendsData.Add(_friendListDataSource.RefreshDataWithEnumerator(delegate
			{
			}, delegate(ServiceBehaviour sb)
			{
				errorBehaviour = sb;
			}));
			loadFriendsData.Add(_friendListHeadersDataSource.RefreshDataWithEnumerator(delegate
			{
			}, delegate(ServiceBehaviour sb)
			{
				errorBehaviour = sb;
			}));
			yield return loadFriendsData;
			loadingIconPresenter.NotifyLoadingDone("Friends");
			if (errorBehaviour != null)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(Localization.Get("strErrorFreshFriendListDataTitle", true), Localization.Get("strErrorFreshFriendListDataBody", true), Localization.Get("strRetry", true), Localization.Get("strCancel", true), RefreshData, delegate
				{
				}));
			}
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "root", string.Empty));
		}
	}
}
