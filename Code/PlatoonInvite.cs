internal class PlatoonInvite
{
	public readonly string InviterName;

	public readonly string DisplayName;

	public readonly AvatarInfo AvatarInfo;

	public PlatoonInvite(string inviterName, string displayName, AvatarInfo avatarInfo)
	{
		InviterName = inviterName;
		DisplayName = displayName;
		AvatarInfo = avatarInfo;
	}

	public PlatoonInvite Clone()
	{
		return new PlatoonInvite(InviterName, DisplayName, new AvatarInfo(AvatarInfo.UseCustomAvatar, AvatarInfo.AvatarId));
	}
}
