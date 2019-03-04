using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using System;

namespace Simulation
{
	internal sealed class HealAssistBonusEngine : IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private HealthTracker _healthTracker;

		private INetworkEventManagerClient _networkEventManager;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public HealAssistBonusEngine(HealthTracker healthTracker, INetworkEventManagerClient networkEventManager)
		{
			_healthTracker = healthTracker;
			_healthTracker.OnPlayerHealthChanged += HandleOnPlayerHealthChanged;
			_networkEventManager = networkEventManager;
		}

		public void Ready()
		{
		}

		public void OnFrameworkDestroyed()
		{
			_healthTracker.OnPlayerHealthChanged -= HandleOnPlayerHealthChanged;
		}

		private void HandleOnPlayerHealthChanged(HealthTracker.PlayerHealthChangedInfo info)
		{
			HealAssistAwarderEntityView healAssistAwarderEntityView = default(HealAssistAwarderEntityView);
			if (info.deltaHealth > 0 && info.shooterTargetType == TargetType.Player && info.shotPlayerId != info.shooterId && entityViewsDB.TryQueryEntityView<HealAssistAwarderEntityView>(info.shotMachineId, ref healAssistAwarderEntityView) && !(_healthTracker.GetCurrentHealthPercent(TargetType.Player, info.shotMachineId) < 1f) && !healAssistAwarderEntityView.healAssistComponent.rewardedPlayers.Contains(info.shooterId))
			{
				healAssistAwarderEntityView.healAssistComponent.rewardedPlayers.Add(info.shooterId);
				_networkEventManager.SendEventToServer(NetworkEvent.HeallingAssistBonusRequest, new HealingAssistBonusRequestDependency(info.shooterId, info.shotPlayerId));
				if (healAssistAwarderEntityView.healAssistComponent.rewardedPlayers.Count == 1)
				{
					healAssistAwarderEntityView.aliveStateComponent.isAlive.NotifyOnValueSet((Action<int, bool>)OnAliveStateChanged);
				}
			}
		}

		private void OnAliveStateChanged(int machineId, bool alive)
		{
			HealAssistAwarderEntityView healAssistAwarderEntityView = default(HealAssistAwarderEntityView);
			if (alive && entityViewsDB.TryQueryEntityView<HealAssistAwarderEntityView>(machineId, ref healAssistAwarderEntityView))
			{
				healAssistAwarderEntityView.healAssistComponent.rewardedPlayers.Clear();
				healAssistAwarderEntityView.aliveStateComponent.isAlive.StopNotify((Action<int, bool>)OnAliveStateChanged);
			}
		}
	}
}
