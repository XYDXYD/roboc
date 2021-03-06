using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

internal sealed class LobbyCountdown : IWaitForFrameworkDestruction
{
	private int _lastTimeLeftComputed;

	private ITaskRoutine _countUp;

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	public event Action<int> onSecondsChanged = delegate
	{
	};

	public void StartCountUp()
	{
		if (_countUp != null)
		{
			_countUp.Stop();
		}
		_countUp = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)CountUp);
		_countUp.Start((Action<PausableTaskException>)null, (Action)null);
	}

	public int GetTimeWaiting()
	{
		return _lastTimeLeftComputed;
	}

	public void StopCount()
	{
		if (_countUp != null)
		{
			_countUp.Stop();
			_countUp = null;
			this.onSecondsChanged(0);
		}
	}

	void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		StopCount();
	}

	private IEnumerator CountUp()
	{
		DateTime timeStart = DateTime.UtcNow;
		while (true)
		{
			float secondsWaited = (float)(DateTime.UtcNow - timeStart).TotalSeconds;
			int secondsWait = Mathf.FloorToInt(secondsWaited);
			if (secondsWait != _lastTimeLeftComputed || _lastTimeLeftComputed == 0)
			{
				this.onSecondsChanged(secondsWait);
				_lastTimeLeftComputed = secondsWait;
			}
			yield return null;
		}
	}
}
