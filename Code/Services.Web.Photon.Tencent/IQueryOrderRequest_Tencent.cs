using Svelto.ServiceLayer;

namespace Services.Web.Photon.Tencent
{
	internal interface IQueryOrderRequest_Tencent : IServiceRequest<QueryOrderDependency>, IAnswerOnComplete<bool>, IServiceRequest
	{
	}
}
