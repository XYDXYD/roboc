internal sealed class GameModeSettingsDependency
{
	public GameModeSettings BattleArena
	{
		get;
		private set;
	}

	public GameModeSettings Classic
	{
		get;
		private set;
	}

	public GameModeSettings Pit
	{
		get;
		private set;
	}

	public GameModeSettings TeamDeathmatch
	{
		get;
		private set;
	}

	public GameModeSettingsDependency(GameModeSettings battleArena, GameModeSettings classic, GameModeSettings pit, GameModeSettings teamDeathMatch)
	{
		BattleArena = battleArena;
		Classic = classic;
		Pit = pit;
		TeamDeathmatch = teamDeathMatch;
	}

	public GameModeSettingsDependency(GameModeSettingsDependency original)
	{
		BattleArena = new GameModeSettings(original.BattleArena);
		Classic = new GameModeSettings(original.Classic);
		Pit = new GameModeSettings(original.Pit);
		TeamDeathmatch = new GameModeSettings(original.TeamDeathmatch);
	}
}
