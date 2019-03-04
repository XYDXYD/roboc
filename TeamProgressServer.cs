using System;
using UnityEngine;

internal sealed class TeamProgressServer
{
	private float _progress;

	private float _maxProgress = 4f;

	private bool _hasCompleted;

	public event Action onProgressFull;

	public float GetCurrentProgress()
	{
		return _progress;
	}

	public float GetMaxProgress()
	{
		return _maxProgress;
	}

	public bool GetCompleted()
	{
		return _hasCompleted;
	}

	public void AddProgress(float amount)
	{
		if (!(_progress < _maxProgress))
		{
			return;
		}
		_progress += amount;
		if (_progress >= _maxProgress)
		{
			_hasCompleted = true;
			if (this.onProgressFull != null)
			{
				this.onProgressFull();
			}
		}
	}

	public void SubtractProgress(float amount)
	{
		int num = (int)_progress;
		float num2 = _progress - (float)num;
		num2 -= amount;
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		_progress = num2 + (float)num;
	}

	public void ResetCurrentSegment()
	{
		_progress = Mathf.Floor(_progress);
	}
}
