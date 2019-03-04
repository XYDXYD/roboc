using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class MaintenanceModeEventListener : ServerStateEventListener<string>, IMaintenanceModeEventListener, IServerStateEventListener<string>, IServiceEventListener<string>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.MaintenanceMode;

		protected override void ParseData(EventData eventData)
		{
			string data = (string)eventData.Parameters[19];
			Invoke(data);
		}
	}
}
