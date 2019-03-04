namespace Services.Web.Photon.Tencent
{
	public class RegisterUserDependency
	{
		public string railID
		{
			get;
			private set;
		}

		public string sessionID
		{
			get;
			private set;
		}

		public string displayName
		{
			get;
			private set;
		}

		public RegisterUserDependency(string railID_, string sessionID_, string displayName_)
		{
			railID = railID_;
			sessionID = sessionID_;
			displayName = displayName_;
		}
	}
}
