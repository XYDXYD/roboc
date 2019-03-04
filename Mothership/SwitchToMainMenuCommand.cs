using Services.Web.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class SwitchToMainMenuCommand : ICommand
	{
		[Inject]
		internal IServiceRequestFactory serviceFactory
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
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SwitchToMainMenuTask);
		}

		public IEnumerator SwitchToMainMenuTask()
		{
			IRetrieveCustomGameSessionRequest retrieveRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> retrieveTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveRequest);
			yield return retrieveTask;
			if (!retrieveTask.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(retrieveTask.behaviour);
				guiInputController.ToggleScreen(GuiScreens.PlayScreen);
			}
			else if (retrieveTask.result.Response == CustomGameSessionRetrieveResponse.SessionRetrieved)
			{
				guiInputController.ToggleScreen(GuiScreens.CustomGameScreen);
			}
			else
			{
				guiInputController.ToggleScreen(GuiScreens.PlayScreen);
			}
		}
	}
}
