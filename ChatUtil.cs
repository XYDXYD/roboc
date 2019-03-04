using System;

internal static class ChatUtil
{
	public const string EscapedSquareBracket = "[c][[/c]";

	public static string GetVisibleChannelName(string channelName, ChatChannelType chatChannelType)
	{
		switch (chatChannelType)
		{
		case ChatChannelType.Battle:
			return Localization.Get("strAll", true);
		case ChatChannelType.BattleTeam:
			return Localization.Get("strTeam", true);
		case ChatChannelType.Clan:
			return Localization.Get("strClanNormalCase", true);
		case ChatChannelType.Platoon:
			return Localization.Get("strPlatoon", true);
		case ChatChannelType.Public:
		case ChatChannelType.Custom:
			return channelName;
		case ChatChannelType.CustomGame:
			return Localization.Get("strCustomGameChatChannel", true);
		default:
			throw new Exception("Unhandled channel type " + chatChannelType);
		}
	}
}
