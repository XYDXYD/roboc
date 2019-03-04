using RCNetwork.Events;
using Simulation.NamedCollections;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class ProtectTeamMateBonusManager : IInitialize, IWaitForFrameworkDestruction
	{
		private class AttackedPlayerData
		{
			public int attackedPlayerId
			{
				get;
				set;
			}

			public DateTime timestamp
			{
				get;
				set;
			}
		}

		private const float PROTECT_TEAM_MATE_COOL_DOWN_TIME = 10f;

		private const float SEND_PERIOD = 5f;

		private float _timeRemainingTillSend;

		private Dictionary<int, AttackedPlayerData> _attackedPlayerdata = new Dictionary<int, AttackedPlayerData>();

		private Dictionary<int, PlayersCubes> _playerProtectTeamBonuses = new Dictionary<int, PlayersCubes>();

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

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

		private void HandleOnWorldJustSwitched(WorldSwitchMode obj)
		{
			worldSwitch.OnWorldIsSwitching.Add(OnPreSwitchWorld());
		}

		public void OnDependenciesInjected()
		{
			worldSwitch.OnWorldJustSwitched += HandleOnWorldJustSwitched;
			destructionReporter.OnPlayerDamageApplied += HandleOnPlayerTakesDamage;
			forceFlushBonusObserver.ForceFlush += SendAndClearPendingBonusesRequest;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
		}

		public void OnFrameworkDestroyed()
		{
			worldSwitch.OnWorldJustSwitched -= HandleOnWorldJustSwitched;
			destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerTakesDamage;
			forceFlushBonusObserver.ForceFlush -= SendAndClearPendingBonusesRequest;
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
			if (_playerProtectTeamBonuses.Count > 0)
			{
				_timeRemainingTillSend -= deltaSec;
				if (_timeRemainingTillSend <= 0f)
				{
					SendAndClearPendingBonusesRequest();
					_timeRemainingTillSend = 5f;
				}
			}
		}

		public void ResetAwardedBonuses(int playerId)
		{
			_attackedPlayerdata.Remove(playerId);
		}

		private void HandleOnPlayerTakesDamage(DestructionData data)
		{
			if (data.shooterIsLocal && data.shooterId != data.hitPlayerId)
			{
				if (!_attackedPlayerdata.TryGetValue(data.shooterId, out AttackedPlayerData value))
				{
					value = new AttackedPlayerData();
					_attackedPlayerdata.Add(data.shooterId, value);
				}
				value.attackedPlayerId = data.hitPlayerId;
				value.timestamp = DateTime.UtcNow;
			}
			AttackedPlayerData value2;
			if (_attackedPlayerdata.TryGetValue(data.hitPlayerId, out value2) && value2.attackedPlayerId != data.shooterId && (DateTime.UtcNow - value2.timestamp).TotalSeconds < 10.0)
			{
				CacheDestroyedCubes(data.destroyedCubes, data.shooterId, data.hitPlayerId);
			}
			if (data.isDestroyed)
			{
				SendAndClearPendingBonusesRequest();
			}
		}

		private IEnumerator OnPreSwitchWorld()
		{
			SendAndClearPendingBonusesRequest();
			yield break;
		}

		private void SendAndClearPendingBonusesRequest()
		{
			if (_playerProtectTeamBonuses.Count > 0)
			{
				eventManagerClient.SendEventToServer(NetworkEvent.ProtectTeamMateBonusRequest, new DestroyedHealedCubesBonusDependency(_playerProtectTeamBonuses));
				_playerProtectTeamBonuses.Clear();
			}
		}

		private void CacheDestroyedCubes(FasterList<InstantiatedCube> cubeInstances, int shooterId, int hitPlayerId)
		{
			for (int i = 0; i < cubeInstances.get_Count(); i++)
			{
				if (!_playerProtectTeamBonuses.TryGetValue(shooterId, out PlayersCubes value))
				{
					value = new PlayersCubes();
					_playerProtectTeamBonuses.Add(shooterId, value);
				}
				if (!value.TryGetValue(hitPlayerId, out CubeAmounts value2))
				{
					value2 = new CubeAmounts();
					value.Add(hitPlayerId, value2);
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
