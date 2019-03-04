using System.Collections.Generic;

internal sealed class PlayerHistoryBuffer<T> where T : PlayerHistoryFrame, new()
{
	private List<T> _history;

	private int _writeIndex;

	private int _lastWrittenState = -1;

	private int _populatedStateCount;

	public PlayerHistoryBuffer(int bufferSize)
	{
		_history = new List<T>(bufferSize);
		for (int i = 0; i < bufferSize; i++)
		{
			_history.Add(new T());
		}
	}

	public void Flush(T frame)
	{
		for (int i = 0; i < _history.Count; i++)
		{
			AddData(frame);
		}
	}

	public void AddData(T frame)
	{
		_lastWrittenState = _writeIndex;
		_history[_writeIndex] = frame;
		_writeIndex++;
		if (_writeIndex >= _history.Count)
		{
			_writeIndex = 0;
		}
		if (_populatedStateCount < _history.Count)
		{
			_populatedStateCount++;
		}
	}

	public T GetState(int index)
	{
		if (index < _history.Count)
		{
			return _history[index];
		}
		return (T)null;
	}

	public int GetStateCount()
	{
		return _history.Count;
	}

	public int GetPopulatedStateCount()
	{
		return _populatedStateCount;
	}

	public int GetLastWrittenStateIndex()
	{
		return _lastWrittenState;
	}

	public int GetWritingState()
	{
		return _writeIndex;
	}

	public T[] GetStateChangesBetween(float startTime, float endTime)
	{
		List<T> list = new List<T>();
		for (int i = 0; i < _history.Count; i++)
		{
			T val = _history[i];
			if (val.timeStamp >= startTime && val.timeStamp < endTime)
			{
				list.Add(val);
			}
		}
		return list.ToArray();
	}
}
