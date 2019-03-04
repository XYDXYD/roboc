using System.Collections.Generic;

namespace Simulation.Hardware
{
	internal sealed class MachineWeapons
	{
		private Dictionary<ItemDescriptor, Dictionary<int, HardwareHealthStatusNode>> _weaponList = new Dictionary<ItemDescriptor, Dictionary<int, HardwareHealthStatusNode>>();

		public int AddWeapon(ItemDescriptor itemCategory, int weaponId, HardwareHealthStatusNode node)
		{
			if (_weaponList.ContainsKey(itemCategory))
			{
				if (!_weaponList[itemCategory].ContainsKey(weaponId))
				{
					_weaponList[itemCategory].Add(weaponId, node);
				}
			}
			else
			{
				Dictionary<int, HardwareHealthStatusNode> dictionary = new Dictionary<int, HardwareHealthStatusNode>();
				dictionary.Add(weaponId, node);
				_weaponList.Add(itemCategory, dictionary);
			}
			return _weaponList[itemCategory].Count;
		}

		public int RemoveWeapon(ItemDescriptor itemCategory, int weaponId)
		{
			if (_weaponList.TryGetValue(itemCategory, out Dictionary<int, HardwareHealthStatusNode> value))
			{
				value.Remove(weaponId);
				return value.Count;
			}
			return 0;
		}
	}
}
