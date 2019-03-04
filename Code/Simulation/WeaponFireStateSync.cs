using Battle;
using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using Utility;

namespace Simulation
{
	internal sealed class WeaponFireStateSync : ITickable, ITickableBase
	{
		private class SyncedDamagePool
		{
			private const int SYNCED_DAMAGE_POOL_SIZE = 20;

			private const int SYNCED_DAMAGE_POOL_RESIZE = 10;

			private Stack<SyncedDamage> _syncedDamagePool = new Stack<SyncedDamage>();

			public SyncedDamagePool()
			{
				Allocate(20);
			}

			public SyncedDamage GetSyncedDamage()
			{
				if (_syncedDamagePool.Count == 0)
				{
					Allocate(10);
				}
				SyncedDamage syncedDamage = _syncedDamagePool.Pop();
				syncedDamage.hitCubeList.FastClear();
				return syncedDamage;
			}

			public void ReturnSyncedDamage(SyncedDamage syncedDamage)
			{
				_syncedDamagePool.Push(syncedDamage);
			}

			private void Allocate(int numberToAllocate)
			{
				for (int i = 0; i < numberToAllocate; i++)
				{
					_syncedDamagePool.Push(new SyncedDamage());
				}
			}
		}

		private class SyncedDamage
		{
			public int hitMachineId;

			public TargetType targetType;

			public FasterList<HitCubeInfo> hitCubeList = new FasterList<HitCubeInfo>(32);

			public SyncedDamage Populate(int hitMachineId, TargetType _targetType, ICollection<HitCubeInfo> _hitCubeList)
			{
				this.hitMachineId = hitMachineId;
				targetType = _targetType;
				hitCubeList.AddRange(_hitCubeList);
				return this;
			}
		}

		private Dictionary<float, SyncedDamage> _pendingWeaponDamage = new Dictionary<float, SyncedDamage>();

		private Dictionary<float, SyncedDamage> _pendingWeaponHeal = new Dictionary<float, SyncedDamage>();

		private FasterList<float> _objectsToRemove = new FasterList<float>();

		private SyncedDamagePool _syncedDamagePool = new SyncedDamagePool();

		private const float TIMEOUT = 4f;

		[Inject]
		public NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		public BattleTimer battleTimer
		{
			private get;
			set;
		}

		public void DamageReportedToServer(IWeaponFireStateSyncDependency dependency, TargetType targetType)
		{
			if (dependency.hitCubeInfo.Count <= 0)
			{
				return;
			}
			float timeStamp = dependency.timeStamp;
			while (_pendingWeaponDamage.ContainsKey(dependency.timeStamp))
			{
				dependency.MinorIncreaseToTimeStamp();
				if (timeStamp == dependency.timeStamp)
				{
					break;
				}
				timeStamp = dependency.timeStamp;
			}
			_pendingWeaponDamage[dependency.timeStamp] = _syncedDamagePool.GetSyncedDamage().Populate(dependency.hitMachineId, dependency.targetType, dependency.hitCubeInfo);
			MarkDamageOnCubes(_pendingWeaponDamage[dependency.timeStamp].hitCubeList, dependency.hitMachineId, dependency.targetType, adding: true);
		}

		public void ClearPendingData()
		{
			_pendingWeaponDamage.Clear();
			_pendingWeaponHeal.Clear();
		}

		public void HealReportedToServer(HealedAllyCubesDependency dependency, TargetType targetType)
		{
			float timeStamp = dependency.timeStamp;
			while (_pendingWeaponHeal.ContainsKey(dependency.timeStamp))
			{
				dependency.MinorIncreaseToTimeStamp();
				if (timeStamp == dependency.timeStamp)
				{
					break;
				}
				timeStamp = dependency.timeStamp;
			}
			SyncedDamage syncedDamage = _syncedDamagePool.GetSyncedDamage();
			syncedDamage.targetType = targetType;
			if (dependency.healedCubes != null && dependency.healedCubes.Count > 0)
			{
				syncedDamage.hitCubeList.AddRange((ICollection<HitCubeInfo>)dependency.healedCubes);
			}
			MarkHealingOnCubes(syncedDamage.hitCubeList, dependency.healedMachine, targetType, adding: true);
			_pendingWeaponHeal[timeStamp] = syncedDamage;
		}

		public void RecievedFireResponse(int shootingMachineId, int hitMachineId, float timeStamp, TargetType targetType)
		{
			if (!playerMachinesContainer.IsMachineRegistered(TargetType.Player, shootingMachineId))
			{
				return;
			}
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, shootingMachineId);
			if (playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId))
			{
				if (_pendingWeaponDamage.ContainsKey(timeStamp))
				{
					MarkDamageOnCubes(_pendingWeaponDamage[timeStamp].hitCubeList, hitMachineId, targetType, adding: false);
					_syncedDamagePool.ReturnSyncedDamage(_pendingWeaponDamage[timeStamp]);
					_pendingWeaponDamage.Remove(timeStamp);
				}
				else
				{
					Console.LogWarning("No weapon fire state to unregister");
				}
			}
		}

		public void RecievedHealResponse(HealedAllyCubesDependency dependency, TargetType targetType)
		{
			if (!playerMachinesContainer.IsMachineRegistered(TargetType.Player, dependency.shootingMachineId))
			{
				return;
			}
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, dependency.shootingMachineId);
			if (playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId))
			{
				if (_pendingWeaponHeal.ContainsKey(dependency.timeStamp))
				{
					MarkHealingOnCubes(_pendingWeaponHeal[dependency.timeStamp].hitCubeList, dependency.healedMachine, targetType, adding: false);
					_syncedDamagePool.ReturnSyncedDamage(_pendingWeaponHeal[dependency.timeStamp]);
					_pendingWeaponHeal.Remove(dependency.timeStamp);
				}
				else
				{
					Console.LogWarning("No weapon heal state to unregister");
				}
			}
		}

		private void MarkDamageOnCubes(FasterList<HitCubeInfo> cubesHit, int machineId, TargetType targetType, bool adding)
		{
			if (!machineManager.IsPlayerMapRegisterred(targetType, machineId))
			{
				return;
			}
			IMachineMap machineMap = machineManager.GetMachineMap(targetType, machineId);
			for (int i = 0; i < cubesHit.get_Count(); i++)
			{
				HitCubeInfo hitCubeInfo = cubesHit.get_Item(i);
				MachineCell cellAt = machineMap.GetCellAt(hitCubeInfo.gridLoc);
				if (adding)
				{
					cellAt.info.pendingDamage += hitCubeInfo.damage;
				}
				else
				{
					cellAt.info.pendingDamage -= hitCubeInfo.damage;
				}
			}
		}

		private void MarkHealingOnCubes(FasterList<HitCubeInfo> cubesHit, int machineId, TargetType targetType, bool adding)
		{
			if (!machineManager.IsPlayerMapRegisterred(targetType, machineId))
			{
				return;
			}
			IMachineMap machineMap = machineManager.GetMachineMap(targetType, machineId);
			for (int i = 0; i < cubesHit.get_Count(); i++)
			{
				HitCubeInfo hitCubeInfo = cubesHit.get_Item(i);
				MachineCell cellAt = machineMap.GetCellAt(hitCubeInfo.gridLoc);
				if (adding)
				{
					cellAt.info.pendingHealing += hitCubeInfo.damage;
				}
				else
				{
					cellAt.info.pendingHealing -= hitCubeInfo.damage;
				}
			}
		}

		public void Tick(float deltaTime)
		{
			_objectsToRemove.FastClear();
			Dictionary<float, SyncedDamage>.Enumerator enumerator = _pendingWeaponDamage.GetEnumerator();
			while (enumerator.MoveNext())
			{
				float key = enumerator.Current.Key;
				SyncedDamage value = enumerator.Current.Value;
				if (key + 4f < battleTimer.SecondsSinceGameInitialised)
				{
					_objectsToRemove.Add(key);
					MarkDamageOnCubes(value.hitCubeList, value.hitMachineId, value.targetType, adding: false);
				}
			}
			Dictionary<float, SyncedDamage>.Enumerator enumerator2 = _pendingWeaponHeal.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				float key2 = enumerator2.Current.Key;
				SyncedDamage value2 = enumerator2.Current.Value;
				if (key2 + 4f < battleTimer.SecondsSinceGameInitialised)
				{
					_objectsToRemove.Add(key2);
					MarkHealingOnCubes(value2.hitCubeList, value2.hitMachineId, value2.targetType, adding: false);
				}
			}
			for (int i = 0; i < _objectsToRemove.get_Count(); i++)
			{
				float key3 = _objectsToRemove.get_Item(i);
				if (_pendingWeaponDamage.ContainsKey(key3))
				{
					_syncedDamagePool.ReturnSyncedDamage(_pendingWeaponDamage[key3]);
				}
				if (_pendingWeaponHeal.ContainsKey(key3))
				{
					_syncedDamagePool.ReturnSyncedDamage(_pendingWeaponHeal[key3]);
				}
				_pendingWeaponDamage.Remove(key3);
				_pendingWeaponHeal.Remove(key3);
			}
		}
	}
}
