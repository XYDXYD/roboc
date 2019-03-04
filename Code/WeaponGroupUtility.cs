internal static class WeaponGroupUtility
{
	private const int PAD = 11;

	public static int MakeID(int machineId, ItemDescriptor itemDescriptor)
	{
		int itemCategory = (int)itemDescriptor.itemCategory;
		int itemSize = (int)itemDescriptor.itemSize;
		int num = (machineId << 11) | itemCategory;
		return (num << 11) | itemSize;
	}
}
