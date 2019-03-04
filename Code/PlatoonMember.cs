using System;
using System.IO;

internal class PlatoonMember : IComparable
{
	public enum MemberStatus
	{
		Invited,
		Ready,
		InQueue,
		InBattle
	}

	public readonly string Name;

	public readonly string DisplayName;

	public MemberStatus Status
	{
		get;
		set;
	}

	public AvatarInfo AvatarInfo
	{
		get;
		set;
	}

	public long AddedTimestamp
	{
		get;
		set;
	}

	public PlatoonMember(string name, string displayName, MemberStatus memberStatus, AvatarInfo avatarInfo, long addedTimestamp)
	{
		Name = name;
		DisplayName = displayName;
		Status = memberStatus;
		AvatarInfo = avatarInfo;
		AddedTimestamp = addedTimestamp;
	}

	public static object Deserialize(byte[] bytes)
	{
		PlatoonMember platoonMember = null;
		MemoryStream input = new MemoryStream(bytes);
		using (BinaryReader binaryReader = new BinaryReader(input))
		{
			string name = binaryReader.ReadString();
			string displayName = binaryReader.ReadString();
			MemberStatus memberStatus = (MemberStatus)binaryReader.ReadByte();
			long addedTimestamp = binaryReader.ReadInt64();
			int avatarId = binaryReader.ReadInt32();
			bool useCustomAvatar = binaryReader.ReadBoolean();
			return new PlatoonMember(name, displayName, memberStatus, new AvatarInfo(useCustomAvatar, avatarId), addedTimestamp);
		}
	}

	public static byte[] Serialize(object platoonMemberToSerialise)
	{
		throw new NotImplementedException();
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (obj.GetType() != typeof(PlatoonMember))
		{
			throw new ArgumentException("Object is not a PlatoonMember");
		}
		PlatoonMember platoonMember = (PlatoonMember)obj;
		int num = -Status.CompareTo(platoonMember.Status);
		if (num == 0)
		{
			return Name.CompareTo(platoonMember.Name);
		}
		return num;
	}
}
