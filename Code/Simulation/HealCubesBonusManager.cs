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
	internal sealed class HealCubesBonusManager : IInitialize, IWaitForFrameworkDestruction
	{
		private Dictionary<int, PlayersCubes> _cachedHealedCubes = new Dictionary<int, PlayersCubes>();

		private float _timer = -1f;

		private const float SENDING_PERIOD = 5f;

		[Inject]
		public IDispatchWorldSwitching worldSwitch
		{
			private get;
			set;
		}

		[Inject]
		public INetworkEventManagerClient eventManagerClient
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

		private void HandleOnWorldJustSwitched(WorldSwitchMode obj)
		{
			worldSwitch.OnWorldIsSwitching.Add(OnPreSwitchWorld());
		}

		public void ProcessLocalPlayerCubesHealed(int shooterId, int targetPlayerId, FasterList<InstantiatedCube> healedCubes)
		{
			for (int i = 0; i < healedCubes.get_Count(); i++)
			{
				if (!_cachedHealedCubes.TryGetValue(shooterId, out PlayersCubes value))
				{
					value = new PlayersCubes();
					_cachedHealedCubes.Add(shooterId, value);
				}
				if (!value.TryGetValue(targetPlayerId, out CubeAmounts value2))
				{
					value2 = new CubeAmounts();
					value.Add(targetPlayerId, value2);
				}
				InstantiatedCube instantiatedCube = healedCubes.get_Item(i);
				uint iD = instantiatedCube.persistentCubeData.cubeType.ID;
				if (!value2.ContainsKey(iD))
				{
					value2.Add(iD, 0u);
				}
				CubeAmounts cubeAmounts;
				uint key;
				(cubeAmounts = value2)[key = iD] = cubeAmounts[key] + 1;
			}
			if (_timer < 0f)
			{
				_timer = 5f;
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

		private void Tick(float deltaSec)
		{
			if (_timer >= 0f)
			{
				_timer -= deltaSec;
				if (_timer <= 0f)
				{
					SendAwardRequest();
				}
			}
		}

		private IEnumerator OnPreSwitchWorld()
		{
			SendAwardRequest();
			yield break;
		}

		private void SendAwardRequest()
		{
			if (_cachedHealedCubes.Count > 0)
			{
				eventManagerClient.SendEventToServer(NetworkEvent.HealCubesBonusRequest, new DestroyedHealedCubesBonusDependency(_cachedHealedCubes));
				_cachedHealedCubes.Clear();
			}
		}
	}
}
