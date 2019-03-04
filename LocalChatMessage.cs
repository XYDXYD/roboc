internal class LocalChatMessage : GlobalChatMessage
{
	public override bool ShouldFilterProfanity => true;

	public LocalChatMessage(string userName, string displayName, bool isDev, bool isMod, bool isAdmin, string text, bool isPrivate, string channelName, ChatChannelType channelType)
		: base(userName, displayName, isDev, isMod, isAdmin, text, isPrivate, channelName, channelType)
	{
	}
}
