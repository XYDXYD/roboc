namespace Services.Web.Photon
{
	internal enum CheckIfCanJoinCustomGameQueueResponse
	{
		PlayerNotInASessionOrInvalidSession,
		CannotJoinQueueTooFewPlayers,
		CannotJoinQueueImbalancedTeams,
		CannotJoinQueuePlayersAlreadyInSession,
		PlayerCanJoinQueue,
		ErrorCheckingIfCanJoinQueue
	}
}
