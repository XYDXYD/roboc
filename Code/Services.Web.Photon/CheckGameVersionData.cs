namespace Services.Web.Photon
{
	internal class CheckGameVersionData
	{
		public int versionNumber
		{
			get;
			private set;
		}

		public string activeTestName
		{
			get;
			private set;
		}

		public int testGroup
		{
			get;
			private set;
		}

		public CheckGameVersionData(int versionNumber, string activeTestName, int testGroup)
		{
			this.versionNumber = versionNumber;
			this.activeTestName = activeTestName;
			this.testGroup = testGroup;
		}
	}
}
