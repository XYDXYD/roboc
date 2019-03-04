using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class DeclinePendingInvitationCommand : IInjectableCommand<DeclinePendingInvitationCommandDependancy>, ICommand
	{
		private ClanInvite[] _moreInvites;

		private DeclinePendingInvitationCommandDependancy _dependancy;

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

		public void Execute()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			loadingIconPresenter.NotifyLoading("Clans");
			SerialTaskCollection val = new SerialTaskCollection();
			val.Add(DeclineClanInviteRequest());
			val.Add(GetPendingDataRequest());
			val.Add(HandleDeclinePendingFinished());
			TaskRunner.get_Instance().Run((IEnumerator)val);
		}

		private IEnumerator HandleDeclinePendingFinished()
		{
			clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.DataNeedsRefreshing, "invitees"));
			clanController.DispatchSignalChainMessage(new ClanInviteListChangedMessage(_moreInvites));
			if (_moreInvites.Length == 0)
			{
				clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.SingleTabOnTopBackClicked, string.Empty));
				clanController.DispatchAnyClanMessage(new SocialMessage(SocialMessageType.AllInvitationsDealtWith, string.Empty));
			}
			loadingIconPresenter.NotifyLoadingDone("Clans");
			yield return null;
		}

		private IEnumerator GetPendingDataRequest()
		{
			IGetClanInvitesRequest clanInvitesRequest = socialRequestFactory.Create<IGetClanInvitesRequest>();
			clanInvitesRequest.ForceRefresh = true;
			TaskService<ClanInvite[]> requestTask = clanInvitesRequest.AsTask();
			yield return requestTask;
			if (requestTask.succeeded)
			{
				_moreInvites = requestTask.result;
				Console.Log(requestTask.result.Length + " items remaining");
			}
			else
			{
				HandleRequestFailure(requestTask.behaviour);
			}
		}

		private IEnumerator DeclineClanInviteRequest()
		{
			IDeclineClanInviteRequest declineInvitationRequest = socialRequestFactory.Create<IDeclineClanInviteRequest>();
			declineInvitationRequest.Inject(_dependancy.ClanName);
			TaskService requestTask = declineInvitationRequest.AsTask();
			yield return requestTask;
			if (!requestTask.succeeded)
			{
				HandleRequestFailure(requestTask.behaviour);
			}
		}

		public void HandleRequestFailure(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			ErrorWindow.ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, behaviour.mainText, behaviour.alternativeText, delegate
			{
				loadingIconPresenter.NotifyLoading("Clans");
				behaviour.MainBehaviour();
			}, behaviour.Alternative));
		}

		public ICommand Inject(DeclinePendingInvitationCommandDependancy input)
		{
			_dependancy = input;
			return this;
		}
	}
}
