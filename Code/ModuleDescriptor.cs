internal class ModuleDescriptor : ItemDescriptor
{
	public override bool isActivable => true;

	public ModuleDescriptor(ItemCategory itemCategory, ItemSize itemSize)
		: base(itemCategory, itemSize)
	{
	}
}
