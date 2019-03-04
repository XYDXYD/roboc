namespace Services.Web.Photon.Tencent
{
	public class RailIDLoginDependency
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

		public RailIDLoginDependency(string railID_, string sessionID_)
		{
			railID = railID_;
			sessionID = sessionID_;
		}
	}
}
