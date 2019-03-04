namespace ChatServiceLayer
{
	internal struct PlayerJoinedChatRoomData
	{
		public readonly string ChannelName;

		public readonly string PlayerName;

		public readonly ChatPlayerState ChatPlayerState;

		public readonly AvatarInfo AvatarInfo;

		public readonly CustomAvatarInfo CustomAvatarInfo;

		public PlayerJoinedChatRoomData(string channelName, string playerName, ChatPlayerState chatPlayerState, AvatarInfo avatarInfo, CustomAvatarInfo customAvatarInfo)
		{
			this = default(PlayerJoinedChatRoomData);
			ChannelName = channelName;
			PlayerName = playerName;
			ChatPlayerState = chatPlayerState;
			AvatarInfo = avatarInfo;
			CustomAvatarInfo = customAvatarInfo;
		}
	}
}
