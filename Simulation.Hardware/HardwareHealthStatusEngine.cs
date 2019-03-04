using Svelto.ECS;
using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace Simulation.Hardware
{
	internal sealed class HardwareHealthStatusEngine : SingleEntityViewEngine<HardwareHealthStatusNode>
	{
		private MachineWeapons _machineWeapons = new MachineWeapons();

		private Dictionary<int, HardwareHealthStatusNode> _weapons = new Dictionary<int, HardwareHealthStatusNode>();

		private HardwareDestroyedObservable _destroyedObservable;

		private HardwareEnabledObservable _enabledObservable;

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		public HardwareHealthStatusEngine(HardwareDestroyedObservable destroyedObservable, HardwareEnabledObservable enabledObservable)
		{
			_destroyedObservable = destroyedObservable;
			_enabledObservable = enabledObservable;
		}

		protected override void Add(HardwareHealthStatusNode node)
		{
			if (!node.ownerComponent.ownedByMe)
			{
				return;
			}
			_weapons.Add(node.get_ID(), node);
			node.healthStatusComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)OnWeaponEnabled);
			if (node.healthStatusComponent.enabled)
			{
				ItemDescriptor itemDescriptor = node.itemDescriptorComponent.itemDescriptor;
				if (_machineWeapons.AddWeapon(itemDescriptor, node.get_ID(), node) == 1)
				{
					_enabledObservable.Dispatch(ref itemDescriptor);
				}
			}
		}

		protected override void Remove(HardwareHealthStatusNode node)
		{
			if (!node.ownerComponent.ownedByMe)
			{
				return;
			}
			if (node.healthStatusComponent.enabled)
			{
				ItemDescriptor itemDescriptor = node.itemDescriptorComponent.itemDescriptor;
				if (_machineWeapons.RemoveWeapon(itemDescriptor, node.get_ID()) == 0)
				{
					_destroyedObservable.Dispatch(ref itemDescriptor);
				}
			}
			_weapons.Remove(node.get_ID());
			node.healthStatusComponent.isPartEnabled.StopNotify((Action<int, bool>)OnWeaponEnabled);
		}

		private void OnWeaponEnabled(int weaponId, bool value)
		{
			HardwareHealthStatusNode hardwareHealthStatusNode = _weapons[weaponId];
			if (!hardwareHealthStatusNode.ownerComponent.ownedByMe)
			{
				return;
			}
			ItemDescriptor itemDescriptor = hardwareHealthStatusNode.itemDescriptorComponent.itemDescriptor;
			if (value)
			{
				if (_machineWeapons.AddWeapon(itemDescriptor, weaponId, hardwareHealthStatusNode) == 1)
				{
					_enabledObservable.Dispatch(ref itemDescriptor);
				}
			}
			else if (_machineWeapons.RemoveWeapon(itemDescriptor, weaponId) == 0)
			{
				_destroyedObservable.Dispatch(ref itemDescriptor);
			}
		}
	}
}
