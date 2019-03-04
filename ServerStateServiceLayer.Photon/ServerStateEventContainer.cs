using Services.Web.Photon;

namespace ServerStateServiceLayer.Photon
{
	internal class ServerStateEventContainer : PhotonEventContainer<WebServicesEventCode>
	{
		public ServerStateEventContainer(ServerStateEventRegistry eventRegistry)
			: base((PhotonEventRegistry<WebServicesEventCode>)eventRegistry, PhotonWebServicesUtility.Instance.GetClientMustBeUsedOnlyForTheServiceContainer())
		{
		}
	}
}
