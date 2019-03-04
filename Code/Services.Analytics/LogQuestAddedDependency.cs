namespace Services.Analytics
{
	internal class LogQuestAddedDependency
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

		public LogQuestAddedDependency(string questID_, int activeQuests_)
		{
			questID = questID_;
			activeQuests = activeQuests_;
		}
	}
}
