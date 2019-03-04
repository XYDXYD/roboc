namespace Services.Web.Photon
{
	public enum DispatchCustomGameInviteResponse
	{
		UserIsNotInSession,
		UserIsNotSessionLeader,
		InviteeHasAlreadyBeenInvited,
		UserIsNotOnline,
		UserInvited,
		ErrorDispatchingMessage,
		InviteeIsInAnotherCustomGame,
		InviteeIsAlreadyInvitedToAnotherCustomGame,
		UserDoesNotExist,
		UserOnlyAcceptsInvitesFromFriendsAndClanmates,
		UserBlockedYou
	}
}
