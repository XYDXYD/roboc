namespace Login
{
	public interface ISplashLoginDialogView
	{
		void InjectController(ISplashLoginDialogController controller);

		void DestroySelf();
	}
}
