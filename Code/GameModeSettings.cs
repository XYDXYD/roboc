internal class GameModeSettings
{
	public float respawnHealDuration;

	public float respawnFullHealDuration;

	public int gameTimeMinutes;

	public int? killLimit;

	public int? teamBaseCaptureTime;

	public int? teamBaseUnCaptureTime;

	public GameModeSettings(float respawnHealDuration_, float respawnFullHealDuration_, int gameTimeMinutes_)
	{
		respawnHealDuration = respawnHealDuration_;
		respawnFullHealDuration = respawnFullHealDuration_;
		gameTimeMinutes = gameTimeMinutes_;
	}

	public GameModeSettings(GameModeSettings original)
	{
		respawnHealDuration = original.respawnHealDuration;
		respawnFullHealDuration = original.respawnFullHealDuration;
		killLimit = original.killLimit;
		gameTimeMinutes = original.gameTimeMinutes;
		teamBaseCaptureTime = original.teamBaseCaptureTime;
		teamBaseUnCaptureTime = original.teamBaseUnCaptureTime;
	}
}
