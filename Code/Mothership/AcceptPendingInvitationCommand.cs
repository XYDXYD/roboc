using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;

namespace Mothership
{
	internal class AcceptPendingInvitationCommand : IInjectableCommand<AcceptPendingInvitationCommandDependancy>, ICommand
	{
		private AcceptPendingInvitationCommandDependancy _dependancy;

		private ChangeTabTypeData _tabTypeData = new ChangeTabTypeData(0, ClanSectionType.YourClan);

		[Inject]
		internal ClanController clanController
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
		internal ISocialRequestFactory socialRequestFactory
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

		public void Execute()
		{
			loadingIconPresenter.NotifyLoading("Clans");
			IAcceptClanInviteRequest acceptClanInviteRequest = socialRequestFactory.Create<IAcceptClanInviteRequest>();
			acceptClanInviteRequest.Inject(_dependancy.ClanName);
			acceptClanInviteRequest.SetAnswer(new ServiceAnswer<ClanInfoAndMembers>(HandleOnAcceptSuccess, HandleOnAcceptFail));
			acceptClanInviteRequest.Execute();
		}

		public void HandleOnAcceptSuccess(ClanInfoAndMembers clanInfoAndMembers)
		{
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.SingleTabOnTopBackClicked, string.Empty, string.Empty));
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.ChangeTabTypeAndSelect, string.Empty, _tabTypeData));
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.ConfigureYourClanData, string.Empty));
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.AllInvitationsDealtWith, string.Empty));
			clanController.DispatchSignalChainMessage(new GenericComponentMessage(MessageType.RefreshData, string.Empty, "ClanNotificationsBoxRoot"));
			clanController.DispatchSignalChainMessage(new ClanInviteListChangedMessage(null));
			loadingIconPresenter.NotifyLoadingDone("Clans");
			CommandFactory.Build<JoinClanChatChannelCommand>().Inject(clanInfoAndMembers.ClanInfo.ClanName).Execute();
		}

		public void HandleOnAcceptFail(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			ErrorWindow.ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, behaviour.mainText, behaviour.alternativeText, delegate
			{
				loadingIconPresenter.NotifyLoading("Clans");
				behaviour.MainBehaviour();
			}, behaviour.Alternative));
		}

		public ICommand Inject(AcceptPendingInvitationCommandDependancy input)
		{
			_dependancy = input;
			return this;
		}
	}
}
