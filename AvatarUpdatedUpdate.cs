internal struct AvatarUpdatedUpdate
{
	public readonly AvatarInfo avatarInfo;

	public readonly string user;

	public AvatarUpdatedUpdate(string user, AvatarInfo avatarInfo)
	{
		this.user = user;
		this.avatarInfo = avatarInfo;
	}
}
