internal class InboundChatMessage : GlobalChatMessage, IIgnorableChatMessage
{
	public string SenderName
	{
		get
		{
			MessageSender sender = Sender;
			return sender.UserName;
		}
	}

	public string SenderDisplayName
	{
		get
		{
			MessageSender sender = Sender;
			return sender.DisplayName;
		}
	}

	public override bool ShouldFilterProfanity => true;

	public InboundChatMessage(string userName, string displayName, bool isDev, bool isMod, bool isAdmin, string text, bool isPrivate, string channelName, ChatChannelType channelType)
		: base(userName, displayName, isDev, isMod, isAdmin, text, isPrivate, channelName, channelType)
	{
	}
}
