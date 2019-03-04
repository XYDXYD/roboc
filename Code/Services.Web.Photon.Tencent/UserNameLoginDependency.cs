namespace Services.Web.Photon.Tencent
{
	public class UserNameLoginDependency
	{
		public string username
		{
			get;
			private set;
		}

		public string password
		{
			get;
			private set;
		}

		public UserNameLoginDependency(string username_, string password_)
		{
			username = username_;
			password = password_;
		}
	}
}
