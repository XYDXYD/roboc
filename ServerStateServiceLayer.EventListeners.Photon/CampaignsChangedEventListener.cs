using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CampaignsChangedEventListener : ServerStateEventListener, ICampaignsChangedEventListener, IServiceEventListener, IServiceEventListenerBase
	{
		public override WebServicesEventCode WebServicesEventCode => WebServicesEventCode.CampaignsChanged;

		protected override void ParseData(EventData eventData)
		{
			Invoke();
		}
	}
}
