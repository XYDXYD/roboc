namespace Services.Analytics
{
	internal class LogItemStockedDependency
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

		public bool locked
		{
			get;
			private set;
		}

		public string restock
		{
			get;
			private set;
		}

		public LogItemStockedDependency(string item_, string itemType_, string currency_, string context_, string restock_, bool locked_, int discount_)
		{
			item = item_;
			itemType = itemType_;
			currency = currency_;
			context = context_;
			restock = restock_;
			locked = locked_;
			discount = discount_;
		}
	}
}
