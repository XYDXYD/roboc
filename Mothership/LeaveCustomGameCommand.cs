using CustomGames;
using Fabric;
using Services.Web.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class LeaveCustomGameCommand : ICommand
	{
		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameScreenController customGameScreenController
		{
			get;
			private set;
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
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LeaveCustomGameTask);
		}

		public IEnumerator LeaveCustomGameTask()
		{
			ILeaveCustomGameSessionRequest leaveService = serviceFactory.Create<ILeaveCustomGameSessionRequest>();
			leaveService.ClearCache();
			TaskService<SessionLeaveResponseCode> leaveTask = new TaskService<SessionLeaveResponseCode>(leaveService);
			loadingIconPresenter.NotifyLoading("LeaveCustomGame");
			yield return leaveTask;
			loadingIconPresenter.NotifyLoadingDone("LeaveCustomGame");
			if (leaveTask.succeeded)
			{
				if (leaveTask.result == SessionLeaveResponseCode.NotInASession)
				{
					Console.Log("Unexpected result from leaving a custom game session: the user was not in a session.");
				}
				else
				{
					Console.Log("Succesfully Left Custom Game.");
					EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[140], 0, (object)null, null);
				}
				IRetrieveCustomGameSessionRequest refreshSessionService = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
				refreshSessionService.ClearCache();
				TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionService);
				yield return refreshTask;
				if (!refreshTask.succeeded)
				{
					ErrorWindow.ShowServiceErrorWindow(refreshTask.behaviour);
				}
				customGameScreenController.LeaveCustomGameScreen();
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(leaveTask.behaviour);
				Console.Log("error attempting to leave a custom game.");
				customGameScreenController.LeaveCustomGameScreen();
			}
			CustomGameStateDependency dep = new CustomGameStateDependency(null);
			stateObservable.Dispatch(ref dep);
		}
	}
}
