using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameRefreshEventListener : ServerStateEventListener<string>, ICustomGameRefreshEventListener, IServerStateEventListener<string>, IServiceEventListener<string>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.CustomGameRefreshNeeded;

		protected override void ParseData(EventData eventData)
		{
			string data = (string)eventData.Parameters[172];
			Invoke(data);
		}
	}
}
