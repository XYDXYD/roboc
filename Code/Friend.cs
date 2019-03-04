using System;
using System.IO;

public class Friend
{
	public bool IsOnline
	{
		get;
		set;
	}

	public string Name
	{
		get;
		private set;
	}

	public string DisplayName
	{
		get;
		private set;
	}

	public string ClanName
	{
		get;
		set;
	}

	public FriendInviteStatus InviteStatus
	{
		get;
		set;
	}

	internal AvatarInfo AvatarInfo
	{
		get;
		set;
	}

	public Friend()
	{
		Name = string.Empty;
	}

	public Friend(string name, string displayName, FriendInviteStatus status)
	{
		Name = name;
		DisplayName = displayName;
		InviteStatus = status;
	}

	public override string ToString()
	{
		return Name + ": " + InviteStatus;
	}

	public static object Deserialize(byte[] bytes)
	{
		Friend friend = null;
		MemoryStream input = new MemoryStream(bytes);
		using (BinaryReader binaryReader = new BinaryReader(input))
		{
			FriendInviteStatus status = (FriendInviteStatus)binaryReader.ReadByte();
			bool isOnline = binaryReader.ReadBoolean();
			string name = binaryReader.ReadString();
			string displayName = binaryReader.ReadString();
			string text = binaryReader.ReadString();
			if (text.Length == 0)
			{
				text = null;
			}
			friend = new Friend(name, displayName, status);
			friend.IsOnline = isOnline;
			friend.ClanName = text;
			return friend;
		}
	}

	public static byte[] Serialize(object friend)
	{
		throw new NotImplementedException();
	}
}
