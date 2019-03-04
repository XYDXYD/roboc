using Svelto.Context;
using Svelto.IoC;
using System.Collections.Generic;

internal sealed class ItemDescriptorSpriteUtility : IWaitForFrameworkInitialization
{
	private readonly Dictionary<ItemCategory, Dictionary<ItemSize, string>> _itemDesciptorSprites = new Dictionary<ItemCategory, Dictionary<ItemSize, string>>();

	[Inject]
	internal ICubeList cubeList
	{
		private get;
		set;
	}

	public void OnFrameworkInitialized()
	{
		ExtractWeaponSubCategorySprites();
	}

	public string GetSprite(ItemCategory itemCategory, ItemSize itemSize)
	{
		Dictionary<ItemSize, string> dictionary = _itemDesciptorSprites[itemCategory];
		return dictionary[itemSize];
	}

	public string GetSprite(int itemCategoryInt, int itemSizeInt)
	{
		return GetSprite((ItemCategory)itemCategoryInt, (ItemSize)itemSizeInt);
	}

	public string GetSprite(ItemDescriptor itemDescriptor)
	{
		return GetSprite(itemDescriptor.itemCategory, itemDescriptor.itemSize);
	}

	public string GetSprite(int itemDescriptorKey)
	{
		ItemDescriptorKey.GetItemInfoFromKey(itemDescriptorKey, out int itemCategory, out int itemSize);
		return GetSprite(itemCategory, itemSize);
	}

	private void ExtractWeaponSubCategorySprites()
	{
		List<CubeTypeID>.Enumerator enumerator = this.cubeList.cubeKeys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ICubeList cubeList = this.cubeList;
			CubeTypeID current = enumerator.Current;
			CubeTypeData cubeTypeData = cubeList.CubeTypeDataOf(current.ID);
			ItemDescriptor itemDescriptor = cubeTypeData.cubeData.itemDescriptor;
			if (!(itemDescriptor == null) && itemDescriptor.isActivable)
			{
				ItemCategory itemCategory = itemDescriptor.itemCategory;
				ItemSize itemSize = itemDescriptor.itemSize;
				if (!_itemDesciptorSprites.ContainsKey(itemCategory))
				{
					_itemDesciptorSprites.Add(itemCategory, new Dictionary<ItemSize, string>());
				}
				if (!_itemDesciptorSprites[itemCategory].ContainsKey(itemSize))
				{
					_itemDesciptorSprites[itemCategory].Add(itemSize, cubeTypeData.spriteName);
				}
			}
		}
		_itemDesciptorSprites[ItemCategory.Laser][ItemSize.T0] = "Top_Laser_Tiny";
		_itemDesciptorSprites[ItemCategory.Laser][ItemSize.T1] = "Top_Laser_Small";
		_itemDesciptorSprites[ItemCategory.Laser][ItemSize.T2] = "Top_Laser_Medium";
		_itemDesciptorSprites[ItemCategory.Laser][ItemSize.T3] = "Top_Laser_Large";
		_itemDesciptorSprites[ItemCategory.Laser][ItemSize.T4] = "Top_Laser_Huge";
		_itemDesciptorSprites[ItemCategory.Laser][ItemSize.T5] = "Mega_Laser";
	}
}
