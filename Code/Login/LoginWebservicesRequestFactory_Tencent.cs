using Services.Web.Photon.Tencent;

namespace Login
{
	internal class LoginWebservicesRequestFactory_Tencent : LoginWebservicesRequestFactory
	{
		public LoginWebservicesRequestFactory_Tencent()
		{
			AddRelation<IUserNameLoginRequest_Tencent, UserNameLoginRequest_Tencent, UserNameLoginDependency>();
			AddRelation<IIsPlayerRegisteredRequest_Tencent, IsPlayerRegisteredRequest_Tencent, IsPlayerRegisteredDependency>();
			AddRelation<IRegisterUserRequest_Tencent, RegisterUserRequest_Tencent, RegisterUserDependency>();
			AddRelation<IRailIDLoginRequest_Tencent, RailIDLoginRequest_Tencent, RailIDLoginDependency>();
		}
	}
}
