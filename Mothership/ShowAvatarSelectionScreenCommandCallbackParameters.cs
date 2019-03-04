namespace Mothership
{
	internal class ShowAvatarSelectionScreenCommandCallbackParameters
	{
		public readonly AvatarInfo AvatarInfo;

		public readonly CustomAvatarInfo CustomAvatarInfo;

		public ShowAvatarSelectionScreenCommandCallbackParameters(AvatarInfo avatarInfo_, CustomAvatarInfo customAvatarInfo_)
		{
			AvatarInfo = avatarInfo_;
			CustomAvatarInfo = customAvatarInfo_;
		}
	}
}
