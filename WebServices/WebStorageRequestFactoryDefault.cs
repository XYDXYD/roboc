using Services.Web.Photon;

namespace WebServices
{
	internal class WebStorageRequestFactoryDefault : WebStorageRequestFactory
	{
		public WebStorageRequestFactoryDefault()
		{
			AddRelation<ILoadGarageDataRequest, GetGarageSlotsRequest>();
		}
	}
}
