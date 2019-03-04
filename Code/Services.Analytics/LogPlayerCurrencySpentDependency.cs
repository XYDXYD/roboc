namespace Services.Analytics
{
	internal class LogPlayerCurrencySpentDependency
	{
		public string currency
		{
			get;
			private set;
		}

		public int spent
		{
			get;
			private set;
		}

		public long balance
		{
			get;
			private set;
		}

		public string sink
		{
			get;
			private set;
		}

		public string sinkDetail
		{
			get;
			private set;
		}

		public LogPlayerCurrencySpentDependency(string currency_, int spent_, long balance_, string sink_, string sinkDetail_)
		{
			currency = currency_;
			spent = spent_;
			balance = balance_;
			sink = sink_;
			sinkDetail = sinkDetail_;
		}
	}
}
