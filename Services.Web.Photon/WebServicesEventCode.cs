namespace Services.Web.Photon
{
	public enum WebServicesEventCode : byte
	{
		DuplicateLogin,
		DevMessage,
		NewSessionId,
		MaintenanceMode,
		Banned,
		BrawlChanged,
		CampaignsChanged,
		CustomGameInvitation,
		CustomGameRefreshNeeded,
		CustomGameConfigChanged,
		CustomGameLeaderChanged,
		CustomGameKickedFromSession,
		CustomGameDeclinedInvitation,
		MasterSlaveDeniedCCUMax,
		CCUCheckPassCode,
		CustomGameRobotTierChanged
	}
}
