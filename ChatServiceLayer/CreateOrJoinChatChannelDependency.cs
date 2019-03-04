namespace ChatServiceLayer
{
	internal struct CreateOrJoinChatChannelDependency
	{
		public readonly string ChannelName;

		public readonly ChatChannelType ChannelType;

		public readonly string Password;

		public CreateOrJoinChatChannelDependency(string channelName, ChatChannelType channelType, string password)
		{
			this = default(CreateOrJoinChatChannelDependency);
			ChannelName = channelName;
			Password = password;
			ChannelType = channelType;
		}
	}
}
