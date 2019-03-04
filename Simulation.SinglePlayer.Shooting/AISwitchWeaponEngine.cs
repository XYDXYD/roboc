using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation.SinglePlayer.Shooting
{
	internal class AISwitchWeaponEngine : MultiEntityViewsEngine<AIAgentDataComponentsNode, WeaponSwitchNode>, IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkDestruction, IEngine
	{
		private Dictionary<int, ItemCategory> _selectedWeapons = new Dictionary<int, ItemCategory>();

		[Inject]
		internal SwitchWeaponObserver switchWeaponObserver
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		protected override void Add(AIAgentDataComponentsNode obj)
		{
			if (_selectedWeapons.ContainsKey(obj.get_ID()))
			{
				obj.aiEquippedWeaponComponent.itemCategory = _selectedWeapons[obj.get_ID()];
			}
		}

		protected override void Remove(AIAgentDataComponentsNode obj)
		{
			_selectedWeapons.Remove(obj.get_ID());
		}

		protected override void Add(WeaponSwitchNode node)
		{
			if (node.weaponOwnerComponent.ownedByAi)
			{
				node.weaponActiveComponent.active = true;
				switchWeaponObserver.RemoteSwitchWeapon(node.weaponOwnerComponent.machineId, node.itemDescriptorComponent.itemDescriptor);
			}
		}

		protected override void Remove(WeaponSwitchNode node)
		{
			node.weaponActiveComponent.active = false;
		}

		public void OnDependenciesInjected()
		{
			switchWeaponObserver.RemoteSwitchWeaponsEvent += HandleSwitchWeapon;
		}

		public void OnFrameworkDestroyed()
		{
			switchWeaponObserver.RemoteSwitchWeaponsEvent -= HandleSwitchWeapon;
		}

		private void HandleSwitchWeapon(int machineId, ItemDescriptor itemDescriptor)
		{
			AIAgentDataComponentsNode aIAgentDataComponentsNode = default(AIAgentDataComponentsNode);
			if (entityViewsDB.TryQueryEntityView<AIAgentDataComponentsNode>(machineId, ref aIAgentDataComponentsNode))
			{
				aIAgentDataComponentsNode.aiEquippedWeaponComponent.itemCategory = itemDescriptor.itemCategory;
			}
			else
			{
				_selectedWeapons[machineId] = itemDescriptor.itemCategory;
			}
		}

		public void Ready()
		{
		}
	}
}
