namespace Services.Web.Photon.Tencent
{
	public class UserNameLoginResult
	{
		public string token
		{
			get;
			private set;
		}

		public string displayName
		{
			get;
			private set;
		}

		public UserNameLoginResult(string token_, string displayname_)
		{
			token = token_;
			displayName = displayname_;
		}
	}
}
