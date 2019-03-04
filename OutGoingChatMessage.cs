internal class OutGoingChatMessage : ChatMessage, IChatMessageWithChannel
{
	public readonly string LocalText;

	public readonly string ChatLocation;

	public override bool PlaySound => false;

	public override bool ShouldFilterProfanity => false;

	public ChatChannelType ChatChannelType
	{
		get;
		private set;
	}

	public OutGoingChatMessage(string rawText, string localText, string channelName, ChatChannelType channelType, string chatLocation)
		: base(rawText, channelName)
	{
		LocalText = localText;
		ChatChannelType = channelType;
		ChatLocation = chatLocation;
	}
}
