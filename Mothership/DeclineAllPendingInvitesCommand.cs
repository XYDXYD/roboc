using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;

namespace Mothership
{
	internal class DeclineAllPendingInvitesCommand : IInjectableCommand<string>, ICommand
	{
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
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		public void Execute()
		{
			loadingIconPresenter.NotifyLoading("Clans");
			IDeclineAllClanInvitesRequest declineAllClanInvitesRequest = socialRequestFactory.Create<IDeclineAllClanInvitesRequest>();
			declineAllClanInvitesRequest.SetAnswer(new ServiceAnswer(HandleOnDeclineSuccess, HandleOnDeclineFail));
			declineAllClanInvitesRequest.Execute();
		}

		public void HandleOnDeclineSuccess()
		{
			clanController.DispatchSignalChainMessage(new ClanInviteListChangedMessage(null));
			loadingIconPresenter.NotifyLoadingDone("Clans");
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.SingleTabOnTopBackClicked, string.Empty));
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.AllInvitationsDealtWith, string.Empty));
		}

		public void HandleOnDeclineFail(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			ErrorWindow.ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, behaviour.mainText, behaviour.alternativeText, delegate
			{
				loadingIconPresenter.NotifyLoading("Clans");
				behaviour.MainBehaviour();
			}, behaviour.Alternative));
		}

		public ICommand Inject(string input)
		{
			return this;
		}
	}
}
