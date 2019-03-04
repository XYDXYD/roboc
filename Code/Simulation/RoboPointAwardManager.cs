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
	internal sealed class RoboPointAwardManager : IInitialize, IWaitForFrameworkDestruction
	{
		private const float SENDING_PERIOD = 5f;

		private bool _timerEnabled;

		private float _timeRemainingTillSend;

		private Dictionary<int, PlayersCubes> _cachedDestroyedCubes = new Dictionary<int, PlayersCubes>();

		[Inject]
		internal INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		[Inject]
		public IDispatchWorldSwitching worldSwitch
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
			forceFlushBonusObserver.ForceFlush += SendAwardRequest;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
		}

		public void OnFrameworkDestroyed()
		{
			worldSwitch.OnWorldJustSwitched -= HandleOnWorldJustSwitched;
			forceFlushBonusObserver.ForceFlush -= SendAwardRequest;
		}

		public void ResetAwardedBonuses(int playerId)
		{
		}

		public void ProcessLocalPlayerCubesDestroyed(int shooterId, int targetPlayerId, FasterList<InstantiatedCube> destroyedCube, bool wasKilled)
		{
			CacheDestroyedCubes(shooterId, targetPlayerId, destroyedCube);
			if (wasKilled)
			{
				SendAwardRequest();
			}
		}

		private void CacheDestroyedCubes(int shooterId, int targetPlayerId, FasterList<InstantiatedCube> destroyedCubes)
		{
			for (int i = 0; i < destroyedCubes.get_Count(); i++)
			{
				if (!_cachedDestroyedCubes.TryGetValue(shooterId, out PlayersCubes value))
				{
					value = new PlayersCubes();
					_cachedDestroyedCubes.Add(shooterId, value);
				}
				if (!value.TryGetValue(targetPlayerId, out CubeAmounts value2))
				{
					value2 = new CubeAmounts();
					value.Add(targetPlayerId, value2);
				}
				InstantiatedCube instantiatedCube = destroyedCubes.get_Item(i);
				uint iD = instantiatedCube.persistentCubeData.cubeType.ID;
				if (!value2.ContainsKey(iD))
				{
					value2.Add(iD, 0u);
				}
				CubeAmounts cubeAmounts;
				uint key;
				(cubeAmounts = value2)[key = iD] = cubeAmounts[key] + 1;
				_timerEnabled = true;
			}
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				Tick(Time.get_deltaTime());
				yield return null;
			}
		}

		private void Tick(float deltaTime)
		{
			if (_timerEnabled)
			{
				_timeRemainingTillSend += deltaTime;
				if (_timeRemainingTillSend >= 5f)
				{
					SendAwardRequest();
				}
			}
		}

		public void SendAwardRequest()
		{
			_timerEnabled = false;
			_timeRemainingTillSend = 0f;
			if (_cachedDestroyedCubes.Count > 0)
			{
				eventManagerClient.SendEventToServer(NetworkEvent.DestroyCubesBonusRequest, new DestroyedHealedCubesBonusDependency(_cachedDestroyedCubes));
				_cachedDestroyedCubes.Clear();
			}
		}

		private void HandleOnWorldJustSwitched(WorldSwitchMode obj)
		{
			worldSwitch.OnWorldIsSwitching.Add(OnPreSwitchWorld());
		}

		private IEnumerator OnPreSwitchWorld()
		{
			SendAwardRequest();
			yield break;
		}
	}
}
