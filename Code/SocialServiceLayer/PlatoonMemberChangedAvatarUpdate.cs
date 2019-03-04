namespace SocialServiceLayer
{
	internal class PlatoonMemberChangedAvatarUpdate
	{
		public readonly string UserName;

		public readonly AvatarInfo AvatarInfo;

		public PlatoonMemberChangedAvatarUpdate(string userName, AvatarInfo avatarInfo)
		{
			UserName = userName;
			AvatarInfo = avatarInfo;
		}
	}
}
