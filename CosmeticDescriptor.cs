internal class CosmeticDescriptor : ItemDescriptor
{
	public override bool isActivable => false;

	public CosmeticDescriptor(ItemCategory itemCategory, ItemSize itemSize)
		: base(itemCategory, itemSize)
	{
	}
}
