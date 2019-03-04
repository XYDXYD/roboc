internal class AvatarInfo
{
	public bool UseCustomAvatar
	{
		get;
		set;
	}

	public int AvatarId
	{
		get;
		set;
	}

	public AvatarInfo(bool useCustomAvatar, int avatarId)
	{
		UseCustomAvatar = useCustomAvatar;
		AvatarId = avatarId;
	}
}
