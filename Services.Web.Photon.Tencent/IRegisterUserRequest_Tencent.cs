using Svelto.ServiceLayer;

namespace Services.Web.Photon.Tencent
{
	internal interface IRegisterUserRequest_Tencent : IServiceRequest<RegisterUserDependency>, IAnswerOnComplete<RegisterUserReturn>, IServiceRequest
	{
	}
}
