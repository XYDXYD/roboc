using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class NextWeaponToFireEngine : MultiEntityViewsEngine<NextWeaponToFireNode, HardwareHealthStatusNode>, IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private class WeaponContainer
		{
			public int currentWeaponIndex;

			public FasterList<NextWeaponToFireNode> activeWeapons = new FasterList<NextWeaponToFireNode>();

			public Dictionary<ItemDescriptor, Dictionary<int, NextWeaponToFireNode>> allWeapons = new Dictionary<ItemDescriptor, Dictionary<int, NextWeaponToFireNode>>();

			public FasterList<NextWeaponToFireNode> leftActiveWeapons = new FasterList<NextWeaponToFireNode>();

			public FasterList<NextWeaponToFireNode> rightActiveWeapons = new FasterList<NextWeaponToFireNode>();

			public FasterList<NextWeaponToFireNode> activeWeaponsInFireOrder = new FasterList<NextWeaponToFireNode>();

			private ItemDescriptor _currentActiveWeapon;

			public NextWeaponToFireNode nextWeaponToFire
			{
				get;
				set;
			}

			public ItemDescriptor currentActiveWeaponType
			{
				get
				{
					return _currentActiveWeapon;
				}
				set
				{
					_currentActiveWeapon = value;
				}
			}
		}

		private Dictionary<int, WeaponContainer> _weaponContainers = new Dictionary<int, WeaponContainer>();

		private WeaponReadyObserver _weaponReadyObserver;

		private NetworkWeaponFiredObserver _networkWeaponFiredObserver;

		[Inject]
		internal MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		[Inject]
		internal SwitchWeaponObserver switchWeaponObserver
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe NextWeaponToFireEngine(WeaponReadyObserver weaponReadyObserver, NetworkWeaponFiredObserver networkWeaponFiredObserver)
		{
			_weaponReadyObserver = weaponReadyObserver;
			_networkWeaponFiredObserver = networkWeaponFiredObserver;
			_weaponReadyObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_networkWeaponFiredObserver.AddAction(new ObserverAction<FiringInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Ready()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			switchWeaponObserver.SwitchWeaponsEvent += SwitchToNewWeaponCategory;
			switchWeaponObserver.RemoteSwitchWeaponsEvent += SwitchToNewWeaponCategory;
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			switchWeaponObserver.SwitchWeaponsEvent -= SwitchToNewWeaponCategory;
			switchWeaponObserver.RemoteSwitchWeaponsEvent -= SwitchToNewWeaponCategory;
			_weaponReadyObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_networkWeaponFiredObserver.RemoveAction(new ObserverAction<FiringInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(NextWeaponToFireNode node)
		{
			node.misfireComponent.weaponMisfired.subscribers += HandleWeaponMisfired;
			node.shootingComponent.shotIsGoingToBeFired.subscribers += HandleWeaponFired;
		}

		protected override void Add(HardwareHealthStatusNode node)
		{
			node.healthStatusComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)OnWeaponEnabled);
			if (node.healthStatusComponent.enabled)
			{
				OnWeaponEnabled(node.get_ID(), value: true);
			}
		}

		protected override void Remove(NextWeaponToFireNode node)
		{
			int machineId = node.ownerComponent.machineId;
			_weaponContainers.Remove(machineId);
			node.misfireComponent.weaponMisfired.subscribers -= HandleWeaponMisfired;
			node.shootingComponent.shotIsGoingToBeFired.subscribers -= HandleWeaponFired;
		}

		protected override void Remove(HardwareHealthStatusNode node)
		{
			node.healthStatusComponent.isPartEnabled.StopNotify((Action<int, bool>)OnWeaponEnabled);
		}

		private void HandleWeaponMisfired(IMisfireComponent sender, int weaponId)
		{
			NextWeaponToFireNode nextWeaponToFireNode = default(NextWeaponToFireNode);
			if (entityViewsDB.TryQueryEntityView<NextWeaponToFireNode>(weaponId, ref nextWeaponToFireNode))
			{
				int machineId = nextWeaponToFireNode.ownerComponent.machineId;
				if (_weaponContainers.TryGetValue(machineId, out WeaponContainer value))
				{
					FindNextWeaponToFire(value);
					int value2 = value.nextWeaponToFire.get_ID();
					value.nextWeaponToFire.shootingComponent.shotIsReadyToFire.Dispatch(ref value2);
				}
			}
		}

		private void HandleWeaponFired(IShootingComponent sender, int weaponId)
		{
			NextWeaponToFireNode nextWeaponToFireNode = default(NextWeaponToFireNode);
			if (entityViewsDB.TryQueryEntityView<NextWeaponToFireNode>(weaponId, ref nextWeaponToFireNode))
			{
				int machineId = nextWeaponToFireNode.ownerComponent.machineId;
				if (_weaponContainers.TryGetValue(machineId, out WeaponContainer value))
				{
					FindNextWeaponToFire(value);
				}
			}
		}

		private void OnWeaponEnabled(int weaponId, bool value)
		{
			NextWeaponToFireNode nextWeaponToFireNode = default(NextWeaponToFireNode);
			if (!entityViewsDB.TryQueryEntityView<NextWeaponToFireNode>(weaponId, ref nextWeaponToFireNode))
			{
				return;
			}
			int machineId = nextWeaponToFireNode.ownerComponent.machineId;
			ItemDescriptor itemDescriptor = nextWeaponToFireNode.itemDescriptorComponent.itemDescriptor;
			WeaponContainer weaponContainer = _weaponContainers[machineId];
			if (!weaponContainer.allWeapons.TryGetValue(itemDescriptor, out Dictionary<int, NextWeaponToFireNode> value2))
			{
				Dictionary<int, NextWeaponToFireNode> dictionary = new Dictionary<int, NextWeaponToFireNode>();
				weaponContainer.allWeapons[itemDescriptor] = dictionary;
				value2 = dictionary;
			}
			if (value)
			{
				value2.Add(weaponId, nextWeaponToFireNode);
				if (itemDescriptor.Equals(weaponContainer.currentActiveWeaponType))
				{
					SwitchToNewWeaponCategory(machineId, itemDescriptor);
				}
			}
			else
			{
				value2.Remove(weaponId);
				if (itemDescriptor.Equals(weaponContainer.currentActiveWeaponType))
				{
					SwitchToNewWeaponCategory(machineId, itemDescriptor);
				}
			}
		}

		private void SwitchToNewWeaponCategory(int machineId, ItemDescriptor itemDescriptor)
		{
			if (_weaponContainers.TryGetValue(machineId, out WeaponContainer value))
			{
				value.currentActiveWeaponType = itemDescriptor;
				value.activeWeapons.FastClear();
				if (value.allWeapons.ContainsKey(itemDescriptor))
				{
					Dictionary<int, NextWeaponToFireNode>.Enumerator enumerator = value.allWeapons[itemDescriptor].GetEnumerator();
					while (enumerator.MoveNext())
					{
						value.activeWeapons.Add(enumerator.Current.Value);
					}
				}
				OrderEnabledWeapons(value, machineId);
				FindNextWeaponToFire(value);
			}
			else
			{
				value = new WeaponContainer();
				value.currentActiveWeaponType = itemDescriptor;
				_weaponContainers.Add(machineId, value);
			}
		}

		private void SelectWeaponToFire(ref int machineId)
		{
			if (_weaponContainers.TryGetValue(machineId, out WeaponContainer value))
			{
				int value2 = value.nextWeaponToFire.get_ID();
				value.nextWeaponToFire.shootingComponent.shotIsReadyToFire.Dispatch(ref value2);
			}
		}

		private void FindNextWeaponToFire(WeaponContainer weaponContainer)
		{
			FasterList<NextWeaponToFireNode> activeWeaponsInFireOrder = weaponContainer.activeWeaponsInFireOrder;
			if (weaponContainer.currentWeaponIndex >= activeWeaponsInFireOrder.get_Count())
			{
				weaponContainer.currentWeaponIndex = 0;
			}
			if (activeWeaponsInFireOrder.get_Count() != 0)
			{
				weaponContainer.nextWeaponToFire = activeWeaponsInFireOrder.get_Item(weaponContainer.currentWeaponIndex++);
			}
			if (weaponContainer.nextWeaponToFire != null)
			{
				int value = weaponContainer.nextWeaponToFire.get_ID();
				weaponContainer.nextWeaponToFire.fireOrderComponent.nextElegibleWeaponToFire.Dispatch(ref value);
			}
		}

		private void OrderEnabledWeapons(WeaponContainer weaponContainer, int machineId)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			if (!machineRootContainer.IsMachineRegistered(TargetType.Player, machineId))
			{
				return;
			}
			GameObject machineRoot = machineRootContainer.GetMachineRoot(TargetType.Player, machineId);
			Vector3 val = Vector3.get_zero();
			int count = weaponContainer.activeWeapons.get_Count();
			for (int i = 0; i < count; i++)
			{
				val += weaponContainer.activeWeapons.get_Item(i).cubePositionComponent.position;
			}
			Vector3 val2 = Vector3.get_zero();
			if (count > 0)
			{
				val2 = val / (float)count;
			}
			weaponContainer.nextWeaponToFire = null;
			weaponContainer.rightActiveWeapons.FastClear();
			weaponContainer.leftActiveWeapons.FastClear();
			weaponContainer.activeWeaponsInFireOrder.FastClear();
			Vector3 right = machineRoot.get_transform().get_right();
			Vector3 forward = machineRoot.get_transform().get_forward();
			for (int j = 0; j < count; j++)
			{
				Vector3 val3 = weaponContainer.activeWeapons.get_Item(j).cubePositionComponent.position - val2;
				float num = Vector3.Dot(val3, right);
				float num2 = Vector3.Dot(val3, forward);
				if (num > 0f)
				{
					weaponContainer.rightActiveWeapons.Add(weaponContainer.activeWeapons.get_Item(j));
				}
				else if (num < 0f)
				{
					weaponContainer.leftActiveWeapons.Add(weaponContainer.activeWeapons.get_Item(j));
				}
				else if (num2 > 0f)
				{
					weaponContainer.rightActiveWeapons.Add(weaponContainer.activeWeapons.get_Item(j));
				}
				else
				{
					weaponContainer.leftActiveWeapons.Add(weaponContainer.activeWeapons.get_Item(j));
				}
			}
			int count2 = weaponContainer.leftActiveWeapons.get_Count();
			int count3 = weaponContainer.rightActiveWeapons.get_Count();
			if (Mathf.Abs(count2 - count3) > 1)
			{
				FasterList<NextWeaponToFireNode> val4 = (count2 >= count3) ? weaponContainer.rightActiveWeapons : weaponContainer.leftActiveWeapons;
				FasterList<NextWeaponToFireNode> val5 = (count2 <= count3) ? weaponContainer.rightActiveWeapons : weaponContainer.leftActiveWeapons;
				while (val5.get_Count() > val4.get_Count() + 1)
				{
					int num3 = val5.get_Count() - 1;
					val4.Add(val5.get_Item(num3));
					val5.UnorderedRemoveAt(num3);
				}
			}
			count2 = weaponContainer.leftActiveWeapons.get_Count();
			count3 = weaponContainer.rightActiveWeapons.get_Count();
			int num4 = (count2 >= count3) ? count3 : count2;
			for (int k = 0; k < num4; k++)
			{
				weaponContainer.activeWeaponsInFireOrder.Add(weaponContainer.leftActiveWeapons.get_Item(k));
				weaponContainer.activeWeaponsInFireOrder.Add(weaponContainer.rightActiveWeapons.get_Item(k));
			}
			FasterList<NextWeaponToFireNode> val6 = (count2 <= count3) ? weaponContainer.rightActiveWeapons : weaponContainer.leftActiveWeapons;
			for (int l = num4; l < val6.get_Count(); l++)
			{
				weaponContainer.activeWeaponsInFireOrder.Add(val6.get_Item(l));
			}
		}

		private void HandleRemoteWeaponFired(ref FiringInfo firingInfo)
		{
			NextWeaponToFireNode nextWeaponToFireNode = default(NextWeaponToFireNode);
			if (entityViewsDB.TryQueryEntityView<NextWeaponToFireNode>(firingInfo.weaponId, ref nextWeaponToFireNode))
			{
				int machineId = nextWeaponToFireNode.ownerComponent.machineId;
				if (_weaponContainers.TryGetValue(machineId, out WeaponContainer value))
				{
					FindNextWeaponToFire(value);
				}
			}
		}
	}
}
