internal class PartyMemberDataChangedMessage
{
	internal readonly int positionIndex;

	internal readonly string playerName;

	internal readonly string playerDisplayName;

	internal readonly PlatoonMember.MemberStatus status;

	internal readonly bool isLeader;

	internal readonly AvatarInfo avatarInfo;

	internal readonly string partyLeaderName;

	internal PartyMemberDataChangedMessage(int index)
	{
		positionIndex = index;
		playerName = string.Empty;
		playerDisplayName = string.Empty;
		avatarInfo = null;
		status = PlatoonMember.MemberStatus.Ready;
		isLeader = false;
		partyLeaderName = string.Empty;
	}

	internal PartyMemberDataChangedMessage(int index, string name, string displayName, PlatoonMember.MemberStatus status, bool leader, AvatarInfo avatar, string leaderName_)
	{
		positionIndex = index;
		playerName = name;
		playerDisplayName = displayName;
		this.status = status;
		isLeader = leader;
		avatarInfo = avatar;
		partyLeaderName = leaderName_;
	}
}
