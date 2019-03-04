namespace LobbyServiceLayer
{
	internal enum LobbyReasonCode
	{
		strUnexpectedError = -1,
		Ok,
		strMaintenanceMode,
		strRobotValidationError,
		strLoggedInOtherLocation,
		strGroupFailedChecks,
		strConnectionTestFailed,
		strWrongGameModeForParty,
		strBrawlConnectionTestFailed,
		strPartyNotAllowed,
		strNoSuitableLobbyFound
	}
}
