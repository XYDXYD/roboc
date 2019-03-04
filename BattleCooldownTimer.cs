using System;

internal sealed class BattleCooldownTimer
{
	private bool _isStarted;

	private bool _isCooldownFinished;

	private double _timeLeft = double.MaxValue;

	private int _lastSentSeconds;

	private DateTime _lastTickedTime;

	public event Action onModeChanged = delegate
	{
	};

	public event Action<int> onSecondsChanged = delegate
	{
	};

	public void StartCounting(double timeLeft)
	{
		_timeLeft = timeLeft;
		_lastTickedTime = DateTime.UtcNow;
		_isStarted = true;
		bool flag = IsCooldownFinished();
		if (flag != _isCooldownFinished)
		{
			_isCooldownFinished = flag;
			this.onModeChanged();
		}
	}

	public bool IsCooldownFinished()
	{
		return _timeLeft <= 0.0 && _isStarted;
	}

	public void Tick(float deltaTime)
	{
		if (_isStarted)
		{
			DateTime utcNow = DateTime.UtcNow;
			float num = Convert.ToSingle((utcNow - _lastTickedTime).TotalSeconds);
			_lastTickedTime = utcNow;
			if (_timeLeft > 0.0)
			{
				_timeLeft -= num;
			}
			bool flag = IsCooldownFinished();
			if (flag != _isCooldownFinished)
			{
				_isCooldownFinished = flag;
				this.onModeChanged();
			}
			int secondsLeft = GetSecondsLeft();
			if (secondsLeft != _lastSentSeconds || _lastSentSeconds == 0)
			{
				_lastSentSeconds = secondsLeft;
				this.onSecondsChanged(_lastSentSeconds);
			}
		}
	}

	private int GetSecondsLeft()
	{
		return (int)Math.Ceiling(_timeLeft);
	}
}
