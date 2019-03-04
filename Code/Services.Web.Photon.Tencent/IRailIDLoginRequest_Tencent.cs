using Svelto.ServiceLayer;

namespace Services.Web.Photon.Tencent
{
	internal interface IRailIDLoginRequest_Tencent : IServiceRequest<RailIDLoginDependency>, IAnswerOnComplete<RailIDLoginResponse>, IServiceRequest
	{
	}
}
