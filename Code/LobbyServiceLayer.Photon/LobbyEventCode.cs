namespace LobbyServiceLayer.Photon
{
	internal enum LobbyEventCode : byte
	{
		DuplicateLogin = 0,
		ErrorJoiningQueue = 1,
		StartConnectionTest = 2,
		BattleFound = 3,
		BattleCancelled = 4,
		EnterBattle = 5,
		ErrorJoiningBattle = 6,
		ErrorInQueue = 7,
		BrawlVersionOutdated = 8,
		MasterSlaveDeniedCCUMax = 40,
		CCUCheckPassCode = 41
	}
}
