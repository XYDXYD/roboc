namespace ChatServiceLayer
{
	internal class ChatChannelMember
	{
		public readonly string UserName;

		public readonly ChatPlayerState ChatPlayerState;

		public readonly AvatarInfo AvatarInfo;

		public readonly CustomAvatarInfo CustomAvatarInfo;

		public ChatChannelMember(string userName, ChatPlayerState chatPlayerState, AvatarInfo avatarInfo, CustomAvatarInfo customAvatarInfo)
		{
			UserName = userName;
			ChatPlayerState = chatPlayerState;
			AvatarInfo = avatarInfo;
			CustomAvatarInfo = customAvatarInfo;
		}
	}
}
