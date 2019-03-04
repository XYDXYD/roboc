internal struct PlayerLevelAndXPData
{
	private float _playerProgress;

	private int _playerLevel;

	private int _playerGainedXP;

	internal float PlayerProgress => _playerProgress;

	internal int PlayerLevel => _playerLevel;

	internal int PlayerGainedXP => _playerGainedXP;

	internal PlayerLevelAndXPData(float playerProgress, int playerLevel, int playerGainedXP)
	{
		_playerProgress = playerProgress;
		_playerLevel = playerLevel;
		_playerGainedXP = playerGainedXP;
	}
}
