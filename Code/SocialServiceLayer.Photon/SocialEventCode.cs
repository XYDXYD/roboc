namespace SocialServiceLayer.Photon
{
	public enum SocialEventCode : byte
	{
		FriendInviteReceived = 0,
		FriendInviteAccepted = 1,
		FriendInviteDeclined = 2,
		FriendRemoved = 3,
		FriendStatusUpdate = 4,
		FriendInviteCancelled = 5,
		FriendCount = 6,
		AllFriendsOffline = 7,
		DuplicateLogin = 8,
		AvatarUpdated = 9,
		PlatoonInviteReceived = 10,
		PlatoonMemberLeft = 11,
		KickedFromPlatoon = 12,
		PlatoonDisbanded = 13,
		PlatoonMemberAvatarChanged = 14,
		PlatoonRobotTierChanged = 0xF,
		PlatoonLeaderChanged = 0x10,
		PlatoonMemberAdded = 17,
		PlatoonMemberStatusChanged = 18,
		PlatoonInviteCancelled = 19,
		StartFriendChat = 20,
		ClanInviteReceived = 21,
		ClanMemberJoined = 22,
		ClanMemberLeft = 23,
		RemovedFromClan = 24,
		ClanInviteCancelled = 25,
		ClanMemberDataChanged = 26,
		ClanDataChanged = 27,
		ClanRenamed = 28,
		ClanMemberXPChanged = 29,
		FriendClanChanged = 30,
		MasterSlaveDeniedCCUMax = 40,
		CCUCheckPassCode = 41,
		NotUsed = 200
	}
}
