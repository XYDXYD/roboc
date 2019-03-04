using Services;
using Svelto.Context;
using Svelto.IoC;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal sealed class WeaponTypeEditModeCount : IWaitForFrameworkInitialization
	{
		private Dictionary<int, uint> weaponCounts = new Dictionary<int, uint>();

		private HashSet<InstantiatedCube> weapons = new HashSet<InstantiatedCube>();

		private int _totalWeaponSubCategory;

		[Inject]
		public IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		public ICubeList cubeList
		{
			private get;
			set;
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			Array values = Enum.GetValues(typeof(ItemSize));
			Array values2 = Enum.GetValues(typeof(ItemCategory));
			IEnumerator enumerator = values2.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ItemCategory itemCategory = (ItemCategory)enumerator.Current;
				IEnumerator enumerator2 = values.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					ItemSize itemSize = (ItemSize)enumerator2.Current;
					weaponCounts[ItemDescriptorKey.GenerateKey(itemCategory, itemSize)] = 0u;
				}
			}
			machineMap.OnAddCubeAt += OnAddCube;
			machineMap.OnRemoveCubeAt += OnRemoveCube;
			AddStartingCubes();
		}

		public uint GetItemDescriptorCount(ItemDescriptor itemDescriptor)
		{
			return weaponCounts[itemDescriptor.GenerateKey()];
		}

		public bool WillBeOverWeaponLimit(ItemDescriptor newSubCategory)
		{
			if (_totalWeaponSubCategory >= WeaponOrder.MAX_WEAPON_CATEGORY_PER_MACHINE && weaponCounts[newSubCategory.GenerateKey()] == 0)
			{
				return true;
			}
			return false;
		}

		public bool IsOverWeaponLimit()
		{
			return _totalWeaponSubCategory > WeaponOrder.MAX_WEAPON_CATEGORY_PER_MACHINE;
		}

		private void OnAddCube(Byte3 pos, MachineCell cell)
		{
			OnAddCube(cell.info);
		}

		private void OnAddCube(InstantiatedCube info)
		{
			ItemDescriptor itemDescriptor = info.persistentCubeData.itemDescriptor;
			if (itemDescriptor != null && itemDescriptor.isActivable)
			{
				Dictionary<int, uint> dictionary;
				int key;
				(dictionary = weaponCounts)[key = itemDescriptor.GenerateKey()] = dictionary[key] + 1;
				weapons.Add(info);
				RecalculateTotalWeaponSubCategories();
			}
		}

		private void OnRemoveCube(Byte3 pos, MachineCell cell)
		{
			ItemDescriptor itemDescriptor = cell.info.persistentCubeData.itemDescriptor;
			if (itemDescriptor != null && itemDescriptor.isActivable)
			{
				Dictionary<int, uint> dictionary;
				int key;
				(dictionary = weaponCounts)[key = itemDescriptor.GenerateKey()] = dictionary[key] - 1;
				weapons.Remove(cell.info);
				RecalculateTotalWeaponSubCategories();
			}
		}

		private void AddStartingCubes()
		{
			ICollection<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			IEnumerator<InstantiatedCube> enumerator = allInstantiatedCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				OnAddCube(current);
			}
		}

		private void RecalculateTotalWeaponSubCategories()
		{
			_totalWeaponSubCategory = 0;
			Dictionary<int, uint>.Enumerator enumerator = weaponCounts.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value != 0)
				{
					_totalWeaponSubCategory++;
				}
			}
		}
	}
}
