using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;

public class RobotShopFactory
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
		OnLoadingSuccess("RobotShop_Infinity");
	}

	private void OnLoadingSuccess(string robotShopPrefabName)
	{
		gameObjectFactory.Build(robotShopPrefabName);
	}
}
