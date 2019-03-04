namespace Services.Web.Photon
{
	internal enum ReplyToCustomGameInviteResponseCode
	{
		AcceptedInvitationSuccesfully,
		DeclinedInvitationSuccesfully,
		ReplyToResponseError,
		UserAlreadyInSession,
		SessionDoesntExist,
		UserCouldNotAcceptInviteAsNotInvited
	}
}
