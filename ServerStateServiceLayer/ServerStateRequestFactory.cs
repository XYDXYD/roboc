using ServerStateServiceLayer.Requests.Photon;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer
{
	internal class ServerStateRequestFactory : ServiceRequestFactory, IServerStateRequestFactory, IServiceRequestFactory
	{
		public ServerStateRequestFactory()
		{
			AddRelation<IGetDevMessageRequest, GetDevMessageRequest>();
			AddRelation<IGetMaintenanceModeRequest, GetMaintenanceModeRequest>();
		}
	}
}
