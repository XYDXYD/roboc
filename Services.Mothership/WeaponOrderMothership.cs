using Svelto.DataStructures;

namespace Services.Mothership
{
	internal sealed class WeaponOrderMothership : WeaponOrder
	{
		private FasterList<int> _overCapacitySlots = new FasterList<int>();

		public WeaponOrderMothership(int[] weaponOrderList)
			: base(weaponOrderList)
		{
			if (_order.get_Count() > WeaponOrder.MAX_WEAPON_CATEGORY_PER_MACHINE)
			{
				for (int num = _order.get_Count() - 1; num >= WeaponOrder.MAX_WEAPON_CATEGORY_PER_MACHINE; num--)
				{
					_overCapacitySlots.Add(_order.get_Item(num));
					_order.RemoveAt(num);
				}
			}
		}

		public bool Set(int index, int itemDescriptorKey)
		{
			if (index >= _order.get_Count())
			{
				int count = _order.get_Count();
				_order.Resize(index + 1);
				for (int i = count; i < _order.get_Count(); i++)
				{
					_order.set_Item(i, 0);
				}
			}
			if (itemDescriptorKey != 0)
			{
				if (!Contains(itemDescriptorKey))
				{
					_order.set_Item(index, itemDescriptorKey);
					return true;
				}
				return false;
			}
			RemoveAt(index);
			return true;
		}

		public void Remove(int itemDescriptorKey)
		{
			int num = _order.IndexOf(itemDescriptorKey);
			if (num > -1)
			{
				RemoveAt(num);
			}
			else
			{
				_overCapacitySlots.Remove(itemDescriptorKey);
			}
		}

		private void RemoveAt(int index)
		{
			int num = 0;
			if (_overCapacitySlots.get_Count() > 0)
			{
				num = _overCapacitySlots.get_Item(_overCapacitySlots.get_Count() - 1);
				_overCapacitySlots.RemoveAt(_overCapacitySlots.get_Count() - 1);
			}
			_order.set_Item(index, num);
		}
	}
}
