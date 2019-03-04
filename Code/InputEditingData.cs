using System;

internal sealed class InputEditingData
{
	public float[] data = new float[Enum.GetValues(typeof(EditingInputAxis)).Length];

	public float this[EditingInputAxis axis]
	{
		get
		{
			return data[(int)axis];
		}
		set
		{
			data[(int)axis] = value;
		}
	}

	public void Reset()
	{
		Array.Clear(data, 0, data.Length);
	}

	public void Set(EditingInputAxis axis, float val)
	{
		data[(int)axis] = val;
	}
}
