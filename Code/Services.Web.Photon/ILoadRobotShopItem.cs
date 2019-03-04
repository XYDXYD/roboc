using Mothership;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ILoadRobotShopItem : IServiceRequest<int>, IAnswerOnComplete<CRFItem>, IServiceRequest
	{
	}
}
