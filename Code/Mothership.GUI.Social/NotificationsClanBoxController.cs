using Fabric;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal sealed class NotificationsClanBoxController : NotificationsBoxControllerBase, IWaitForFrameworkDestruction
	{
		private IDataSource _yourClanOnlineMembersDataSource;

		private IServiceEventContainer _socialEventContainer;

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

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

		public void OnFrameworkDestroyed()
		{
			_socialEventContainer.Dispose();
		}

		public override void PlayAlertSound(GameObject source)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[132], 0, (object)null, source);
		}

		public override void SetView(NotificationsBoxView view)
		{
			base.SetView(view);
			_yourClanOnlineMembersDataSource = new YourClanOnlineMembersDataSource(socialRequestFactory);
			notificationsBoxLayoutFactory.Build(view, _yourClanOnlineMembersDataSource);
			CheckIfPlayerIsInClan();
			_socialEventContainer = socialEventContainerFactory.Create();
			RegisterToEventListeners();
		}

		public override void HandleGenericMessage(GenericComponentMessage genericMessage)
		{
			base.HandleGenericMessage(genericMessage);
			if (genericMessage.Message == MessageType.RefreshData)
			{
				CheckIfPlayerIsInClan();
			}
		}

		public override void HandleSignalChainMessage(object Message)
		{
			if (Message is ClanInviteListChangedMessage)
			{
				HandleOnClanInviteDataChanged((Message as ClanInviteListChangedMessage).ClanInvitations);
			}
		}

		private void CheckIfPlayerIsInClan()
		{
			IGetMyClanInfoRequest getMyClanInfoRequest = socialRequestFactory.Create<IGetMyClanInfoRequest>();
			getMyClanInfoRequest.SetAnswer(new ServiceAnswer<ClanInfo>(HandleOnClanInfoRetrieved, HandleGetClanInfoError)).Execute();
		}

		private void RegisterToEventListeners()
		{
			_socialEventContainer.ListenTo<IClanMemberDataChangedEventListener, ClanMember[], ClanMemberDataChangedEventContent>(HandleOnClanMemberDataChanged);
			_socialEventContainer.ListenTo<IClanMemberJoinedEventListener, ClanMember[], ClanMember>(HandleOnClanMemberDataChanged);
			_socialEventContainer.ListenTo<IClanMemberLeftEventListener, ClanMember[], ClanMember>(HandleOnClanMemberDataChanged);
			_socialEventContainer.ListenTo<IClanInviteReceivedEventListener, ClanInvite, ClanInvite[]>(HandleOnClanInviteDataChanged);
			_socialEventContainer.ListenTo<IClanInviteCancelledEventListener, ClanInvite[]>(HandleOnClanInviteDataChanged);
		}

		private void HandleOnClanMemberDataChanged(ClanMember[] clanMembers, ClanMember newMember)
		{
			HandleOnClanMemberDataChanged(clanMembers);
		}

		private void HandleOnClanMemberDataChanged(ClanMember[] clanMembers, ClanMemberDataChangedEventContent _)
		{
			HandleOnClanMemberDataChanged(clanMembers);
		}

		private void HandleOnClanMemberDataChanged(ClanMember[] clanMembers)
		{
			(_yourClanOnlineMembersDataSource as YourClanOnlineMembersDataSource).OnClanMemberDataChange(clanMembers);
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, _view.Name, string.Empty));
		}

		private void HandleOnClanInviteDataChanged(ClanInvite invite, ClanInvite[] allInvites)
		{
			HandleOnClanInviteDataChanged(allInvites);
		}

		private void HandleOnClanInviteDataChanged(ClanInvite[] invites)
		{
			if (invites == null)
			{
				_view.ShowAlert(show: false);
			}
			else
			{
				_view.ShowAlert(invites.Length > 0);
			}
		}

		private void HandleOnClanInfoRetrieved(ClanInfo myClanInfo)
		{
			if (myClanInfo != null)
			{
				_view.ShowLabel(visible: true);
				RefreshData();
			}
			else
			{
				_view.ShowLabel(visible: false);
				CheckIfHavePendingInvites();
			}
		}

		private void HandleGetClanInfoError(ServiceBehaviour behaviour)
		{
			GenericErrorData error = new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, Localization.Get("strOK", true));
			ErrorWindow.ShowErrorWindow(error);
		}

		private void CheckIfHavePendingInvites()
		{
			IGetClanInvitesRequest getClanInvitesRequest = socialRequestFactory.Create<IGetClanInvitesRequest>();
			getClanInvitesRequest.SetAnswer(new ServiceAnswer<ClanInvite[]>(HandleOnClanInvitesRetrieved, HandleGetClanInvitesError)).Execute();
		}

		private void HandleGetClanInvitesError(ServiceBehaviour behaviour)
		{
			GenericErrorData error = new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, Localization.Get("strOK", true));
			ErrorWindow.ShowErrorWindow(error);
		}

		private void HandleOnClanInvitesRetrieved(ClanInvite[] invites)
		{
			if (invites != null)
			{
				_view.ShowAlert(invites.Length > 0);
			}
		}

		private void RefreshData()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			ParallelTaskCollection val = new ParallelTaskCollection();
			val.Add(_yourClanOnlineMembersDataSource.RefreshData());
			val.add_onComplete((Action)OnTaskComplete);
			TaskRunner.get_Instance().Run((IEnumerator)val);
		}

		private void OnTaskComplete()
		{
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, _view.Name, string.Empty));
		}
	}
}
