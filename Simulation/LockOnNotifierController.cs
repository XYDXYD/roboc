using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class LockOnNotifierController : IInitialize, IWaitForFrameworkDestruction
	{
		private const float LOCK_SOUND_MODULATION_TIME = 3f;

		private IDictionary<int, WeaponStatsData> _weaponStatsDatas;

		private Dictionary<int, Dictionary<int, int>> _targetToRocketOwnerAndLockStage;

		private Dictionary<int, int> _effectOwnersToLockStage = new Dictionary<int, int>();

		private Dictionary<int, int> _machineIdToCurrentWeaponKey = new Dictionary<int, int>();

		private int _currentLockAlert;

		private float _currentLockTime;

		[Inject]
		internal DestructionReporter destructionReporter
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

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal LockOnStateObservable lockOnStateObservable
		{
			private get;
			set;
		}

		[Inject]
		internal LockedOnEffectPresenter lockedOnEffectPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public event Action<int> OnOwnPlayerLockedOn = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			destructionReporter.OnMachineDestroyed += MachineDestroyed;
			_targetToRocketOwnerAndLockStage = new Dictionary<int, Dictionary<int, int>>();
			TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator((IEnumerator)new LoopActionEnumerator((Action)Update))
				.Start((Action<PausableTaskException>)null, (Action)null);
			ILoadWeaponStatsRequest service = serviceFactory.Create<ILoadWeaponStatsRequest>();
			TaskService<IDictionary<int, WeaponStatsData>> taskService = new TaskService<IDictionary<int, WeaponStatsData>>(service);
			taskService.Execute();
			_weaponStatsDatas = taskService.result;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineDestroyed -= MachineDestroyed;
		}

		private void Update()
		{
			if (_currentLockAlert == 3 && _currentLockTime < 3f)
			{
				_currentLockTime += Time.get_deltaTime();
				if (_currentLockTime > 3f)
				{
					_currentLockTime = 3f;
				}
				EventManager.get_Instance().SetParameter("RocketLauncher_Player_Locked_Loop", "Time", _currentLockTime, null);
			}
		}

		public void SetPlayerLockAlert(int rocketOwner, int targetPlayer, int lockStage, Byte3 lockedCubePosition, ItemCategory itemCategory, ItemSize itemSize)
		{
			int num = ItemDescriptorKey.GenerateKey(itemCategory, itemSize);
			WeaponStatsData weaponStatsData = _weaponStatsDatas[num];
			if (weaponStatsData.lockTime > 0f)
			{
				_machineIdToCurrentWeaponKey[rocketOwner] = num;
				SetLockStage(targetPlayer, rocketOwner, lockStage);
				ApplyLockEffect(targetPlayer, num);
			}
			LockOnData lockOnData = new LockOnData(rocketOwner, targetPlayer, playerMachinesContainer.GetActiveMachine(TargetType.Player, targetPlayer), lockStage == 3, lockedCubePosition);
			lockOnStateObservable.Dispatch(ref lockOnData);
		}

		private bool HasPlayerAnyLockOnSelf(int playerId)
		{
			if (!_targetToRocketOwnerAndLockStage.ContainsKey(playerTeamsContainer.localPlayerId))
			{
				return false;
			}
			Dictionary<int, int> dictionary = _targetToRocketOwnerAndLockStage[playerTeamsContainer.localPlayerId];
			return dictionary.ContainsKey(playerId) && dictionary[playerId] > 0;
		}

		private void MachineDestroyed(int playerId, int machineId, bool isMe)
		{
			bool flag = HasPlayerAnyLockOnSelf(playerId);
			if (_targetToRocketOwnerAndLockStage.ContainsKey(playerId))
			{
				_targetToRocketOwnerAndLockStage[playerId].Clear();
			}
			Dictionary<int, Dictionary<int, int>>.Enumerator enumerator = _targetToRocketOwnerAndLockStage.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<int, int> value = enumerator.Current.Value;
				if (value.ContainsKey(playerId))
				{
					value.Remove(playerId);
				}
			}
			if (flag)
			{
				if (_machineIdToCurrentWeaponKey.ContainsKey(playerTeamsContainer.localPlayerId))
				{
					int localPlayerId = playerTeamsContainer.localPlayerId;
					ApplyLockEffect(localPlayerId, _machineIdToCurrentWeaponKey[localPlayerId]);
				}
			}
			else if (_machineIdToCurrentWeaponKey.ContainsKey(playerId))
			{
				ApplyLockEffect(playerId, _machineIdToCurrentWeaponKey[playerId]);
			}
		}

		private void ApplyLockEffect(int targetedPlayer, int itemDescriptorKey)
		{
			if (targetedPlayer != playerTeamsContainer.localPlayerId || !_targetToRocketOwnerAndLockStage.ContainsKey(targetedPlayer))
			{
				return;
			}
			Dictionary<int, int> dictionary = _targetToRocketOwnerAndLockStage[targetedPlayer];
			WeaponStatsData weaponStatsData = _weaponStatsDatas[itemDescriptorKey];
			if (weaponStatsData.lockTime > 0f)
			{
				_effectOwnersToLockStage.Clear();
				Dictionary<int, int>.Enumerator enumerator = dictionary.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int itemCategory = 0;
					int itemSize = 0;
					ItemDescriptorKey.GetItemInfoFromKey(_machineIdToCurrentWeaponKey[enumerator.Current.Key], out itemCategory, out itemSize);
					int key = ItemDescriptorKey.GenerateKey((ItemCategory)itemCategory, (ItemSize)itemSize);
					WeaponStatsData weaponStatsData2 = _weaponStatsDatas[key];
					if (weaponStatsData2.lockTime > 0f)
					{
						_effectOwnersToLockStage[enumerator.Current.Key] = enumerator.Current.Value;
					}
				}
				lockedOnEffectPresenter.ApplyLockOnEffectFromPlayer(dictionary);
			}
			if (dictionary.Count == 0 && _currentLockAlert == 0)
			{
				return;
			}
			int num = CurrentHighestLockStage(targetedPlayer);
			if (CurrentHighestLockStage(targetedPlayer) != _currentLockAlert)
			{
				this.OnOwnPlayerLockedOn(num);
				switch (num)
				{
				case 0:
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_OFF", 0);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_01_Loop", 1);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_02_Loop", 1);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Locked_Loop", 1);
					_currentLockAlert = 0;
					break;
				case 1:
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_OFF", 1);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_01_Loop", 0);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_02_Loop", 1);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Locked_Loop", 1);
					_currentLockAlert = 1;
					break;
				case 2:
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_OFF", 1);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_01_Loop", 1);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_02_Loop", 0);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Locked_Loop", 1);
					_currentLockAlert = 2;
					break;
				case 3:
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_OFF", 1);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_01_Loop", 1);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Lock_02_Loop", 1);
					EventManager.get_Instance().PostEvent("RocketLauncher_Player_Locked_Loop", 0);
					_currentLockAlert = 3;
					_currentLockTime = 0f;
					break;
				}
			}
		}

		private int CurrentHighestLockStage(int targetPlayer)
		{
			Dictionary<int, int> dictionary = _targetToRocketOwnerAndLockStage[targetPlayer];
			if (dictionary.Count == 0)
			{
				return 0;
			}
			int num = 0;
			Dictionary<int, int>.Enumerator enumerator = dictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int value = enumerator.Current.Value;
				if (value > num)
				{
					num = value;
				}
			}
			return num;
		}

		private int GetLockStage(int targetId, int rocketOwnerId)
		{
			if (!_targetToRocketOwnerAndLockStage.ContainsKey(targetId))
			{
				return 0;
			}
			Dictionary<int, int> dictionary = _targetToRocketOwnerAndLockStage[targetId];
			if (!dictionary.ContainsKey(rocketOwnerId))
			{
				return 0;
			}
			return dictionary[rocketOwnerId];
		}

		private void SetLockStage(int targetId, int rocketOwnerId, int lockStage)
		{
			Dictionary<int, int> dictionary;
			if (!_targetToRocketOwnerAndLockStage.ContainsKey(targetId))
			{
				dictionary = new Dictionary<int, int>();
				_targetToRocketOwnerAndLockStage[targetId] = dictionary;
			}
			else
			{
				dictionary = _targetToRocketOwnerAndLockStage[targetId];
			}
			if (lockStage == 0)
			{
				dictionary.Remove(rocketOwnerId);
			}
			else
			{
				dictionary[rocketOwnerId] = lockStage;
			}
		}
	}
}
