namespace ChatServiceLayer
{
	internal struct PlayerLeftChatRoomData
	{
		public readonly string ChannelName;

		public readonly string PlayerName;

		public PlayerLeftChatRoomData(string channelName, string playerName)
		{
			this = default(PlayerLeftChatRoomData);
			ChannelName = channelName;
			PlayerName = playerName;
		}
	}
}
