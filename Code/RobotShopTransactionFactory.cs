using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;

public class RobotShopTransactionFactory
{
	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory requestFactory
	{
		private get;
		set;
	}

	public void Build()
	{
		OnLoadingSuccess("RobotShopTransaction_Infinity");
	}

	private void OnLoadingSuccess(string robotShopTransactionPrefabName)
	{
		gameObjectFactory.Build(robotShopTransactionPrefabName);
	}

	private void OnLoadingFailed(ServiceBehaviour behaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(behaviour);
	}
}
