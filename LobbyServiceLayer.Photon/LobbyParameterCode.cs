namespace LobbyServiceLayer.Photon
{
	internal enum LobbyParameterCode
	{
		Guid = 0,
		GameMode = 1,
		PlatoonId = 2,
		GarageSlot = 3,
		PlatoonSize = 4,
		Players = 5,
		HostAddress = 6,
		HostPort = 7,
		MapName = 8,
		BaseModel = 9,
		FusionTowerModel = 10,
		BattleGuid = 11,
		WaitTime = 12,
		EstimatedQueueTime = 13,
		IsPlatoonLeader = 14,
		BlockTimeSecs = 0xF,
		QueueResult = 0x10,
		Unused = 17,
		Tier = 18,
		QuitLastGame = 19,
		WasSuccess = 20,
		ErrorCode = 21,
		ErrorDetails = 22,
		NetworkConfigs = 23,
		BrawlVersionNum = 24,
		IsRanked = 25,
		IsBrawl = 26,
		IsCustomGame = 27,
		GameModePreferences = 29,
		LobbyType = 30,
		ReconnectGameGuid = 40
	}
}
