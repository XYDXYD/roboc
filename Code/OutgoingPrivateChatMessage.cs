using System.Text;

internal class OutgoingPrivateChatMessage : ChatMessage, IChatMessageWithChannel
{
	public override bool PlaySound => false;

	public override bool ShouldFilterProfanity => false;

	public string Receiver
	{
		get;
		private set;
	}

	public string DisplayName
	{
		get;
		private set;
	}

	public string ChatLocation
	{
		get;
		private set;
	}

	public ChatChannelType ChatChannelType => ChatChannelType.Private;

	public OutgoingPrivateChatMessage(string receiver, string displayName, string rawText, string chatLocation)
		: base(rawText, receiver)
	{
		Receiver = receiver;
		DisplayName = displayName;
		ChatLocation = chatLocation;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ChatColours.GetColourString(ChatColours.ChatColour.Whisper));
		stringBuilder.AppendFormat("[i] [{0} ", StringTableBase<StringTable>.Instance.GetString("strChatMessageTo"));
		stringBuilder.Append(DisplayName);
		stringBuilder.Append("] ");
		stringBuilder.Append(base.RawText);
		stringBuilder.Append("[-]");
		stringBuilder.Append("[/i]");
		return stringBuilder.ToString();
	}
}
