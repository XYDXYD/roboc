using System;

namespace Simulation
{
	internal sealed class SurrenderCooldownTimer
	{
		private DateTime _lastTickedTime;

		private bool _isCooldownFinished = true;

		private double _cooldownTimeLeft = double.MaxValue;

		private int _lastDisplayedSeconds;

		private bool _timerStoppedBeforeEnd;

		private bool _timerRunning;

		public event Action<int> onCooldownSecondsChanged = delegate
		{
		};

		public event Action onCooldownTimerStarted = delegate
		{
		};

		public event Action onCooldownTimerEnded = delegate
		{
		};

		public void StartCounting(double totalCooldownTime)
		{
			_cooldownTimeLeft = totalCooldownTime;
			_lastTickedTime = DateTime.UtcNow;
			_timerRunning = true;
			_timerStoppedBeforeEnd = false;
			bool flag = IsCooldownFinished();
			if (flag != _isCooldownFinished)
			{
				_isCooldownFinished = flag;
				this.onCooldownTimerStarted();
			}
		}

		public void StopTimer()
		{
			_timerRunning = false;
			if (_cooldownTimeLeft > 0.0)
			{
				_timerStoppedBeforeEnd = true;
			}
		}

		public bool IsCooldownFinished()
		{
			return _cooldownTimeLeft <= 0.0 && _timerRunning;
		}

		public void Tick(float deltaTime)
		{
			if (!_timerRunning && !_timerStoppedBeforeEnd)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			float num = Convert.ToSingle((utcNow - _lastTickedTime).TotalSeconds);
			_lastTickedTime = utcNow;
			if (_cooldownTimeLeft > 0.0)
			{
				_cooldownTimeLeft -= num;
			}
			bool flag = IsCooldownFinished();
			if (flag != _isCooldownFinished)
			{
				_isCooldownFinished = flag;
				if (_timerRunning)
				{
					this.onCooldownTimerEnded();
				}
				_timerRunning = false;
				_timerStoppedBeforeEnd = false;
			}
			int secondsLeft = GetSecondsLeft();
			if (secondsLeft != _lastDisplayedSeconds || _lastDisplayedSeconds == 0)
			{
				_lastDisplayedSeconds = secondsLeft;
				if (_timerRunning)
				{
					this.onCooldownSecondsChanged(_lastDisplayedSeconds);
				}
			}
		}

		public int GetSecondsLeft()
		{
			return (int)Math.Ceiling(_cooldownTimeLeft);
		}
	}
}
