using System;

namespace Simulation
{
	internal sealed class SurrenderVoteTimer
	{
		private DateTime _lastTickedTime;

		private double _voteTimeLeft = double.MaxValue;

		private bool _isVoteTimeFinished;

		public event Action onVoteTimerFinished = delegate
		{
		};

		public event Action<double> onVoteTimeChanged = delegate
		{
		};

		public void StartVotingTimer(int timeLeft)
		{
			_isVoteTimeFinished = false;
			_voteTimeLeft = timeLeft;
			_lastTickedTime = DateTime.UtcNow;
		}

		public void EndVoting()
		{
			_isVoteTimeFinished = true;
		}

		public void StopTimer()
		{
			_isVoteTimeFinished = true;
		}

		public void Tick(float deltaTime)
		{
			if (!_isVoteTimeFinished)
			{
				DateTime utcNow = DateTime.UtcNow;
				float num = Convert.ToSingle((utcNow - _lastTickedTime).TotalSeconds);
				_lastTickedTime = utcNow;
				if (_voteTimeLeft > 0.0)
				{
					_voteTimeLeft -= num;
				}
				bool flag = _voteTimeLeft <= 0.0;
				if (flag != _isVoteTimeFinished)
				{
					_isVoteTimeFinished = flag;
					this.onVoteTimerFinished();
					_voteTimeLeft = 0.0;
				}
				this.onVoteTimeChanged(_voteTimeLeft);
			}
		}

		private int GetSecondsLeft()
		{
			return (int)Math.Ceiling(_voteTimeLeft);
		}
	}
}
