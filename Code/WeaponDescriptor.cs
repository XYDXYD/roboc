internal class WeaponDescriptor : ItemDescriptor
{
	public override bool isActivable => true;

	public WeaponDescriptor(ItemCategory itemCategory, ItemSize itemSize)
		: base(itemCategory, itemSize)
	{
	}
}
