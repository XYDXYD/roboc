using Svelto.ServiceLayer;

namespace Services.Web.Photon.Tencent
{
	internal interface IIsPlayerRegisteredRequest_Tencent : IServiceRequest<IsPlayerRegisteredDependency>, IAnswerOnComplete<bool>, IServiceRequest
	{
	}
}
