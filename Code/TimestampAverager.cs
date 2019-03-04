internal sealed class TimestampAverager
{
	private float[] _intervals;

	private float _lastValue;

	private int _writeValue;

	private int _writeCount;

	public TimestampAverager(int size)
	{
		_intervals = new float[size];
	}

	public void AddValue(float data)
	{
		_intervals[_writeValue] = data - _lastValue;
		_lastValue = data;
		if (_writeCount < _intervals.Length)
		{
			_writeCount++;
		}
		if (++_writeValue == _intervals.Length)
		{
			_writeValue = 0;
		}
	}

	public bool IsFull()
	{
		return _writeCount == _intervals.Length;
	}

	public float GetAverage()
	{
		float num = 0f;
		for (int i = 0; i < _intervals.Length; i++)
		{
			num += _intervals[i];
		}
		return num / (float)_intervals.Length;
	}
}
