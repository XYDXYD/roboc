using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class BrawlChangedEventListener : ServerStateEventListener<bool>, IBrawlChangedEventListener, IServerStateEventListener<bool>, IServiceEventListener<bool>, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.BrawlChanged;

		protected override void ParseData(EventData eventData)
		{
			bool data = (bool)eventData.Parameters[156];
			Invoke(data);
		}
	}
}
