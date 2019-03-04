internal class MovementDescriptor : ItemDescriptor
{
	public override bool isActivable => false;

	public MovementDescriptor(ItemCategory itemCategory, ItemSize itemSize)
		: base(itemCategory, itemSize)
	{
	}
}
