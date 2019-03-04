namespace Services.Analytics
{
	internal class LogChatSentDependency
	{
		public ChatChannelType channel
		{
			get;
			private set;
		}

		public int totalCharacters
		{
			get;
			private set;
		}

		public LogChatSentDependency(ChatChannelType channel_, int totalCharacters_)
		{
			channel = channel_;
			totalCharacters = totalCharacters_;
		}
	}
}
