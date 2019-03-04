using Svelto.ServiceLayer;

namespace Services.Web.Photon.Tencent
{
	internal interface ICreateOrderRequest_Tencent : IServiceRequest<CreateOrderDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
