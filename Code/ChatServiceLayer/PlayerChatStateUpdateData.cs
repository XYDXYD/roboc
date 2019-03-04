namespace ChatServiceLayer
{
	internal struct PlayerChatStateUpdateData
	{
		public readonly string ChannelName;

		public readonly string PlayerName;

		public readonly ChatPlayerState ChatPlayerState;

		public PlayerChatStateUpdateData(string channelName, string playerName, ChatPlayerState chatPlayerState)
		{
			this = default(PlayerChatStateUpdateData);
			ChannelName = channelName;
			PlayerName = playerName;
			ChatPlayerState = chatPlayerState;
		}
	}
}
