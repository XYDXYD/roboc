public static class GameModeUtils
{
	public static bool SupportsReconnect(GameModeKey gameMode)
	{
		return gameMode.type == GameModeType.Normal || gameMode.type == GameModeType.TeamDeathmatch;
	}

	public static bool HasQuitterPenalty(LobbyType gameMode)
	{
		return gameMode == LobbyType.QuickPlay || gameMode == LobbyType.Brawl;
	}

	public static bool HasRandomGameTypes(LobbyType gameMode)
	{
		return gameMode == LobbyType.QuickPlay;
	}

	public static string GetDisplayableName(GameModeType gameModeType)
	{
		switch (gameModeType)
		{
		case GameModeType.Normal:
			return StringTableBase<StringTable>.Instance.GetString("strLobbyGameModeArena");
		case GameModeType.TeamDeathmatch:
			return StringTableBase<StringTable>.Instance.GetString("strLobbyGameModeDeathMatch");
		case GameModeType.SuddenDeath:
			return StringTableBase<StringTable>.Instance.GetString("strLobbyElimination");
		case GameModeType.Pit:
			return StringTableBase<StringTable>.Instance.GetString("strLobbyGameModePit");
		default:
			return "Error";
		}
	}
}
