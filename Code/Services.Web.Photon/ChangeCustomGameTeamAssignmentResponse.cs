namespace Services.Web.Photon
{
	internal enum ChangeCustomGameTeamAssignmentResponse
	{
		SessionNoLongerExists,
		UserIsNotSessionLeader,
		SourceOrTargetPlayerNotFoundInCorrectTeam,
		DestinationTeamAlreadyFull,
		TeamAssignmentChangeSuccess,
		TeamAssignmentChangeError
	}
}
