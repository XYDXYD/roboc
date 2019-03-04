using Fabric;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal sealed class NotificationsFriendBoxController : NotificationsBoxControllerBase, IWaitForFrameworkDestruction
	{
		private FriendListDataSource _friendListDataSource;

		private IServiceEventContainer _socialEventContainer;

		[Inject]
		internal NotificationsBoxLayoutFactory notificationsBoxLayoutFactory
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
		internal IServiceRequestFactory webRequestFactory
		{
			private get;
			set;
		}

		public void OnFrameworkDestroyed()
		{
			_socialEventContainer.Dispose();
		}

		public override void PlayAlertSound(GameObject source)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[136], 0, (object)null, source);
		}

		public override void SetView(NotificationsBoxView view)
		{
			base.SetView(view);
			_friendListDataSource = new FriendListDataSource(socialRequestFactory, webRequestFactory, null, null);
			notificationsBoxLayoutFactory.Build(view, _friendListDataSource, -2);
			RefreshData();
			_socialEventContainer = socialEventContainerFactory.Create();
			RegisterToEventListeners();
		}

		private void RegisterToEventListeners()
		{
			_socialEventContainer.ListenTo<IFriendStatusEventListener, Friend, IList<Friend>>(OnFriendStatusChange);
			_socialEventContainer.ListenTo<IFriendAcceptEventListener, FriendListUpdate>(OnFriendDataChange);
			_socialEventContainer.ListenTo<IFriendInviteEventListener, FriendListUpdate>(OnFriendDataChange);
			_socialEventContainer.ListenTo<IFriendRemovedEventListener, FriendListUpdate>(OnFriendDataChange);
			_socialEventContainer.ListenTo<IFriendCancelledEventListener, FriendListUpdate>(OnFriendDataChange);
			_socialEventContainer.ListenTo<IFriendDeclineEventListener, FriendListUpdate>(OnFriendDataChange);
		}

		private void OnFriendDataChange(IList<Friend> friendList)
		{
			RefreshData();
		}

		private void OnFriendDataChange(FriendListUpdate friendListUpdate)
		{
			OnFriendDataChange(friendListUpdate.friendList);
		}

		private void OnFriendStatusChange(Friend friend, IList<Friend> friendList)
		{
			OnFriendDataChange(friendList);
		}

		public override void HandleGenericMessage(GenericComponentMessage genericMessage)
		{
		}

		public override void HandleSignalChainMessage(object Message)
		{
			if (Message is FriendListChangedMessage)
			{
				OnFriendDataChange((Message as FriendListChangedMessage).FriendList);
			}
		}

		private void RefreshData()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			ParallelTaskCollection val = new ParallelTaskCollection();
			val.Add(_friendListDataSource.RefreshData());
			val.add_onComplete((Action)OnTaskComplete);
			TaskRunner.get_Instance().Run((IEnumerator)val);
		}

		private void OnTaskComplete()
		{
			int num = _friendListDataSource.NumberOfDataItemsAvailable(0);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				FriendListEntryData friendListEntryData = _friendListDataSource.QueryData<FriendListEntryData>(i, 0);
				if (friendListEntryData.Friend.InviteStatus == FriendInviteStatus.InvitePending)
				{
					num2++;
				}
			}
			_view.ShowAlert(num2 > 0);
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, _view.Name, string.Empty));
		}
	}
}
