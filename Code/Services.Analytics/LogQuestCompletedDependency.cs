namespace Services.Analytics
{
	internal class LogQuestCompletedDependency
	{
		public string questID
		{
			get;
			private set;
		}

		public int activeQuests
		{
			get;
			private set;
		}

		public LogQuestCompletedDependency(string questID_, int activeQuests_)
		{
			questID = questID_;
			activeQuests = activeQuests_;
		}
	}
}
