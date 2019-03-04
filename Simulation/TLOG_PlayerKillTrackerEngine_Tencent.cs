using Battle;
using LobbyServiceLayer;
using Services.Analytics;
using Simulation.Hardware;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class TLOG_PlayerKillTrackerEngine_Tencent : IEngine, IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		private DestructionReporter destructionReporter
		{
			get;
			set;
		}

		[Inject]
		private PlayerNamesContainer playerNamesContainer
		{
			get;
			set;
		}

		[Inject]
		private HealthTracker healthTracker
		{
			get;
			set;
		}

		[Inject]
		private IServiceRequestFactory serviceRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private ILobbyRequestFactory lobbyRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private IAnalyticsRequestFactory analyticsRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private BattlePlayers battlePlayers
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingPresenter
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		public void OnDependenciesInjected()
		{
			destructionReporter.OnPlayerDamageApplied += CheckPlayerDamage;
		}

		public void OnFrameworkDestroyed()
		{
			destructionReporter.OnPlayerDamageApplied -= CheckPlayerDamage;
		}

		private void CheckPlayerDamage(DestructionData data)
		{
			if (data.shooterIsMe && data.isDestroyed)
			{
				TaskRunner.get_Instance().Run(SendPlayerKillRequest(data));
			}
		}

		private IEnumerator SendPlayerKillRequest(DestructionData data)
		{
			loadingPresenter.NotifyLoading("HandleAnalytics");
			int killerPlayerId = data.shooterId;
			int victimPlayerId = data.hitPlayerId;
			LogPlayerKillDependency dependency = new LogPlayerKillDependency();
			dependency.killerIsAIBot = false;
			dependency.victimIsAIBot = false;
			dependency.killerUsername = playerNamesContainer.GetPlayerName(killerPlayerId);
			dependency.victimUsername = playerNamesContainer.GetPlayerName(victimPlayerId);
			dependency.killerPosition = GetPlayerPosition(killerPlayerId);
			dependency.victimPosition = GetPlayerPosition(victimPlayerId);
			dependency.distance = Vector3.Distance(dependency.killerPosition, dependency.victimPosition);
			dependency.killerHealth = healthTracker.GetCurrentHealth(TargetType.Player, killerPlayerId);
			dependency.lastDamage = 0;
			for (int i = 0; i < data.destroyedCubes.get_Count(); i++)
			{
				dependency.lastDamage += data.destroyedCubes.get_Item(i).lastDamageApplied;
			}
			for (int j = 0; j < data.damagedCubes.get_Count(); j++)
			{
				dependency.lastDamage += data.damagedCubes.get_Item(j).lastDamageApplied;
			}
			FasterReadOnlyList<TLOG_PlayerKillTrackerDataCacheEntityView_Tencent> playerKillTrackerDataCacheEntityViews = entityViewsDB.QueryEntityViews<TLOG_PlayerKillTrackerDataCacheEntityView_Tencent>();
			ReadOnlyDictionary<string, PlayerDataDependency> playerDataCache = playerKillTrackerDataCacheEntityViews.get_Item(0).playerKillTrackerDataCacheComponent.playerDataCache;
			Dictionary<string, uint> playerTierCache = playerKillTrackerDataCacheEntityViews.get_Item(0).playerKillTrackerDataCacheComponent.playerTierCache;
			PlayerDataDependency killerPlayerData = playerDataCache.get_Item(dependency.killerUsername);
			dependency.killerRobotUniqueId = killerPlayerData.RobotUniqueId;
			dependency.killerRobotName = killerPlayerData.RobotName;
			dependency.killerTier = (int)(playerTierCache[dependency.killerUsername] + 1);
			dependency.victimTier = (int)(playerTierCache[dependency.victimUsername] + 1);
			ILogPlayerKillRequest service = analyticsRequestFactory.Create<ILogPlayerKillRequest, LogPlayerKillDependency>(dependency);
			TaskService task = new TaskService(service);
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Player Kill request failed to send " + task.behaviour.errorBody);
			}
			loadingPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private Vector3 GetPlayerPosition(int playerId)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			MachineRigidbodyNode[] array = entityViewsDB.QueryEntityViewsAsArray<MachineRigidbodyNode>(ref num);
			for (int i = 0; i < num; i++)
			{
				if (array[i].ownerComponent.ownerId == playerId)
				{
					return array[i].rigidbodyComponent.rb.get_centerOfMass();
				}
			}
			Console.LogError("Failed to retrieve kill player position for TLOG");
			return Vector3.get_zero();
		}
	}
}
