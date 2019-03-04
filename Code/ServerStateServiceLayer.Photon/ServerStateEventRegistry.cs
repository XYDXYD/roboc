using Services;
using Services.Web.Photon;

namespace ServerStateServiceLayer.Photon
{
	internal class ServerStateEventRegistry : PhotonEventRegistry<WebServicesEventCode>
	{
		public ServerStateEventRegistry(IServerStateEventListenerFactory serverStateEventListenerFactory)
			: base((IEventListenerFactory)serverStateEventListenerFactory)
		{
			PhotonWebServicesUtility.Instance.GetClientMustBeUsedOnlyForTheServiceContainer().SetEventRegistry(this);
		}
	}
}
