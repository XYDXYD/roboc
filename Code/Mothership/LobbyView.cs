using Svelto.IoC;

namespace Mothership
{
	internal class LobbyView
	{
		[Inject]
		internal IGUIInputControllerMothership guiInputController
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

		public void ShowLoadingScreen()
		{
			loadingIconPresenter.NotifyLoading("LobbyLoadingScreen");
		}

		public void HideLoadingScreen()
		{
			loadingIconPresenter.NotifyLoadingDone("LobbyLoadingScreen");
		}
	}
}
