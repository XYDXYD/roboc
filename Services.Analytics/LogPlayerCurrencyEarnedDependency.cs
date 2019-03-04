namespace Services.Analytics
{
	internal class LogPlayerCurrencyEarnedDependency
	{
		public string currency
		{
			get;
			private set;
		}

		public int earned
		{
			get;
			private set;
		}

		public long balance
		{
			get;
			private set;
		}

		public int premiumBonus
		{
			get;
			private set;
		}

		public string source
		{
			get;
			private set;
		}

		public string sourceDetail
		{
			get;
			private set;
		}

		public LogPlayerCurrencyEarnedDependency(string currency_, int earned_, long balance_, int premiumBonus_, string source_, string sourceDetail_)
		{
			currency = currency_;
			earned = earned_;
			balance = balance_;
			premiumBonus = premiumBonus_;
			source = source_;
			sourceDetail = sourceDetail_;
		}
	}
}
