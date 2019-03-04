using Svelto.DataStructures;

namespace Services
{
	public class WeaponOrder
	{
		public static int MAX_WEAPON_CATEGORY_PER_MACHINE = 5;

		protected FasterList<int> _order = new FasterList<int>(MAX_WEAPON_CATEGORY_PER_MACHINE);

		public WeaponOrder(int[] weaponOrderList)
		{
			Clear();
			_order.AddRange(weaponOrderList);
		}

		public void Clear()
		{
			_order.FastClear();
		}

		public int GetItemDescriptorKeyByIndex(int index)
		{
			if (index >= _order.get_Count())
			{
				return 0;
			}
			return _order.get_Item(index);
		}

		public int GetIndexByItemDescriptor(ItemDescriptor itemDescriptor)
		{
			int num = itemDescriptor.GenerateKey();
			return _order.IndexOf(num);
		}

		public int Count()
		{
			return _order.get_Count();
		}

		public int GetFirstItemDescriptorKey()
		{
			for (int i = 0; i < _order.get_Count(); i++)
			{
				int num = _order.get_Item(i);
				if (num != 0)
				{
					return num;
				}
			}
			return 0;
		}

		public bool Contains(ItemDescriptor itemDescriptor)
		{
			if (itemDescriptor != null)
			{
				int num = itemDescriptor.GenerateKey();
				return _order.Contains(num);
			}
			return false;
		}

		protected bool Contains(int itemDescriptorKey)
		{
			if (itemDescriptorKey != 0)
			{
				return _order.Contains(itemDescriptorKey);
			}
			return false;
		}

		public FasterList<ItemCategory> GetItemCategories()
		{
			FasterList<ItemCategory> val = new FasterList<ItemCategory>();
			for (int i = 0; i < _order.get_Count(); i++)
			{
				int num = _order.get_Item(i);
				if (num != 0)
				{
					ItemDescriptorKey.GetItemCategoryFromKey(num, out int itemCategory);
					ItemCategory itemCategory2 = (ItemCategory)itemCategory;
					if (!val.Contains(itemCategory2))
					{
						val.Add(itemCategory2);
					}
				}
			}
			return val;
		}

		public int[] Serialise()
		{
			return _order.ToArray();
		}
	}
}
