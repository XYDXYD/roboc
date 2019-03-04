namespace Services.Analytics
{
	internal class LogPromoCodeActivatedDependency
	{
		public string promoId
		{
			get;
			private set;
		}

		public float price
		{
			get;
			private set;
		}

		public string bundleId
		{
			get;
			private set;
		}

		public LogPromoCodeActivatedDependency(string promoId_, float price_, string bundleId_)
		{
			promoId = promoId_;
			price = price_;
			bundleId = bundleId_;
		}
	}
}
