using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class ClanInvitesController : ClanSectionControllerBase
	{
		private ClanInvitesView _view;

		private InviteesListDataSource _inviteesListDataSource;

		private InviteesHeadersListDataSource _inviteesListHeadersDataSource;

		private IServiceEventContainer _socialEventContainer;

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ClanInvitesLayoutFactory clanInvitesLayoutFactory
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
		internal ICommandFactory commandFactory
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

		public override ClanSectionType SectionType => ClanSectionType.ClanInvites;

		public override void PushCurrentState()
		{
		}

		public override void PopPreviousState()
		{
		}

		public override void HandleClanMessageDerived(SocialMessage message)
		{
			if (message.messageDispatched == SocialMessageType.DataNeedsRefreshing && message.extraDetails == "invitees")
			{
				_inviteesListDataSource.RefreshData(delegate
				{
					DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "root", string.Empty));
				}, HandleRefreshDataFailed);
			}
		}

		private void HandleRefreshDataFailed(ServiceBehaviour behaviour)
		{
		}

		public override void HandleGenericMessage(GenericComponentMessage receivedMessage)
		{
			if (receivedMessage.Message == MessageType.ButtonClicked && receivedMessage.Originator == "declineall")
			{
				commandFactory.Build<DeclineAllPendingInvitesCommand>().Execute();
			}
			if (receivedMessage.Message == MessageType.ButtonWithinListClicked && receivedMessage.Originator == "inviteeslist" && receivedMessage.Message == MessageType.ButtonWithinListClicked)
			{
				InviteeListItemComponentDataContainer.ListItemInfo listItemInfo = receivedMessage.Data.UnpackData<InviteeListItemComponentDataContainer.ListItemInfo>();
				if (listItemInfo.action == InviteeListItemComponentDataContainer.ListItemAction.Accept)
				{
					AcceptPendingInvitationCommandDependancy input = new AcceptPendingInvitationCommandDependancy(listItemInfo.nameOfClan, listItemInfo.nameOfPlayer);
					commandFactory.Build<AcceptPendingInvitationCommand>().Inject(input).Execute();
					_clanView.BubbleSocialMessageUp(new SocialMessage(SocialMessageType.ClanJoined, string.Empty));
				}
				if (listItemInfo.action == InviteeListItemComponentDataContainer.ListItemAction.Decline)
				{
					DeclinePendingInvitationCommandDependancy input2 = new DeclinePendingInvitationCommandDependancy(listItemInfo.nameOfClan, listItemInfo.nameOfPlayer);
					commandFactory.Build<DeclinePendingInvitationCommand>().Inject(input2).Execute();
				}
			}
		}

		private void HandleOnClanInviteCancelledEvent(ClanInvite[] allInvites)
		{
			RefreshData();
			if (allInvites.Length == 0)
			{
				base.clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.SingleTabOnTopBackClicked, string.Empty));
				base.clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.AllInvitationsDealtWith, string.Empty));
			}
		}

		private void HandleOnClanInviteReceivedEvent(ClanInvite invite, ClanInvite[] allInvites)
		{
			RefreshData();
		}

		public override void OnSetupController()
		{
			_inviteesListDataSource = new InviteesListDataSource(socialRequestFactory);
			_inviteesListHeadersDataSource = new InviteesHeadersListDataSource(socialRequestFactory);
			_socialEventContainer = socialEventContainerFactory.Create();
			_socialEventContainer.ListenTo<IClanInviteReceivedEventListener, ClanInvite, ClanInvite[]>(HandleOnClanInviteReceivedEvent);
			_socialEventContainer.ListenTo<IClanInviteCancelledEventListener, ClanInvite[]>(HandleOnClanInviteCancelledEvent);
		}

		private void RefreshData()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			ParallelTaskCollection val = new ParallelTaskCollection();
			val.Add(_inviteesListDataSource.RefreshData());
			val.Add(_inviteesListHeadersDataSource.RefreshData());
			val.add_onComplete((Action)OnLoadTasksComplete);
			TaskRunner.get_Instance().Run((IEnumerator)val);
		}

		private void OnLoadTasksComplete()
		{
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "root", string.Empty));
		}

		public override void OnViewSet(ClanSectionViewBase view)
		{
			_view = (ClanInvitesView)view;
		}

		public void BuildLayout(IContainer container)
		{
			clanInvitesLayoutFactory.BuildAll(_view, _inviteesListDataSource, _inviteesListHeadersDataSource, container);
			RefreshData();
		}
	}
}
