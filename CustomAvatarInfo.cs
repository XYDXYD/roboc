internal class CustomAvatarInfo
{
	public byte[] CustomAvatarBytes
	{
		get;
		set;
	}

	public CustomAvatarFormat AvatarFormat
	{
		get;
		set;
	}

	public CustomAvatarInfo(byte[] customAvatarBytes, CustomAvatarFormat avatarFormat = CustomAvatarFormat.Jpg)
	{
		CustomAvatarBytes = customAvatarBytes;
		AvatarFormat = avatarFormat;
	}
}
