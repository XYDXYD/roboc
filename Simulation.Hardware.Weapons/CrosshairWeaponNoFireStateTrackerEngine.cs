using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal sealed class CrosshairWeaponNoFireStateTrackerEngine : MultiEntityViewsEngine<CrosshairWeaponNoFireStateTrackerNode, PowerBarNode>, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IInitialize, IStep<object>, IEngine, IStep
	{
		private WeaponReadyObserver _weaponReadyObserver;

		private WeaponCooldownEndedObserver _weaponCooldownEndedObserver;

		private HardwareDestroyedObserver _hardwareDestroyedObserver;

		private HardwareEnabledObserver _hardwareEnabledObserver;

		private HashSet<int> _weaponsOnCooldown = new HashSet<int>();

		private HashSet<int> _weaponsEnabled = new HashSet<int>();

		private ItemDescriptor _currentActiveWeapon;

		private int _localPlayerMachineId = -1;

		private PowerBarNode _powerBar;

		private const float COOLDOWN_THRESHOLD_SECONDS = 0.45f;

		[Inject]
		internal CrosshairController crosshairController
		{
			private get;
			set;
		}

		[Inject]
		internal SwitchWeaponObserver switchWeaponObserver
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe CrosshairWeaponNoFireStateTrackerEngine(WeaponReadyObserver weaponReadyObserver, HardwareDestroyedObserver hardwareDestroyedObserver, HardwareEnabledObserver hardwareEnabledObserver, WeaponCooldownEndedObserver cooldownEndedObserver)
		{
			_weaponReadyObserver = weaponReadyObserver;
			_weaponReadyObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_hardwareDestroyedObserver = hardwareDestroyedObserver;
			_hardwareDestroyedObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_hardwareEnabledObserver = hardwareEnabledObserver;
			_hardwareEnabledObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_weaponCooldownEndedObserver = cooldownEndedObserver;
			_weaponCooldownEndedObserver.AddAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		void IInitialize.OnDependenciesInjected()
		{
			switchWeaponObserver.SwitchWeaponsEvent += HandleSwitchWeaponsEvent;
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_weaponReadyObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_hardwareDestroyedObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_hardwareEnabledObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_weaponCooldownEndedObserver.RemoveAction(new ObserverAction<ItemDescriptor>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Step(ref object data, Enum condition)
		{
			Tick();
		}

		protected override void Add(CrosshairWeaponNoFireStateTrackerNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				_localPlayerMachineId = node.ownerComponent.machineId;
			}
		}

		protected override void Remove(CrosshairWeaponNoFireStateTrackerNode node)
		{
		}

		protected override void Add(PowerBarNode node)
		{
			_powerBar = node;
		}

		protected override void Remove(PowerBarNode node)
		{
			_powerBar = null;
		}

		private void HandleOnWeaponEnabled(ref ItemDescriptor itemDescriptor)
		{
			_weaponsEnabled.Add(itemDescriptor.GenerateKey());
		}

		private void HandleOnWeaponDestroyed(ref ItemDescriptor itemDescriptor)
		{
			_weaponsEnabled.Remove(itemDescriptor.GenerateKey());
		}

		private void HandleOnCooldownStart(ref int machineId)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			if (_localPlayerMachineId == -1)
			{
				_localPlayerMachineId = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerTeamsContainer.localPlayerId);
			}
			if (machineId != _localPlayerMachineId || (_currentActiveWeapon != null && _currentActiveWeapon.itemCategory == ItemCategory.Chaingun))
			{
				return;
			}
			FasterReadOnlyList<CrosshairWeaponNoFireStateTrackerNode> val = entityViewsDB.QueryEntityViews<CrosshairWeaponNoFireStateTrackerNode>();
			int num = 0;
			while (true)
			{
				if (num < val.get_Count())
				{
					CrosshairWeaponNoFireStateTrackerNode crosshairWeaponNoFireStateTrackerNode = val.get_Item(num);
					if (crosshairWeaponNoFireStateTrackerNode.ownerComponent.machineId == machineId && crosshairWeaponNoFireStateTrackerNode.itemDescriptorComponent.itemDescriptor.Equals(_currentActiveWeapon) && crosshairWeaponNoFireStateTrackerNode.cooldownComponent.weaponCooldown > 0.45f)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			int item = _currentActiveWeapon.GenerateKey();
			_weaponsOnCooldown.Add(item);
			crosshairController.ActivateNoFireState(active: true);
		}

		private void HandleOnCooldownEnd(ref ItemDescriptor item)
		{
			int item2 = item.GenerateKey();
			_weaponsOnCooldown.Remove(item2);
			crosshairController.ActivateNoFireState(active: false);
		}

		private void HandleSwitchWeaponsEvent(int machineId, ItemDescriptor itemDescriptor)
		{
			_currentActiveWeapon = itemDescriptor;
			crosshairController.ActivateNoFireState(active: false);
		}

		private void Tick()
		{
			crosshairController.ActivateNoFireState(!CanFire());
		}

		private bool CanFire()
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			if (_currentActiveWeapon == null || _powerBar == null)
			{
				return true;
			}
			int item = _currentActiveWeapon.GenerateKey();
			MachineWeaponsBlockedNode machineWeaponsBlockedNode = default(MachineWeaponsBlockedNode);
			if (entityViewsDB.TryQueryEntityView<MachineWeaponsBlockedNode>(_localPlayerMachineId, ref machineWeaponsBlockedNode))
			{
				bool blocked = machineWeaponsBlockedNode.machineWeaponsBlockedComponent.blocked;
				crosshairController.ActivateGroundWarning(machineWeaponsBlockedNode.machineWeaponsBlockedComponent.weaponNotGrounded);
				if (blocked)
				{
					return false;
				}
			}
			if (!_weaponsEnabled.Contains(item))
			{
				return false;
			}
			if (_weaponsOnCooldown.Contains(item) && _currentActiveWeapon.itemCategory != ItemCategory.Chaingun)
			{
				return false;
			}
			FasterReadOnlyList<CrosshairWeaponNoFireStateTrackerNode> val = entityViewsDB.QueryEntityViews<CrosshairWeaponNoFireStateTrackerNode>();
			IPowerBarDataComponent powerBarDataComponent = _powerBar.powerBarDataComponent;
			for (int i = 0; i < val.get_Count(); i++)
			{
				CrosshairWeaponNoFireStateTrackerNode crosshairWeaponNoFireStateTrackerNode = val.get_Item(i);
				if (crosshairWeaponNoFireStateTrackerNode.itemDescriptorComponent.itemDescriptor.Equals(_currentActiveWeapon))
				{
					if (crosshairWeaponNoFireStateTrackerNode.cooldownComponent.weaponCooldown < 0.45f)
					{
						break;
					}
					float num = crosshairWeaponNoFireStateTrackerNode.manaComponent.weaponFireCost;
					if (_currentActiveWeapon.itemCategory == ItemCategory.Chaingun)
					{
						num = 0f;
					}
					if (powerBarDataComponent.powerValue - num < 0f)
					{
						return false;
					}
					break;
				}
			}
			return true;
		}

		public void Ready()
		{
		}
	}
}
