public struct PlayerSetting
{
	public readonly int TotalLives;

	public readonly bool AutoHeal;

	public readonly int SingleWaveCompletionBonus;

	public PlayerSetting(int totalLives, bool autoHeal, int singleWaveCompletionBonus)
	{
		TotalLives = totalLives;
		AutoHeal = autoHeal;
		SingleWaveCompletionBonus = singleWaveCompletionBonus;
	}
}
