using RCNetwork.Events;
using Simulation.NamedCollections;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class DefendTheBaseBonusManager : IInitialize
	{
		private const float SEND_PERIOD = 5f;

		private float _timeRemainingTillSend;

		private Dictionary<int, PlayersCubes> _playerDefendTheBaseBonuses = new Dictionary<int, PlayersCubes>();

		private HashSet<int> _playersThreateningBase = new HashSet<int>();

		[Inject]
		internal IDispatchWorldSwitching worldSwitch
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		[Inject]
		internal ForceFlushBonusObserver forceFlushBonusObserver
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			worldSwitch.OnWorldJustSwitched += HandleOnWorldJustSwitched;
			forceFlushBonusObserver.ForceFlush += SendAwardDefenseBonus;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
		}

		public void OnFrameworkDestroyed()
		{
			worldSwitch.OnWorldJustSwitched -= HandleOnWorldJustSwitched;
			forceFlushBonusObserver.ForceFlush -= SendAwardDefenseBonus;
		}

		private void HandleOnWorldJustSwitched(WorldSwitchMode obj)
		{
			worldSwitch.OnWorldIsSwitching.Add(OnPreSwitchWorld());
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				Tick(Time.get_deltaTime());
				yield return null;
			}
		}

		private void Tick(float deltaSec)
		{
			if (_playerDefendTheBaseBonuses.Count > 0)
			{
				_timeRemainingTillSend -= deltaSec;
				if (_timeRemainingTillSend <= 0f)
				{
					SendAwardDefenseBonus();
				}
			}
		}

		public void ResetAwardedBonuses(int playerId)
		{
		}

		public void EnteredThreateningEnemyBaseArea(int localPlayerId)
		{
			_playersThreateningBase.Add(localPlayerId);
		}

		public void ExitedThreateningBaseArea(int localPlayerId)
		{
			_playersThreateningBase.Remove(localPlayerId);
		}

		public void ProcessLocalCubesDestroyed(int shooterID, int hitPlayerId, FasterList<InstantiatedCube> destroyedCubes, bool machineDestroyed)
		{
			if (_playersThreateningBase.Contains(hitPlayerId))
			{
				CacheDestroyedCubes(destroyedCubes, shooterID, hitPlayerId);
			}
			if (machineDestroyed && _playerDefendTheBaseBonuses.Count > 0)
			{
				SendAwardDefenseBonus();
			}
		}

		private IEnumerator OnPreSwitchWorld()
		{
			if (_playerDefendTheBaseBonuses.Count > 0)
			{
				SendAwardDefenseBonus();
			}
			yield break;
		}

		private void SendAwardDefenseBonus()
		{
			if (_playerDefendTheBaseBonuses.Count > 0)
			{
				eventManagerClient.SendEventToServer(NetworkEvent.DefendTheBaseBonusRequest, new DestroyedHealedCubesBonusDependency(_playerDefendTheBaseBonuses));
			}
			_playerDefendTheBaseBonuses.Clear();
			_timeRemainingTillSend = 5f;
		}

		private void CacheDestroyedCubes(FasterList<InstantiatedCube> cubeInstances, int defenderPlayeId, int threateningPlayerId)
		{
			for (int i = 0; i < cubeInstances.get_Count(); i++)
			{
				if (!_playerDefendTheBaseBonuses.TryGetValue(defenderPlayeId, out PlayersCubes value))
				{
					value = new PlayersCubes();
					_playerDefendTheBaseBonuses.Add(defenderPlayeId, value);
				}
				if (!value.TryGetValue(threateningPlayerId, out CubeAmounts value2))
				{
					value2 = new CubeAmounts();
					value.Add(threateningPlayerId, value2);
				}
				InstantiatedCube instantiatedCube = cubeInstances.get_Item(i);
				uint iD = instantiatedCube.persistentCubeData.cubeType.ID;
				if (!value2.ContainsKey(iD))
				{
					value2.Add(iD, 0u);
				}
				CubeAmounts cubeAmounts;
				uint key;
				(cubeAmounts = value2)[key = iD] = cubeAmounts[key] + 1;
			}
		}
	}
}
