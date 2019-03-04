namespace Login
{
	internal class SplashLoginSteamRobocraftChoiceController : ISplashLoginDialogController
	{
		private SplashLoginSteamRobocraftChoiceView _view;

		public void SetView(ISplashLoginDialogView view)
		{
			_view = (view as SplashLoginSteamRobocraftChoiceView);
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			return null;
		}

		public void HandleMessage(object message)
		{
		}

		public void Close()
		{
			_view.DestroySelf();
			_view = null;
		}
	}
}
