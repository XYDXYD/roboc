using System;

internal struct ChannelNameAndType
{
	public readonly string ChannelName;

	public readonly ChatChannelType ChatChannelType;

	public ChannelNameAndType(string channelName, ChatChannelType chatChannelType)
	{
		this = default(ChannelNameAndType);
		ChannelName = channelName.ToLower();
		ChatChannelType = chatChannelType;
	}

	public override int GetHashCode()
	{
		return StringComparer.OrdinalIgnoreCase.GetHashCode(ChannelName) ^ ChatChannelType.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ChannelNameAndType))
		{
			return false;
		}
		ChannelNameAndType channelNameAndType = (ChannelNameAndType)obj;
		return ChatChannelType == channelNameAndType.ChatChannelType && ChannelName.Equals(channelNameAndType.ChannelName, StringComparison.OrdinalIgnoreCase);
	}
}
