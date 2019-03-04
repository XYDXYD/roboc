using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct CharacterInputAxisComparer : IEqualityComparer<CharacterInputAxis>
{
	public bool Equals(CharacterInputAxis x, CharacterInputAxis y)
	{
		return x == y;
	}

	public int GetHashCode(CharacterInputAxis obj)
	{
		return (int)obj;
	}
}
