using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IBuyItemShopBundleRequest : IServiceRequest<ItemShopBundle>, IAnswerOnComplete<string[]>, IServiceRequest
	{
	}
}
