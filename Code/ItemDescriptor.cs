using System;
using System.Collections.Generic;

public abstract class ItemDescriptor : IEqualityComparer<ItemDescriptor>, IEquatable<ItemDescriptor>
{
	private ItemCategory _itemCategory;

	private ItemSize _itemSize;

	public abstract bool isActivable
	{
		get;
	}

	public ItemCategory itemCategory
	{
		get
		{
			return _itemCategory;
		}
		set
		{
			_itemCategory = value;
		}
	}

	public ItemSize itemSize
	{
		get
		{
			return _itemSize;
		}
		set
		{
			_itemSize = value;
		}
	}

	public ItemDescriptor(ItemCategory itemCategory, ItemSize itemSize)
	{
		_itemCategory = itemCategory;
		_itemSize = itemSize;
	}

	bool IEquatable<ItemDescriptor>.Equals(ItemDescriptor other)
	{
		return Equals(other);
	}

	public bool Equals(ItemDescriptor x, ItemDescriptor y)
	{
		return x.Equals(y);
	}

	public int GetHashCode(ItemDescriptor obj)
	{
		return obj.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return this == obj as ItemDescriptor;
	}

	public static bool operator ==(ItemDescriptor w1, ItemDescriptor w2)
	{
		if (object.ReferenceEquals(w1, w2))
		{
			return true;
		}
		if ((object)w1 == null || (object)w2 == null)
		{
			return false;
		}
		return w1.itemCategory == w2.itemCategory && w1.itemSize == w2.itemSize;
	}

	public static bool operator !=(ItemDescriptor a, ItemDescriptor b)
	{
		return !(a == b);
	}

	public override int GetHashCode()
	{
		return this.GenerateKey();
	}
}
