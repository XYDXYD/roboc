internal interface IChatMessageWithChannel
{
	string ChannelName
	{
		get;
	}

	ChatChannelType ChatChannelType
	{
		get;
	}
}
