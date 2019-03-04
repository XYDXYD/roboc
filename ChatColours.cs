using System;
using UnityEngine;
using Utility;

public class ChatColours : ScriptableObject
{
	internal enum ChatColour
	{
		SocialEvent,
		HelpText,
		GeneralChat,
		PlatoonChat,
		ClanChat,
		Whisper,
		TeamChat,
		BattleChat,
		NoChannel,
		Count
	}

	public Color32 SystemEvents;

	public Color32 HelpText;

	public Color32 GeneralChat;

	public Color32 PlatoonChat;

	public Color32 ClanChat;

	public Color32 Whispers;

	public Color32 TeamChat;

	public Color32 BattleChat;

	public Color32 NoChannel;

	[Range(0f, 2f)]
	public float UserNameBrightness = 1.4f;

	private static readonly Color32[] ChatColours32 = (Color32[])new Color32[9];

	private static readonly string[] ChatColourStrings = new string[9];

	private static readonly string[] UserNameColourStrings = new string[9];

	public ChatColours()
		: this()
	{
	}

	public void Initialise()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		IndexColour(ChatColour.SocialEvent, SystemEvents);
		IndexColour(ChatColour.HelpText, HelpText);
		IndexColour(ChatColour.GeneralChat, GeneralChat, UserNameBrightness);
		IndexColour(ChatColour.PlatoonChat, PlatoonChat, UserNameBrightness);
		IndexColour(ChatColour.ClanChat, ClanChat, UserNameBrightness);
		IndexColour(ChatColour.Whisper, Whispers, UserNameBrightness);
		IndexColour(ChatColour.TeamChat, TeamChat, UserNameBrightness);
		IndexColour(ChatColour.BattleChat, BattleChat, UserNameBrightness);
		IndexColour(ChatColour.NoChannel, NoChannel, UserNameBrightness);
		Console.Log("ChatColours initialised");
	}

	internal static string GetUserNameColorString(ChatColour i)
	{
		return UserNameColourStrings[(int)i];
	}

	private static void IndexColour(ChatColour i, Color32 color, float userNameBrightness = 1f)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		ChatColours32[(int)i] = color;
		ChatColourStrings[(int)i] = "[" + NGUIText.EncodeColor32(Color32.op_Implicit(color)) + "]";
		Color val = Color32.op_Implicit(color);
		val = ((!(userNameBrightness < 1f)) ? Color.Lerp(val, Color.get_white(), userNameBrightness - 1f) : Color.Lerp(Color.get_black(), val, userNameBrightness));
		UserNameColourStrings[(int)i] = "[" + NGUIText.EncodeColor32(val) + "]";
	}

	internal static string GetColourString(ChatColour chatColour)
	{
		return ChatColourStrings[(int)chatColour];
	}

	internal static string GetColourString(ChatChannelType channelType)
	{
		return GetColourString(MapChannelColour(channelType));
	}

	internal static Color32 GetColour(ChatChannelType channelType)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return ChatColours32[(int)MapChannelColour(channelType)];
	}

	internal static Color32 GetColour(ChatColour chatColour)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return ChatColours32[(int)chatColour];
	}

	internal static ChatColour MapChannelColour(ChatChannelType channelType)
	{
		switch (channelType)
		{
		case ChatChannelType.Public:
		case ChatChannelType.Custom:
			return ChatColour.GeneralChat;
		case ChatChannelType.Battle:
			return ChatColour.BattleChat;
		case ChatChannelType.BattleTeam:
			return ChatColour.TeamChat;
		case ChatChannelType.Clan:
			return ChatColour.ClanChat;
		case ChatChannelType.Private:
			return ChatColour.Whisper;
		case ChatChannelType.Platoon:
			return ChatColour.PlatoonChat;
		case ChatChannelType.CustomGame:
			return ChatColour.PlatoonChat;
		case ChatChannelType.None:
			return ChatColour.NoChannel;
		default:
			throw new Exception("Unhandled channelType " + channelType);
		}
	}
}
