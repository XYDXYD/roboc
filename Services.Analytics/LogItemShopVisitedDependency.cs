namespace Services.Analytics
{
	internal class LogItemShopVisitedDependency
	{
		public enum Context
		{
			Featured,
			Daily,
			Both,
			None
		}

		public string context
		{
			get;
			private set;
		}

		public LogItemShopVisitedDependency(string context_)
		{
			context = context_;
		}
	}
}
