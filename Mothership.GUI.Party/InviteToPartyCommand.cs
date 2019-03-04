using Services.Web.Photon;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership.GUI.Party
{
	internal class InviteToPartyCommand : ICommand
	{
		private string _playerName;

		[Inject]
		internal PartyGUIController partyGUIController
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
		internal ISocialRequestFactory socialFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ChatPresenter chatPresenter
		{
			private get;
			set;
		}

		public ICommand Inject(string playerName)
		{
			_playerName = playerName;
			return this;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)ExecuteTask);
		}

		private IEnumerator ExecuteTask()
		{
			IRetrieveCustomGameSessionRequest retrieveRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> retrieveTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveRequest);
			yield return retrieveTask;
			if (!retrieveTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(retrieveTask.behaviour);
				yield break;
			}
			RetrieveCustomGameSessionRequestData customGame = retrieveTask.result;
			if (customGame.Response == CustomGameSessionRetrieveResponse.SessionRetrieved)
			{
				chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strCannotUsePartyInviteInCustomGame"));
				yield break;
			}
			if (customGame.Response == CustomGameSessionRetrieveResponse.PlayerIsInvitedOnly)
			{
				chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strCannotUsePartyInvitePendingCustomGameInvite"));
				yield break;
			}
			IGetPlayerCanBeInvitedToRegularPartyRequest request2 = socialFactory.Create<IGetPlayerCanBeInvitedToRegularPartyRequest>();
			request2.Inject(_playerName);
			TaskService<GetPlayerCanBeInvitedToRegularPartyResponseCode> task2 = new TaskService<GetPlayerCanBeInvitedToRegularPartyResponseCode>(request2);
			yield return task2;
			if (!task2.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(task2.behaviour);
				yield break;
			}
			if (task2.result == GetPlayerCanBeInvitedToRegularPartyResponseCode.TargetAlreadyHasOutstandingInvitationToCustomGame)
			{
				chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strCustomGameCannotInviteToRegularPartyOutstandingCustomInvite"));
				yield break;
			}
			if (task2.result == GetPlayerCanBeInvitedToRegularPartyResponseCode.TargetIsAlreadyInCustomGame)
			{
				chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strCustomGameCannotInviteToRegularPartyIsInCustomGame"));
				yield break;
			}
			if (task2.result == GetPlayerCanBeInvitedToRegularPartyResponseCode.NoNameProvided)
			{
				chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strPartyInviteNoNameProvidedErrorMessage"));
				yield break;
			}
			IGetPlatoonPendingInviteRequest request = socialFactory.Create<IGetPlatoonPendingInviteRequest>();
			TaskService<PlatoonInvite> task = new TaskService<PlatoonInvite>(request);
			yield return task;
			if (!task.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(task.behaviour);
				yield break;
			}
			PlatoonInvite invite = task.result;
			if (invite != null)
			{
				chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strCannotInviteToRegularPartyBecausePendingInvite"));
			}
			else
			{
				partyGUIController.SendPlatoonInvite(_playerName, delegate
				{
					Console.Log("Player " + _playerName + " invited to party");
				}, delegate(string errorMsg)
				{
					Console.Log("Player " + _playerName + " invitation failed, error:" + errorMsg);
				});
			}
		}
	}
}
