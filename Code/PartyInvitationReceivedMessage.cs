internal class PartyInvitationReceivedMessage
{
	internal readonly string originatorName;

	internal readonly string displayName;

	internal readonly AvatarInfo avatarInfo;

	internal PartyInvitationReceivedMessage(string originatorName, string displayName, AvatarInfo avatarInfo)
	{
		this.originatorName = originatorName;
		this.displayName = displayName;
		this.avatarInfo = avatarInfo;
	}
}
