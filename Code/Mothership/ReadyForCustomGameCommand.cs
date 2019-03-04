using Services.Web.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class ReadyForCustomGameCommand : ICommand
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
		internal DesiredGameMode desiredGameMode
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

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)TryCheckIfCanJoinCustomGameQueue);
		}

		private IEnumerator TryCheckIfCanJoinCustomGameQueue()
		{
			loadingIconPresenter.NotifyLoading("CheckCanQueue");
			ICheckIfCanJoinCustomGameQueueResponse request = serviceFactory.Create<ICheckIfCanJoinCustomGameQueueResponse>();
			request.ClearCache();
			TaskService<CheckIfCanJoinCustomGameQueueResponse> taskService = new TaskService<CheckIfCanJoinCustomGameQueueResponse>(request);
			yield return new HandleTaskServiceWithError(taskService, delegate
			{
				loadingIconPresenter.NotifyLoading("CheckCanQueue");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CheckCanQueue");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("CheckCanQueue");
			switch (taskService.result)
			{
			case CheckIfCanJoinCustomGameQueueResponse.PlayerCanJoinQueue:
				desiredGameMode.DesiredMode = LobbyType.CustomGame;
				guiInputController.ToggleScreen(GuiScreens.BattleCountdown);
				break;
			case CheckIfCanJoinCustomGameQueueResponse.PlayerNotInASessionOrInvalidSession:
			case CheckIfCanJoinCustomGameQueueResponse.ErrorCheckingIfCanJoinQueue:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameError"), StringTableBase<StringTable>.Instance.GetString("strCustomGameSessionNoLongerValid"), StringTableBase<StringTable>.Instance.GetString("strOK")));
				guiInputController.CloseCurrentScreen();
				break;
			case CheckIfCanJoinCustomGameQueueResponse.CannotJoinQueueTooFewPlayers:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameError"), StringTableBase<StringTable>.Instance.GetString("strCustomGameReadyForGameValidationFailOnlyOne"), StringTableBase<StringTable>.Instance.GetString("strOK")));
				break;
			case CheckIfCanJoinCustomGameQueueResponse.CannotJoinQueueImbalancedTeams:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameError"), StringTableBase<StringTable>.Instance.GetString("strCustomGameReadyForGameValidationFailUnbalanced"), StringTableBase<StringTable>.Instance.GetString("strOK")));
				break;
			case CheckIfCanJoinCustomGameQueueResponse.CannotJoinQueuePlayersAlreadyInSession:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameError"), StringTableBase<StringTable>.Instance.GetString("strCustomGameReadyForGameValidationFailPlayersInASession"), StringTableBase<StringTable>.Instance.GetString("strOK")));
				break;
			}
		}
	}
}
