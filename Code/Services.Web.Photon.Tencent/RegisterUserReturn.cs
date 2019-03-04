namespace Services.Web.Photon.Tencent
{
	public class RegisterUserReturn
	{
		public string Token;

		public string DisplayName;

		public string UserName;

		public RegisterUserReturn(string token_, string displayName_, string userName_)
		{
			Token = token_;
			DisplayName = displayName_;
			UserName = userName_;
		}
	}
}
