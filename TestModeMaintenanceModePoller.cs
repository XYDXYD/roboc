using ServerStateServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using UnityEngine;
using Utility;

internal class TestModeMaintenanceModePoller : ITickable, ITickableBase
{
	private float _timeSpan;

	private const float CHECK_MAINTENANCE_INTERVAL_SECS = 300f;

	[Inject]
	public IServerStateRequestFactory serverStateRequestFactory
	{
		private get;
		set;
	}

	void ITickable.Tick(float deltaSec)
	{
		_timeSpan += deltaSec;
		if (_timeSpan >= 300f)
		{
			_timeSpan = 0f;
			RequestMaintenance();
		}
	}

	private void RequestMaintenance()
	{
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
			Console.Log("*** MaintenanceMode! ***");
			GenericErrorData error = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strMaintenanceMode"), maintenanceModeData.serverMessage, StringTableBase<StringTable>.Instance.GetString("strQuit"), delegate
			{
				Console.Log("Quitting game for maintenance mode");
				Application.Quit();
			});
			ErrorWindow.ShowErrorWindow(error);
		}
	}
}
