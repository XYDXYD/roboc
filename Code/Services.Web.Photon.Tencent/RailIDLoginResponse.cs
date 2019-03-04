namespace Services.Web.Photon.Tencent
{
	public class RailIDLoginResponse
	{
		public string legacyName
		{
			get;
			private set;
		}

		public string displayName
		{
			get;
			private set;
		}

		public string authToken
		{
			get;
			private set;
		}

		public RailIDLoginResponse(string legacyName_, string authToken_, string displayName_)
		{
			displayName = displayName_;
			legacyName = legacyName_;
			authToken = authToken_;
		}
	}
}
