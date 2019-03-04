using Svelto.ServiceLayer;

internal sealed class TutorialCubeGunPickupPresenter
{
	private TutorialCubeGunPickupView _cubeGunPickupView;

	private LoadingIconPresenter _loadingPresenter;

	private IServiceRequestFactory _serviceFactory;

	public TutorialCubeGunPickupPresenter(LoadingIconPresenter loadingPresenter, IServiceRequestFactory serviceFactory)
	{
		_loadingPresenter = loadingPresenter;
		_serviceFactory = serviceFactory;
	}

	public void RegisterView(TutorialCubeGunPickupView view)
	{
		_cubeGunPickupView = view;
	}
}
