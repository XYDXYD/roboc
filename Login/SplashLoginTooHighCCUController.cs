namespace Login
{
	internal class SplashLoginTooHighCCUController : ISplashLoginDialogController
	{
		private SplashLoginTooHighCCUView _view;

		public void SetView(ISplashLoginDialogView view)
		{
			_view = (view as SplashLoginTooHighCCUView);
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			return null;
		}

		public void SetHeaderAndBody(string header, string body)
		{
			_view.SetTitleText(header);
			_view.SetBodyText(body);
		}

		public void SetQueuePositionText(string queuePosition)
		{
			_view.SetQueuePosition(queuePosition);
		}

		public void Close()
		{
			_view.DestroySelf();
			_view = null;
		}
	}
}
