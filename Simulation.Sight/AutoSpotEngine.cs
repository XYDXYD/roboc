using RCNetwork.Events;
using Simulation.Hardware.Modules;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Sight
{
	internal sealed class AutoSpotEngine : SingleEntityViewEngine<SpottableMachineEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private const float UPDATE_INTERVAL_SECONDS = 1f;

		private int _remoteSpottables;

		private SpotStateObservable _localSpotStateObservable;

		private INetworkEventManagerClient _networkEventManager;

		private bool _anyFriendlyRadarActive;

		private int _localHumanTeamId = -1;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe AutoSpotEngine(INetworkEventManagerClient networkEventManager, RemoteEnemySpottedObserver remoteEnemySpottedObserver, SpotStateObservable localSpotStateObservable, TeamRadarObserver teamRadarObserver)
		{
			remoteEnemySpottedObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			teamRadarObserver.AddAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_localSpotStateObservable = localSpotStateObservable;
			_networkEventManager = networkEventManager;
		}

		public void Ready()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)TickTask);
		}

		protected override void Add(SpottableMachineEntityView spottable)
		{
			spottable.aliveComponent.isAlive.NotifyOnValueSet((Action<int, bool>)OnAliveStateChange);
			if (!spottable.sourceComponent.isLocal)
			{
				_remoteSpottables++;
			}
			if (spottable.ownerComponent.ownedByMe)
			{
				_localHumanTeamId = spottable.teamComponent.ownerTeamId;
			}
		}

		protected override void Remove(SpottableMachineEntityView spottable)
		{
			spottable.aliveComponent.isAlive.StopNotify((Action<int, bool>)OnAliveStateChange);
			if (!spottable.sourceComponent.isLocal)
			{
				_remoteSpottables--;
			}
		}

		private void OnAliveStateChange(int machineId, bool isAlive)
		{
			if (!isAlive)
			{
				SpottableMachineEntityView spottableMachineEntityView = entityViewsDB.QueryEntityView<SpottableMachineEntityView>(machineId);
				if (spottableMachineEntityView.spottableComponent.isSpotted.get_value())
				{
					EndSpotLocally(spottableMachineEntityView);
				}
			}
		}

		private IEnumerator TickTask()
		{
			while (true)
			{
				Tick();
				yield return null;
			}
		}

		public void Tick()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<SpottableMachineEntityView> val = entityViewsDB.QueryEntityViews<SpottableMachineEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				SpottableMachineEntityView spottableMachineEntityView = val.get_Item(i);
				if (GetCurrentTime() - spottableMachineEntityView.spottableComponent.spotLastTimeUpdated < 1f)
				{
					break;
				}
				spottableMachineEntityView.spottableComponent.spotLastTimeUpdated = GetCurrentTime();
				if (spottableMachineEntityView.aliveComponent.isAlive.get_value())
				{
					bool value = spottableMachineEntityView.spottableComponent.isSpotted.get_value();
					bool flag = UpdateRadarSpotting(spottableMachineEntityView) || UpdateMachineSpotting(spottableMachineEntityView) || UpdateStructureSpotting(spottableMachineEntityView);
					if (value && !flag)
					{
						EndSpotLocally(spottableMachineEntityView);
					}
				}
			}
		}

		private void OnTeamRadarChange(ref bool active)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (_anyFriendlyRadarActive == active)
			{
				return;
			}
			_anyFriendlyRadarActive = active;
			if (!active)
			{
				return;
			}
			FasterReadOnlyList<SpottableMachineEntityView> val = entityViewsDB.QueryEntityViews<SpottableMachineEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				SpottableMachineEntityView spottableMachineEntityView = val.get_Item(i);
				if (spottableMachineEntityView.teamComponent.ownerTeamId != _localHumanTeamId && spottableMachineEntityView.visibilityComponent.isVisible && spottableMachineEntityView.aliveComponent.isAlive.get_value())
				{
					SpotLocally(spottableMachineEntityView);
				}
			}
		}

		private bool UpdateRadarSpotting(SpottableMachineEntityView spottable)
		{
			if (_anyFriendlyRadarActive && spottable.visibilityComponent.isVisible && spottable.teamComponent.ownerTeamId != _localHumanTeamId)
			{
				SpotLocally(spottable);
				return true;
			}
			return false;
		}

		private bool UpdateMachineSpotting(SpottableMachineEntityView spottable)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<SpotterMachineEntityView> enumerator = entityViewsDB.QueryEntityViews<SpotterMachineEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SpotterMachineEntityView current = enumerator.get_Current();
					if (!current.ownerComponent.ownedByAi && current.teamComponent.ownerTeamId != spottable.teamComponent.ownerTeamId && TestSpottedByMachine(current, spottable))
					{
						Spot(spottable);
						return true;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return false;
		}

		private bool UpdateStructureSpotting(SpottableMachineEntityView spottable)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<SpotterStructureEntityView> val = entityViewsDB.QueryEntityViews<SpotterStructureEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				SpotterStructureEntityView spotterStructureEntityView = val.get_Item(i);
				if (spotterStructureEntityView.ownerTeamComponent.ownerTeamId != -1 && spotterStructureEntityView.ownerTeamComponent.ownerTeamId != spottable.teamComponent.ownerTeamId && TestSpottedByStructure(spotterStructureEntityView, spottable))
				{
					SelfSpot(spottable);
					return true;
				}
			}
			return false;
		}

		private static bool TestSpottedByMachine(SpotterMachineEntityView spotter, SpottableMachineEntityView spottable)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			if (spotter.ownerComponent.ownerId == spottable.ownerComponent.ownerId)
			{
				return false;
			}
			if (!spottable.visibilityComponent.isVisible)
			{
				return false;
			}
			Vector3 position = spotter.frustrumComponent.attachedCamera.get_transform().get_position();
			Vector3 machineCenter = GetMachineCenter(spottable);
			if (Vector3.Distance(position, machineCenter) > spotter.spotterComponent.spotRange)
			{
				return false;
			}
			if (!IsInFrustum(spotter, machineCenter))
			{
				return false;
			}
			if (IsCovered(position, spotter.spotterComponent.innerSpotRange, spottable))
			{
				return false;
			}
			return true;
		}

		private static bool TestSpottedByStructure(SpotterStructureEntityView spotter, SpottableMachineEntityView spottable)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (!spottable.sourceComponent.isLocal)
			{
				return false;
			}
			if (!spottable.visibilityComponent.isVisible)
			{
				return false;
			}
			Vector3 spotPositionWorld = spotter.spotterComponent.spotPositionWorld;
			Vector3 machineCenter = GetMachineCenter(spottable);
			if (Vector3.Distance(spotPositionWorld, machineCenter) > spotter.spotterComponent.spotRange)
			{
				return false;
			}
			if (IsCovered(spotPositionWorld, spotter.spotterComponent.innerSpotRange, spottable))
			{
				return false;
			}
			return true;
		}

		private static bool IsInFrustum(SpotterMachineEntityView spotter, Vector3 pos)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Camera attachedCamera = spotter.frustrumComponent.attachedCamera;
			Vector3 val = attachedCamera.WorldToViewportPoint(pos);
			return val.x >= 0f && val.x < 1f && val.y >= 0f && val.y < 1f && val.z > 0f;
		}

		private static bool IsCovered(Vector3 eyePos, float innerRadius, SpottableMachineEntityView spottable)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			Vector3 machineCenter = GetMachineCenter(spottable);
			float num = Vector3.Distance(eyePos, machineCenter);
			if (num < Mathf.Max(innerRadius, 0.01f))
			{
				return false;
			}
			eyePos += innerRadius * (machineCenter - eyePos) / num;
			RaycastHit val = default(RaycastHit);
			if (Physics.Linecast(eyePos, machineCenter, ref val, GameLayers.SPOT_LAYER_MASK & ~GameLayers.ENEMY_PLAYERS_LAYER_MASK))
			{
				return true;
			}
			return false;
		}

		private void Spot(SpottableMachineEntityView spottable)
		{
			if (_remoteSpottables > 0)
			{
				_networkEventManager.SendEventToServer(NetworkEvent.EnemySpotted, new PlayerIdDependency(spottable.ownerComponent.ownerId));
			}
			SpotLocally(spottable);
		}

		private void SpotLocally(SpottableMachineEntityView spottable)
		{
			if (!spottable.spottableComponent.isSpotted.get_value())
			{
				spottable.spottableComponent.isSpotted.set_value(true);
				DispatchLocalSpotChange(spottable);
			}
		}

		private void SelfSpot(SpottableMachineEntityView spottable)
		{
			if (_remoteSpottables > 0)
			{
				_networkEventManager.SendEventToServer(NetworkEvent.SpottedByStructure, new PlayerIdDependency(spottable.ownerComponent.ownerId));
			}
			SpotLocally(spottable);
		}

		private void RemoteEnemySpottedReceived(ref int owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<SpottableMachineEntityView> val = entityViewsDB.QueryEntityViews<SpottableMachineEntityView>();
			int num = 0;
			SpottableMachineEntityView spottableMachineEntityView;
			while (true)
			{
				if (num < val.get_Count())
				{
					spottableMachineEntityView = val.get_Item(num);
					if (spottableMachineEntityView.ownerComponent.ownerId == owner)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			spottableMachineEntityView.spottableComponent.spotLastTimeUpdated = GetCurrentTime();
			SpotLocally(spottableMachineEntityView);
		}

		private void EndSpotLocally(SpottableMachineEntityView spottable)
		{
			spottable.spottableComponent.isSpotted.set_value(false);
			DispatchLocalSpotChange(spottable);
		}

		private void DispatchLocalSpotChange(SpottableMachineEntityView spottable)
		{
			SpotStateChangeArgs spotStateChangeArgs = new SpotStateChangeArgs(spottable.ownerComponent.ownerId, spottable.spottableComponent.isSpotted.get_value());
			_localSpotStateObservable.Dispatch(ref spotStateChangeArgs);
		}

		private static float GetCurrentTime()
		{
			return Time.get_realtimeSinceStartup();
		}

		private static Vector3 GetMachineCenter(SpottableMachineEntityView machine)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return machine.rigidbodyComponent.rb.get_worldCenterOfMass();
		}
	}
}
