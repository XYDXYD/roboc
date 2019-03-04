using System;

internal class SwitchWorldDependency
{
	public string planetToLoad
	{
		get;
		private set;
	}

	public bool IsRanked
	{
		get;
		private set;
	}

	public bool IsBrawl
	{
		get;
		private set;
	}

	public bool IsCustomGame
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

	public Action ContinueWith
	{
		get;
		private set;
	}

	public SwitchWorldDependency(string _planetToLoad, bool _fastSwitch, Action continueWith = null)
	{
		planetToLoad = _planetToLoad;
		fastSwitch = false;
		IsRanked = false;
		IsBrawl = false;
		IsCustomGame = false;
		ContinueWith = continueWith;
	}

	public SwitchWorldDependency(string planetToLoad_, bool isRanked_, bool isBrawl_, bool isCustomGame_, GameModeType _gameModeType)
	{
		planetToLoad = planetToLoad_;
		IsRanked = isRanked_;
		IsBrawl = isBrawl_;
		IsCustomGame = isCustomGame_;
		gameModeType = _gameModeType;
	}
}
