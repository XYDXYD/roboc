namespace LobbyServiceLayer.Photon
{
	internal enum LobbyOperationCode
	{
		EnterQueue,
		DOES_NOTHING_LeaveQueue,
		PlatoonUpdate,
		GetBlockInfo,
		ConnectionTestResult,
		CreateCustomGameSession,
		LeaveCustomGameSession,
		RetrieveCustomGameSessionInfo,
		GetReconnectableGame,
		UnregisterPlayerFromReconnectableGame,
		MarkGameQuitter,
		GetGameGUIDForSinglePlayerGame
	}
}
