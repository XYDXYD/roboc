using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ItemCategoryComparer : IEqualityComparer<ItemCategory>
{
	public bool Equals(ItemCategory x, ItemCategory y)
	{
		return x == y;
	}

	public int GetHashCode(ItemCategory obj)
	{
		return (int)obj;
	}
}
