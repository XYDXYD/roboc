using ServerStateServiceLayer;
using Svelto.Context;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using UnityEngine;
using Utility;

internal sealed class MaintenanceModeController : IWaitForFrameworkDestruction, IWaitForFrameworkInitialization
{
	private IServiceEventContainer _serverStateEventContainer;

	[Inject]
	public IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	[Inject]
	public IServerStateEventContainerFactory serverStateEventContainerFactory
	{
		private get;
		set;
	}

	[Inject]
	public IServerStateRequestFactory serverStateRequestFactory
	{
		private get;
		set;
	}

	public static bool isInMaintenance
	{
		get;
		private set;
	}

	void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		_serverStateEventContainer.Dispose();
		_serverStateEventContainer = null;
	}

	void IWaitForFrameworkInitialization.OnFrameworkInitialized()
	{
		_serverStateEventContainer = serverStateEventContainerFactory.Create();
		_serverStateEventContainer.ListenTo<IMaintenanceModeEventListener, string>(GoToMaintenance);
		serverStateRequestFactory.Create<IGetMaintenanceModeRequest>().SetAnswer(new ServiceAnswer<MaintenanceModeData>(HandleMaintenanceModeRequestComplete, OnRequestFailed)).Execute();
	}

	private void OnRequestFailed(ServiceBehaviour obj)
	{
		ErrorWindow.ShowServiceErrorWindow(obj);
	}

	private void HandleMaintenanceModeRequestComplete(MaintenanceModeData maintenanceModeData)
	{
		if (maintenanceModeData.isInMaintenance)
		{
			GoToMaintenance(maintenanceModeData.serverMessage);
		}
	}

	private void GoToMaintenance(string maintenanceMessage)
	{
		isInMaintenance = true;
		ShowMaintenanceDialogue(maintenanceMessage);
	}

	private void ShowMaintenanceDialogue(string bodyText)
	{
		Console.Log("*** MaintenanceMode! ***");
		GenericErrorData errorData = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strMaintenanceMode"), bodyText, StringTableBase<StringTable>.Instance.GetString("strQuit"), delegate
		{
			Console.Log("Quitting game for maintenance mode");
			Application.Quit();
		});
		GenericErrorDialogue component = gameObjectFactory.Build("ErrorDialog").GetComponent<GenericErrorDialogue>();
		component.Open(errorData);
	}
}
