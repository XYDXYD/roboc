namespace Services.Analytics
{
	internal class LogItemBoughtDependency
	{
		public string item
		{
			get;
			private set;
		}

		public string itemType
		{
			get;
			private set;
		}

		public string currency
		{
			get;
			private set;
		}

		public int cost
		{
			get;
			private set;
		}

		public string context
		{
			get;
			private set;
		}

		public int discount
		{
			get;
			private set;
		}

		public LogItemBoughtDependency(string item_, string itemType_, string currency_, int cost_, int discount_, string context_)
		{
			item = item_;
			itemType = itemType_;
			currency = currency_;
			cost = cost_;
			context = context_;
			discount = discount_;
		}
	}
}
