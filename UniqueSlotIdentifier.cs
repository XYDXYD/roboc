using System;

public class UniqueSlotIdentifier : IEquatable<UniqueSlotIdentifier>
{
	private uint _compositeIDPart1;

	private uint _compositeIDPart2;

	public UniqueSlotIdentifier()
	{
		_compositeIDPart1 = 0u;
		_compositeIDPart2 = 0u;
	}

	public UniqueSlotIdentifier(UniqueSlotIdentifier other)
	{
		_compositeIDPart1 = other._compositeIDPart1;
		_compositeIDPart2 = other._compositeIDPart2;
	}

	public UniqueSlotIdentifier(uint keyPart1, uint keyPart2)
	{
		_compositeIDPart1 = keyPart1;
		_compositeIDPart2 = keyPart2;
	}

	public UniqueSlotIdentifier(string fromString)
	{
		string[] array = fromString.Split('_');
		_compositeIDPart1 = uint.Parse(array[0]);
		_compositeIDPart2 = uint.Parse(array[1]);
	}

	public bool Equals(UniqueSlotIdentifier obj)
	{
		return obj != null && obj._compositeIDPart1 == _compositeIDPart1 && obj._compositeIDPart2 == _compositeIDPart2;
	}

	public override bool Equals(object obj)
	{
		UniqueSlotIdentifier uniqueSlotIdentifier = obj as UniqueSlotIdentifier;
		if (uniqueSlotIdentifier == null)
		{
			return false;
		}
		return _compositeIDPart1 == uniqueSlotIdentifier._compositeIDPart1 && _compositeIDPart2 == uniqueSlotIdentifier._compositeIDPart2;
	}

	public override int GetHashCode()
	{
		if (_compositeIDPart2 >= int.MaxValue)
		{
			return -Convert.ToInt32(_compositeIDPart2 - int.MaxValue);
		}
		return Convert.ToInt32(_compositeIDPart2);
	}

	public override string ToString()
	{
		return _compositeIDPart1 + "_" + _compositeIDPart2;
	}
}
