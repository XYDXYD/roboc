using System.Text;

internal abstract class GlobalChatMessage : ChatMessage, IChatMessageWithChannel
{
	public readonly MessageSender Sender;

	public readonly bool IsPrivate;

	public ChatChannelType ChatChannelType
	{
		get;
		private set;
	}

	public override string VisibleChannelName => ChatUtil.GetVisibleChannelName(base.ChannelName, ChatChannelType);

	protected GlobalChatMessage(string userName, string displayName, bool isDev, bool isMod, bool isAdmin, string text, bool isPrivate, string channelName, ChatChannelType channelType)
		: this(new MessageSender(userName, displayName, isDev, isMod, isAdmin), text, isPrivate, channelName, channelType)
	{
	}

	protected GlobalChatMessage(MessageSender sender, string rawText, bool isPrivate, string channelName, ChatChannelType channelType)
		: base(rawText, channelName)
	{
		Sender = sender;
		IsPrivate = isPrivate;
		ChatChannelType = channelType;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ChatColours.GetColourString(ChatChannelType));
		stringBuilder.Append("[c][[/c]");
		stringBuilder.Append(ChatUtil.GetVisibleChannelName(base.ChannelName, ChatChannelType));
		stringBuilder.Append("] ");
		MessageSender sender = Sender;
		if (sender.IsDev)
		{
			stringBuilder.Append("[");
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strDevPrefix"));
			stringBuilder.Append("] ");
		}
		else
		{
			MessageSender sender2 = Sender;
			if (sender2.IsAdmin)
			{
				stringBuilder.Append("[");
				stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strAdminPrefix"));
				stringBuilder.Append("] ");
			}
			else
			{
				MessageSender sender3 = Sender;
				if (sender3.IsMod)
				{
					stringBuilder.Append("[");
					stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strModPrefix"));
					stringBuilder.Append("] ");
				}
			}
		}
		stringBuilder.Append(ChatColours.GetUserNameColorString(ChatColours.MapChannelColour(ChatChannelType)));
		StringBuilder stringBuilder2 = stringBuilder;
		MessageSender sender4 = Sender;
		stringBuilder2.Append(sender4.DisplayName);
		stringBuilder.Append(":[-] ");
		stringBuilder.Append(base.RawText);
		stringBuilder.Append("[-]");
		return stringBuilder.ToString();
	}
}
