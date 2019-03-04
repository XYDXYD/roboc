namespace ChatServiceLayer.Photon
{
	public enum ChatOperationCode : byte
	{
		JoinChatChannel = 1,
		ChatMessage,
		PrivateChatMessage,
		CreateChatChannel,
		LeaveChannel,
		AcknowledgeSanction,
		AddOrUpdateSanction,
		GetChatIgnores,
		IgnoreUser,
		UnignoreUser,
		SubscribeAllJoinedChannels,
		GetSubscribedChannels,
		GetPublicChannelNames,
		GetUserExists,
		CheckPendingSanctions
	}
}
