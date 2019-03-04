namespace Services.Web.Photon.Tencent
{
	public class IsPlayerRegisteredDependency
	{
		public string railID
		{
			get;
			private set;
		}

		public string railSessionID
		{
			get;
			private set;
		}

		public IsPlayerRegisteredDependency(string railID_, string railSessionID_)
		{
			railID = railID_;
			railSessionID = railSessionID_;
		}
	}
}
