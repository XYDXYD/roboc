using System.Text;

internal class ChannelEventMessage : ChatMessage, IChatMessageWithChannel
{
	public ChatChannelType ChatChannelType
	{
		get;
		private set;
	}

	public override bool ShouldFilterProfanity => false;

	public ChannelEventMessage(string rawText, string channelName, ChatChannelType channelType)
		: base(rawText, channelName)
	{
		ChatChannelType = channelType;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ChatColours.GetColourString(ChatChannelType));
		stringBuilder.Append("[c][[/c]");
		stringBuilder.Append(ChatUtil.GetVisibleChannelName(base.ChannelName, ChatChannelType));
		stringBuilder.Append("] ");
		stringBuilder.Append(base.RawText);
		stringBuilder.Append("[-]");
		return stringBuilder.ToString();
	}
}
