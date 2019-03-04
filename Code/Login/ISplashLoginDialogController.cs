namespace Login
{
	public interface ISplashLoginDialogController
	{
		void SetView(ISplashLoginDialogView view);

		void Close();

		object GetEntry(SplashLoginEntryField fieldToGet);
	}
}
