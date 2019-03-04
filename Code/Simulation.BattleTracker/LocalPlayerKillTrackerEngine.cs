using Simulation.Hardware;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation.BattleTracker
{
	internal class LocalPlayerKillTrackerEngine : MultiEntityViewsEngine<WeaponTrackerNode, MovementTrackerNode>, IInitialize, IWaitForFrameworkDestruction
	{
		private PlayerKillData _playerKillData = new PlayerKillData();

		private LocalPlayerMadeKillObservable _observable;

		private int _localPlayerId = -1;

		private int _localPlayerMachineId = -1;

		private List<WeaponTrackerNode> _localPlayerWeapons = new List<WeaponTrackerNode>();

		private Dictionary<int, FasterList<ItemCategory>> _allPlayersMovementCategories = new Dictionary<int, FasterList<ItemCategory>>();

		[Inject]
		private DestructionReporter destructionReporter
		{
			get;
			set;
		}

		public LocalPlayerKillTrackerEngine(LocalPlayerMadeKillObservable observable)
		{
			_observable = observable;
		}

		public void OnDependenciesInjected()
		{
			destructionReporter.OnMachineKilled += CheckLocalPlayerMadeKill;
		}

		public void OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineKilled -= CheckLocalPlayerMadeKill;
		}

		protected override void Add(MovementTrackerNode movementNode)
		{
			int ownerId = movementNode.ownerComponent.ownerId;
			ItemCategory itemCategory = movementNode.itemDescriptorComponent.itemDescriptor.itemCategory;
			if (itemCategory == ItemCategory.Rudder)
			{
				itemCategory = ItemCategory.Wing;
			}
			if (!_allPlayersMovementCategories.ContainsKey(ownerId))
			{
				_allPlayersMovementCategories.Add(ownerId, new FasterList<ItemCategory>());
			}
			if (!_allPlayersMovementCategories[ownerId].Contains(itemCategory))
			{
				_allPlayersMovementCategories[ownerId].Add(itemCategory);
			}
		}

		protected override void Remove(MovementTrackerNode entityView)
		{
		}

		protected override void Add(WeaponTrackerNode weaponNode)
		{
			IHardwareOwnerComponent ownerComponent = weaponNode.ownerComponent;
			if (ownerComponent.ownedByMe)
			{
				_localPlayerId = ownerComponent.ownerId;
				_localPlayerMachineId = ownerComponent.machineId;
				_localPlayerWeapons.Add(weaponNode);
			}
		}

		protected override void Remove(WeaponTrackerNode entityView)
		{
		}

		private void CheckLocalPlayerMadeKill(int victimId, int shooterId)
		{
			if (shooterId == _localPlayerId)
			{
				_playerKillData.activeWeapon = GetActiveWeapon();
				FasterList<ItemCategory> victimMovements = new FasterList<ItemCategory>();
				if (_allPlayersMovementCategories.ContainsKey(victimId))
				{
					victimMovements = _allPlayersMovementCategories[victimId];
				}
				_playerKillData.victimMovements = victimMovements;
				_observable.Dispatch(ref _playerKillData);
			}
		}

		private ItemCategory GetActiveWeapon()
		{
			for (int i = 0; i < _localPlayerWeapons.Count; i++)
			{
				WeaponTrackerNode weaponTrackerNode = _localPlayerWeapons[i];
				if (weaponTrackerNode.weaponActiveComponent.active)
				{
					return weaponTrackerNode.itemDescriptorComponent.itemDescriptor.itemCategory;
				}
			}
			return ItemCategory.NotAFunctionalItem;
		}
	}
}
