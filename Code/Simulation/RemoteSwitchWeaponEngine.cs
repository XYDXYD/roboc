using Services.Simulation;
using Simulation.Hardware;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class RemoteSwitchWeaponEngine : MultiEntityViewsEngine<WeaponSwitchNode, MachineWeaponOrderView>, IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private Dictionary<int, int> _initialCategoryPerMachine = new Dictionary<int, int>(30);

		[Inject]
		internal SwitchWeaponObserver switchWeaponObserver
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			switchWeaponObserver.RemoteSwitchWeaponsEvent += SetActiveWeapons;
		}

		protected override void Add(WeaponSwitchNode node)
		{
			if (!node.weaponOwnerComponent.ownedByMe)
			{
				int machineId = node.weaponOwnerComponent.machineId;
				if (_initialCategoryPerMachine.TryGetValue(machineId, out int value))
				{
					int num = node.itemDescriptorComponent.itemDescriptor.GenerateKey();
					node.weaponActiveComponent.active = (num == value);
				}
			}
		}

		protected override void Remove(WeaponSwitchNode node)
		{
			if (!node.weaponOwnerComponent.ownedByMe)
			{
				node.weaponActiveComponent.active = false;
			}
		}

		protected override void Add(MachineWeaponOrderView entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				return;
			}
			WeaponOrderSimulation weaponOrder = entityView.orderComponent.weaponOrder;
			int num = 0;
			int itemDescriptorKeyByIndex;
			ItemDescriptor itemDescriptorFromCube;
			while (true)
			{
				if (num >= weaponOrder.Count())
				{
					return;
				}
				itemDescriptorKeyByIndex = weaponOrder.GetItemDescriptorKeyByIndex(num);
				if (itemDescriptorKeyByIndex > 0)
				{
					itemDescriptorFromCube = cubeList.GetItemDescriptorFromCube(itemDescriptorKeyByIndex);
					if (itemDescriptorFromCube is WeaponDescriptor)
					{
						break;
					}
				}
				num++;
			}
			int ownerMachineId = entityView.ownerComponent.ownerMachineId;
			_initialCategoryPerMachine[ownerMachineId] = itemDescriptorKeyByIndex;
			switchWeaponObserver.RemoteSwitchWeapon(ownerMachineId, itemDescriptorFromCube);
		}

		protected override void Remove(MachineWeaponOrderView entityView)
		{
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			switchWeaponObserver.RemoteSwitchWeaponsEvent -= SetActiveWeapons;
		}

		private void SetActiveWeapons(int machineId, ItemDescriptor itemDescriptor)
		{
			int num = default(int);
			WeaponSwitchNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<WeaponSwitchNode>(machineId, ref num);
			for (int i = 0; i < num; i++)
			{
				WeaponSwitchNode weaponSwitchNode = array[i];
				weaponSwitchNode.weaponActiveComponent.active = weaponSwitchNode.itemDescriptorComponent.itemDescriptor.Equals(itemDescriptor);
			}
		}
	}
}
