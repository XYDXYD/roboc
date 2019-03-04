namespace Services.Analytics
{
	internal class LogClaimedPromotionDependency
	{
		public string bundleName
		{
			get;
			private set;
		}

		public float price
		{
			get;
			private set;
		}

		public LogClaimedPromotionDependency(string bundleName_, float price_)
		{
			bundleName = bundleName_;
			price = price_;
		}
	}
}
