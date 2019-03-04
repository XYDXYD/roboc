internal class SwitchTutorialTestWorldDependency
{
	public string planetToLoad
	{
		get;
		private set;
	}

	public bool isRanked
	{
		get;
		private set;
	}

	public GameModeType gameModeType
	{
		get;
		private set;
	}

	public bool fastSwitch
	{
		get;
		private set;
	}

	public bool isNewPlayer
	{
		get;
		private set;
	}

	public SwitchTutorialTestWorldDependency(string planetToLoad_, bool isRanked_, GameModeType gameModeType_, bool isNewPlayer_)
	{
		planetToLoad = planetToLoad_;
		isRanked = isRanked_;
		gameModeType = gameModeType_;
		isNewPlayer = isNewPlayer_;
	}
}
