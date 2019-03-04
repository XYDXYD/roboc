using LobbyServiceLayer;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Simulation
{
	internal class TLOG_PlayerKillTrackerDataCacheEngine_Tencent : MultiEntityViewsEngine<CurrentWaveTrackerEntityView, TLOG_PlayerKillTrackerDataCacheEntityView_Tencent>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ILobbyRequestFactory _lobbyRequestFactory;

		private readonly IServiceRequestFactory _serviceRequestFactory;

		private readonly MachinePreloader _machinePreloader;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public TLOG_PlayerKillTrackerDataCacheEngine_Tencent(ILobbyRequestFactory lobbyRequestFactory, IServiceRequestFactory serviceRequestFactory, MachinePreloader machinePreloader)
		{
			_lobbyRequestFactory = lobbyRequestFactory;
			_serviceRequestFactory = serviceRequestFactory;
			_machinePreloader = machinePreloader;
		}

		public void Ready()
		{
		}

		protected override void Add(CurrentWaveTrackerEntityView entityView)
		{
			entityView.ReadyToSpawnWaveComponent.ReadyToSpawn.NotifyOnValueSet((Action<int, bool>)HandleNewWave);
		}

		protected override void Remove(CurrentWaveTrackerEntityView entityView)
		{
			entityView.ReadyToSpawnWaveComponent.ReadyToSpawn.NotifyOnValueSet((Action<int, bool>)HandleNewWave);
		}

		protected override void Add(TLOG_PlayerKillTrackerDataCacheEntityView_Tencent entityView)
		{
			if (WorldSwitching.GetGameModeType() != GameModeType.Campaign)
			{
				RetrieveExpectedPlayers();
			}
		}

		protected override void Remove(TLOG_PlayerKillTrackerDataCacheEntityView_Tencent entityView)
		{
		}

		private void HandleNewWave(int entityID, bool readyToSpawn)
		{
			if (readyToSpawn)
			{
				RetrieveExpectedPlayers();
			}
		}

		private void RetrieveExpectedPlayers()
		{
			_lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>().SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>>(CachePlayer)).Execute();
		}

		private void CachePlayer(ReadOnlyDictionary<string, PlayerDataDependency> players)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			entityViewsDB.QueryEntityViews<TLOG_PlayerKillTrackerDataCacheEntityView_Tencent>().get_Item(0).playerKillTrackerDataCacheComponent.playerDataCache = players;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)CacheTiers);
		}

		private IEnumerator CacheTiers()
		{
			FasterReadOnlyList<TLOG_PlayerKillTrackerDataCacheEntityView_Tencent> playerKillTrackerDataCacheEntityViews = entityViewsDB.QueryEntityViews<TLOG_PlayerKillTrackerDataCacheEntityView_Tencent>();
			ReadOnlyDictionary<string, PlayerDataDependency> playerDataCache = playerKillTrackerDataCacheEntityViews.get_Item(0).playerKillTrackerDataCacheComponent.playerDataCache;
			Dictionary<string, uint> playerTiersCache = new Dictionary<string, uint>();
			TaskService<TiersData> loadTiersBandingReq = _serviceRequestFactory.Create<ILoadTiersBandingRequest>().AsTask();
			yield return loadTiersBandingReq;
			if (loadTiersBandingReq.succeeded)
			{
				TiersData tiersData = loadTiersBandingReq.result;
				TaskService<CPUSettingsDependency> cpuRequest = _serviceRequestFactory.Create<ILoadCpuSettingsRequest>().AsTask();
				yield return cpuRequest;
				if (cpuRequest.succeeded)
				{
					uint maxCpuPower = cpuRequest.result.maxRegularCpu;
					while (!_machinePreloader.IsComplete)
					{
						yield return null;
					}
					DictionaryEnumerator<string, PlayerDataDependency> enumerator = playerDataCache.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, PlayerDataDependency> current = enumerator.get_Current();
							PreloadedMachine preloadedMachine = _machinePreloader.GetPreloadedMachine(current.Value.PlayerName);
							FasterList<InstantiatedCube> allInstantiatedCubes = preloadedMachine.machineMap.GetAllInstantiatedCubes();
							int num = 0;
							uint num2 = 0u;
							for (int i = 0; i < allInstantiatedCubes.get_Count(); i++)
							{
								int cpuRating = (int)allInstantiatedCubes.get_Item(i).persistentCubeData.cpuRating;
								int cubeRanking = allInstantiatedCubes.get_Item(i).persistentCubeData.cubeRanking;
								if (allInstantiatedCubes.get_Item(i).persistentCubeData.itemType == ItemType.Cosmetic)
								{
								}
								num += cpuRating;
								num2 = (uint)((int)num2 + cubeRanking);
							}
							bool isMegabot = num > maxCpuPower;
							uint value = RRAndTiers.ConvertRRToTierIndex(num2, isMegabot, tiersData);
							playerTiersCache.Add(current.Value.PlayerName, value);
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					playerKillTrackerDataCacheEntityViews.get_Item(0).playerKillTrackerDataCacheComponent.playerTierCache = playerTiersCache;
				}
				else
				{
					Console.LogError("Error loading cpu. " + cpuRequest.behaviour.exceptionThrown);
				}
			}
			else
			{
				Console.LogError("Error loading tiers. " + loadTiersBandingReq.behaviour.exceptionThrown);
			}
		}
	}
}
