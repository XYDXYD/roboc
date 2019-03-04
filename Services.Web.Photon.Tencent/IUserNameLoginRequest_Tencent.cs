using Svelto.ServiceLayer;

namespace Services.Web.Photon.Tencent
{
	internal interface IUserNameLoginRequest_Tencent : IServiceRequest<UserNameLoginDependency>, IAnswerOnComplete<UserNameLoginResult>, IServiceRequest
	{
	}
}
