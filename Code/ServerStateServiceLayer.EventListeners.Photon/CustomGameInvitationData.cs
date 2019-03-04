namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameInvitationData
	{
		public readonly string SessionID;

		public readonly string InviterName;

		public readonly string DisplayName;

		public readonly AvatarInfo AvatarInfo;

		public readonly bool InvitedToTeamA;

		public CustomGameInvitationData(string sessionID_, string inviterName_, string displayName_, bool useCustomAvatar, int avatarId, bool invitedToTeamA_)
		{
			SessionID = sessionID_;
			InviterName = inviterName_;
			DisplayName = displayName_;
			AvatarInfo = new AvatarInfo(useCustomAvatar, avatarId);
			InvitedToTeamA = invitedToTeamA_;
		}
	}
}
