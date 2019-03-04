using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class KillAssistBonusEngine : IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		public const float KILL_ASSIST_BONUS_PERCENTAGE_THRESHOLD = 0.1f;

		private DestructionReporter _destructionReporter;

		private INetworkEventManagerClient _eventManagerClient;

		private HealthTracker _healthTracker;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public KillAssistBonusEngine(DestructionReporter destructionReporter, INetworkEventManagerClient eventManagerClient, HealthTracker healthTracker)
		{
			_eventManagerClient = eventManagerClient;
			_healthTracker = healthTracker;
			_destructionReporter = destructionReporter;
			_destructionReporter.OnPlayerDamageApplied += HandleOnPlayerDamageApplied;
		}

		public void Ready()
		{
		}

		public void OnFrameworkDestroyed()
		{
			_destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerDamageApplied;
		}

		private void HandleOnPlayerDamageApplied(DestructionData data)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			KillAssistAwarderEntityView killAssistAwarderEntityView = default(KillAssistAwarderEntityView);
			if (!entityViewsDB.TryQueryEntityView<KillAssistAwarderEntityView>(data.hitMachineId, ref killAssistAwarderEntityView))
			{
				return;
			}
			if (!data.isDestroyed)
			{
				int value = 0;
				if (!killAssistAwarderEntityView.damagedByComponent.damagedBy.TryGetValue(data.shooterId, out value))
				{
					killAssistAwarderEntityView.damagedByComponent.damagedBy[data.shooterId] = 0;
				}
				FasterListEnumerator<InstantiatedCube> enumerator = data.damagedCubes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						InstantiatedCube current = enumerator.get_Current();
						value += current.lastDamageApplied;
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				FasterListEnumerator<InstantiatedCube> enumerator2 = data.destroyedCubes.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						InstantiatedCube current2 = enumerator2.get_Current();
						value += current2.lastDamageApplied;
					}
				}
				finally
				{
					((IDisposable)enumerator2).Dispose();
				}
				killAssistAwarderEntityView.damagedByComponent.damagedBy[data.shooterId] = value;
			}
			else
			{
				int ownerId = killAssistAwarderEntityView.ownerComponent.ownerId;
				List<int> list = new List<int>();
				float num = _healthTracker.GetMaxHealth(TargetType.Player, ownerId);
				foreach (KeyValuePair<int, int> item in killAssistAwarderEntityView.damagedByComponent.damagedBy)
				{
					if (item.Key != data.shooterId && (float)item.Value / num > 0.1f)
					{
						list.Add(item.Key);
					}
				}
				if (list.Count > 0)
				{
					_eventManagerClient.SendEventToServer(NetworkEvent.AssistBonusRequest, new AssistBonusRequestDependency(ownerId, list));
				}
				killAssistAwarderEntityView.damagedByComponent.damagedBy.Clear();
				killAssistAwarderEntityView.aliveStateComponent.isAlive.NotifyOnValueSet((Action<int, bool>)OnAliveStateChanged);
			}
		}

		private void OnAliveStateChanged(int machineId, bool alive)
		{
			KillAssistAwarderEntityView killAssistAwarderEntityView = default(KillAssistAwarderEntityView);
			if (alive && entityViewsDB.TryQueryEntityView<KillAssistAwarderEntityView>(machineId, ref killAssistAwarderEntityView))
			{
				killAssistAwarderEntityView.damagedByComponent.damagedBy.Clear();
				killAssistAwarderEntityView.aliveStateComponent.isAlive.StopNotify((Action<int, bool>)OnAliveStateChanged);
			}
		}
	}
}
