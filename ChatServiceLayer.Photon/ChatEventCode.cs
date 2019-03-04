namespace ChatServiceLayer.Photon
{
	public enum ChatEventCode : byte
	{
		DuplicateLogin = 0,
		ChatMessage = 1,
		PrivateChatMessage = 2,
		Sanction = 3,
		PlayerJoinedRoom = 4,
		PlayerLeftRoom = 5,
		PlayerStateUpdate = 6,
		MasterSlaveDeniedCCUMax = 40,
		CCUCheckPassCode = 41
	}
}
