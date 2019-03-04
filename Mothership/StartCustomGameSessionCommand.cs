using Authentication;
using CustomGames;
using Services.Web.Photon;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class StartCustomGameSessionCommand : ICommand
	{
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
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			get;
			private set;
		}

		[Inject]
		internal CustomGameStateObservable stateObservable
		{
			get;
			private set;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)StartCustomGameSessionTask);
		}

		public IEnumerator StartCustomGameSessionTask()
		{
			IEnumerator refreshEnumerator = RefreshCachedGameSession();
			yield return refreshEnumerator;
			if (refreshEnumerator.Current is ServiceBehaviour)
			{
				ErrorWindow.ShowServiceErrorWindow(refreshEnumerator.Current as ServiceBehaviour);
				yield return null;
			}
			RetrieveCustomGameSessionRequestData resultData = refreshEnumerator.Current as RetrieveCustomGameSessionRequestData;
			if (resultData.Response == CustomGameSessionRetrieveResponse.PlayerIsInvitedOnly)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameError"), StringTableBase<StringTable>.Instance.GetString("strCustomGameCannotStartGameBecauseHasBeenInvitedElsewhere")));
				yield return null;
			}
			if (resultData.Response == CustomGameSessionRetrieveResponse.SessionRetrieved)
			{
				RemoteLogger.Error("executed start custom game session while still in a session", $"UserName={User.Username}", null);
			}
			yield return LeaveMultiplayerPartyIfInParty();
			yield return DeclineMultiplayerPartyIfIsInvited();
			ICreateCustomGameSessionRequest createRequest = serviceFactory.Create<ICreateCustomGameSessionRequest>();
			createRequest.ClearCache();
			TaskService<SessionCreationResponseCode> createTask = new TaskService<SessionCreationResponseCode>(createRequest);
			loadingIconPresenter.NotifyLoading("CreateCustomGame");
			yield return new HandleTaskServiceWithError(createTask, delegate
			{
				loadingIconPresenter.NotifyLoading("CreateCustomGame");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CreateCustomGame");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("CreateCustomGame");
			if (!createTask.succeeded)
			{
				yield break;
			}
			if (createTask.result == SessionCreationResponseCode.AlreadyInSession)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameError"), StringTableBase<StringTable>.Instance.GetString("strCustomGameSessionCreateFailAlreadyInSession")));
			}
			if (createTask.result == SessionCreationResponseCode.SessionCreateError)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameError"), StringTableBase<StringTable>.Instance.GetString("strCustomGameSessionCreateFail")));
			}
			if (createTask.result == SessionCreationResponseCode.SessionCreated)
			{
				IEnumerator refreshEnumeratorAfterCreate = RefreshCachedGameSession();
				yield return refreshEnumeratorAfterCreate;
				if (refreshEnumeratorAfterCreate.Current is ServiceBehaviour)
				{
					ErrorWindow.ShowServiceErrorWindow(refreshEnumeratorAfterCreate.Current as ServiceBehaviour);
					yield return null;
				}
				guiInputController.ToggleScreen(GuiScreens.CustomGameScreen);
				RetrieveCustomGameSessionRequestData session = (RetrieveCustomGameSessionRequestData)refreshEnumeratorAfterCreate.Current;
				CustomGameStateDependency dep = new CustomGameStateDependency(session.Data.SessionGUID);
				stateObservable.Dispatch(ref dep);
			}
		}

		public IEnumerator DeclineMultiplayerPartyIfIsInvited()
		{
			loadingIconPresenter.NotifyLoading("CheckIsInMultiplayerParty");
			IGetPlatoonPendingInviteRequest getPlatoonInviteRequest = socialRequestFactory.Create<IGetPlatoonPendingInviteRequest>();
			TaskService<PlatoonInvite> getPlatooninviteTask = new TaskService<PlatoonInvite>(getPlatoonInviteRequest);
			yield return new HandleTaskServiceWithError(getPlatooninviteTask, delegate
			{
				loadingIconPresenter.NotifyLoading("CheckIsInMultiplayerParty");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CheckIsInMultiplayerParty");
			}).GetEnumerator();
			if (!getPlatooninviteTask.succeeded)
			{
				yield break;
			}
			loadingIconPresenter.NotifyLoadingDone("CheckIsInMultiplayerParty");
			if (getPlatooninviteTask.result != null)
			{
				loadingIconPresenter.NotifyLoading("DeclineInvitation");
				IDeclinePlatoonInviteRequest declineRequest = socialRequestFactory.Create<IDeclinePlatoonInviteRequest>();
				TaskService declinePlatoonInviteTask = new TaskService(declineRequest);
				yield return new HandleTaskServiceWithError(declinePlatoonInviteTask, delegate
				{
					loadingIconPresenter.NotifyLoading("DeclineInvitation");
				}, delegate
				{
					loadingIconPresenter.NotifyLoadingDone("DeclineInvitation");
				}).GetEnumerator();
				if (declinePlatoonInviteTask.succeeded)
				{
					loadingIconPresenter.NotifyLoadingDone("DeclineInvitation");
				}
			}
		}

		public IEnumerator LeaveMultiplayerPartyIfInParty()
		{
			loadingIconPresenter.NotifyLoading("CheckIsInMultiplayerParty");
			IGetPlatoonDataRequest getPlatoonDataRequest = socialRequestFactory.Create<IGetPlatoonDataRequest>();
			TaskService<Platoon> getPlatoonTask = new TaskService<Platoon>(getPlatoonDataRequest);
			yield return new HandleTaskServiceWithError(getPlatoonTask, delegate
			{
				loadingIconPresenter.NotifyLoading("CheckIsInMultiplayerParty");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CheckIsInMultiplayerParty");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("CheckIsInMultiplayerParty");
			if (getPlatoonTask.succeeded && getPlatoonTask.result.isInPlatoon)
			{
				loadingIconPresenter.NotifyLoading("ForceLeaveMultiplayerParty");
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
			}
		}

		public IEnumerator RefreshCachedGameSession()
		{
			IRetrieveCustomGameSessionRequest retrieveRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			retrieveRequest.ClearCache();
			TaskService<RetrieveCustomGameSessionRequestData> retrieveTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveRequest);
			loadingIconPresenter.NotifyLoading("RetrieveCustomGame");
			yield return retrieveTask;
			loadingIconPresenter.NotifyLoadingDone("RetrieveCustomGame");
			if (!retrieveTask.succeeded)
			{
				yield return retrieveTask.behaviour;
			}
			else
			{
				yield return retrieveTask.result;
			}
		}
	}
}
