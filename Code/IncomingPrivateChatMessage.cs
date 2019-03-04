using System.Text;

internal class IncomingPrivateChatMessage : ChatMessage, IIgnorableChatMessage, IChatMessageWithChannel
{
	private MessageSender _sender;

	public string SenderName => _sender.UserName;

	public string SenderDisplayName => _sender.DisplayName;

	public ChatChannelType ChatChannelType => ChatChannelType.Private;

	public override bool ShouldFilterProfanity => true;

	public IncomingPrivateChatMessage(string sender, string displayName, string rawText, bool isDev, bool isMod, bool isAdmin)
		: base(rawText, sender)
	{
		_sender = new MessageSender(sender, displayName, isDev, isMod, isAdmin);
	}

	public override string ToString()
	{
		ChatColours.ChatColour chatColour = ChatColours.ChatColour.Whisper;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ChatColours.GetColourString(chatColour));
		stringBuilder.Append("[i]");
		stringBuilder.Append(Localization.Get("strChatMessageFrom", true));
		stringBuilder.Append(' ');
		if (_sender.IsDev)
		{
			stringBuilder.Append("[");
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strDevPrefix"));
			stringBuilder.Append("] ");
		}
		else if (_sender.IsAdmin)
		{
			stringBuilder.Append("[");
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strAdminPrefix"));
			stringBuilder.Append("] ");
		}
		else if (_sender.IsMod)
		{
			stringBuilder.Append("[");
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strModPrefix"));
			stringBuilder.Append("] ");
		}
		stringBuilder.Append(ChatColours.GetUserNameColorString(chatColour));
		stringBuilder.Append(_sender.DisplayName);
		stringBuilder.Append(":[-] ");
		stringBuilder.Append(base.RawText);
		stringBuilder.Append("[-]");
		stringBuilder.Append("[/i]");
		return stringBuilder.ToString();
	}
}
