using UnityEngine;

internal sealed class CapFrameRateSettings
{
	private int _cappedFrameRateAmount;

	private bool _capEnabled = true;

	private const string PLAYER_PREF_CAPPED_FRAME_RATE_KEY = "CappedFrameRate";

	private const string PLAYER_PREF_CAPPED_FRAME_RATE_ENABLED_KEY = "CappedFrameRateEnabled";

	public int cappedFrameRateAmount => _cappedFrameRateAmount;

	public bool capEnabled => _capEnabled;

	public void InitialiseFrameRate()
	{
		_cappedFrameRateAmount = PlayerPrefs.GetInt("CappedFrameRate", 60);
		_capEnabled = (PlayerPrefs.GetInt("CappedFrameRateEnabled", 1) == 1);
		if (_capEnabled && _cappedFrameRateAmount == 0)
		{
			_cappedFrameRateAmount = 60;
		}
		SetUpFrameRate();
	}

	public void ChangeSetting(bool enabled, int value)
	{
		_capEnabled = enabled;
		_cappedFrameRateAmount = value;
		SetUpFrameRate();
		PlayerPrefs.SetInt("CappedFrameRate", value);
		PlayerPrefs.SetInt("CappedFrameRateEnabled", enabled ? 1 : 0);
	}

	private void SetUpFrameRate()
	{
		if (_capEnabled)
		{
			Application.set_targetFrameRate(_cappedFrameRateAmount);
		}
		else
		{
			Application.set_targetFrameRate(120);
		}
		QualitySettings.set_vSyncCount(0);
		QualitySettings.set_maxQueuedFrames(3);
	}
}
