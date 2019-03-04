using System;

internal static class ItemDescriptorKey
{
	public const int ITEM_DESCRIPTOR_KEY_MULTIPLIER = 100000;

	public static int GenerateKey(ItemCategory itemCategory, ItemSize itemSize)
	{
		return (int)((int)itemCategory * 100000 + itemSize);
	}

	public static int GenerateKey(this ItemDescriptor descriptor)
	{
		return (int)((int)descriptor.itemCategory * 100000 + descriptor.itemSize);
	}

	public static void GetItemInfoFromKey(int Key, out int itemCategory, out int itemSize)
	{
		itemCategory = (int)Math.Truncate((float)Key / 100000f);
		itemSize = Key % 100000;
	}

	public static void GetItemCategoryFromKey(int Key, out int itemCategory)
	{
		itemCategory = (int)Math.Truncate((float)Key / 100000f);
	}
}
