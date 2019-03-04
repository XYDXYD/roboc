using Services.Web.Photon;

namespace WebServices
{
	internal class WebStorageRequestFactoryTutorial : WebStorageRequestFactory
	{
		public WebStorageRequestFactoryTutorial()
		{
			AddRelation<ILoadGarageDataRequest, GetGarageSlotsRequestTutorial>();
		}
	}
}
