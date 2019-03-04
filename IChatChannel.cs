using Svelto.WeakEvents;

internal interface IChatChannel
{
	string ChannelName
	{
		get;
	}

	string VisibleName
	{
		get;
	}

	ChatChannelType ChatChannelType
	{
		get;
	}

	bool Suspended
	{
		get;
		set;
	}

	void SendMessage(OutGoingChatMessage message, WeakAction<OutGoingChatMessage> onSentSuccesfully);
}
