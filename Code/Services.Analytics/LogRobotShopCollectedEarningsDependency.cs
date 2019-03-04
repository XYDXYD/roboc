namespace Services.Analytics
{
	internal class LogRobotShopCollectedEarningsDependency
	{
		public int totalRobits
		{
			get;
			private set;
		}

		public int earnings
		{
			get;
			private set;
		}

		public LogRobotShopCollectedEarningsDependency(int totalRobits_, int earnings_)
		{
			totalRobits = totalRobits_;
			earnings = earnings_;
		}
	}
}
