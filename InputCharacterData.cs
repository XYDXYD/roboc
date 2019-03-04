using System;

internal sealed class InputCharacterData
{
	public float[] data = new float[Enum.GetValues(typeof(CharacterInputAxis)).Length];

	public float this[CharacterInputAxis axis]
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

	public void Set(CharacterInputAxis axis, float val)
	{
		data[(int)axis] = val;
	}
}
