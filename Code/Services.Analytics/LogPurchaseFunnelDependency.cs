namespace Services.Analytics
{
	internal class LogPurchaseFunnelDependency
	{
		public readonly string step;

		public readonly string context;

		public readonly bool isStartEvent;

		public readonly string startEventId;

		public LogPurchaseFunnelDependency(string step_, string context_, bool isStartEvent_, string startEventId_)
		{
			step = step_;
			context = context_;
			isStartEvent = isStartEvent_;
			startEventId = startEventId_;
		}
	}
}
