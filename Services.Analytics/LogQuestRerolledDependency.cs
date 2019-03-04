namespace Services.Analytics
{
	internal class LogQuestRerolledDependency
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

		public LogQuestRerolledDependency(string questID_, int activeQuests_)
		{
			questID = questID_;
			activeQuests = activeQuests_;
		}
	}
}
