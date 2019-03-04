using RCNetwork.Events;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal class RadarEngine : SingleEntityViewEngine<RadarModuleEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private PlayerIdDependency _playerIdDependency;

		private TeamRadarObservable _teamRadarObservable;

		private INetworkEventManagerClient _networkEventManager;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe RadarEngine(INetworkEventManagerClient networkEventManager, RemoteRadarActivationObserver remoteRadarActivationObserver, TeamRadarObservable teamRadarObservable)
		{
			remoteRadarActivationObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_teamRadarObservable = teamRadarObservable;
			_networkEventManager = networkEventManager;
		}

		public void Ready()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)TickTask);
		}

		private IEnumerator TickTask()
		{
			while (true)
			{
				Tick(Time.get_deltaTime());
				yield return null;
			}
		}

		private void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<RadarModuleEntityView> val = entityViewsDB.QueryEntityViews<RadarModuleEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				RadarModuleEntityView radarModuleEntityView = val.get_Item(i);
				if (radarModuleEntityView.radarComponent.isRadarActive.get_value())
				{
					radarModuleEntityView.radarComponent.radarRemainingTime -= deltaSec;
					if (radarModuleEntityView.radarComponent.radarRemainingTime <= 0f)
					{
						DeactivateRadarLocally(radarModuleEntityView);
					}
				}
			}
		}

		protected override void Add(RadarModuleEntityView radar)
		{
			if (radar.ownerComponent.ownedByMe)
			{
				_playerIdDependency = new PlayerIdDependency(radar.ownerComponent.ownerId);
				radar.activationComponent.activate.subscribers += OnRadarActivatedByLocalPlayer;
			}
		}

		protected override void Remove(RadarModuleEntityView radar)
		{
			if (radar.ownerComponent.ownedByMe)
			{
				radar.activationComponent.activate.subscribers -= OnRadarActivatedByLocalPlayer;
			}
		}

		private void OnRadarActivatedByLocalPlayer(IModuleActivationComponent cmp, int moduleId)
		{
			RadarModuleEntityView radarModuleEntityView = default(RadarModuleEntityView);
			if (entityViewsDB.TryQueryEntityView<RadarModuleEntityView>(moduleId, ref radarModuleEntityView) && !radarModuleEntityView.radarComponent.isRadarActive.get_value())
			{
				_networkEventManager.SendEventToServer(NetworkEvent.RadarModuleActivated, _playerIdDependency);
				ActivateRadarLocally(radarModuleEntityView);
				radarModuleEntityView.confirmActivationComponent.activationConfirmed.Dispatch(ref moduleId);
			}
		}

		private void OnRemoteRadarActivated(ref int playerId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<RadarModuleEntityView> val = entityViewsDB.QueryEntityViews<RadarModuleEntityView>();
			int num = 0;
			RadarModuleEntityView radarModuleEntityView;
			while (true)
			{
				if (num < val.get_Count())
				{
					radarModuleEntityView = val.get_Item(num);
					if (radarModuleEntityView.ownerComponent.ownerId == playerId)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			ActivateRadarLocally(radarModuleEntityView);
		}

		private void ActivateRadarLocally(RadarModuleEntityView radar)
		{
			radar.radarComponent.isRadarActive.set_value(true);
			radar.radarComponent.radarRemainingTime = radar.radarStatsComponent.radarDuration;
			if (!radar.ownerComponent.isEnemy)
			{
				bool flag = true;
				_teamRadarObservable.Dispatch(ref flag);
			}
		}

		private void DeactivateRadarLocally(RadarModuleEntityView radar)
		{
			radar.radarComponent.isRadarActive.set_value(false);
			if (!IsAnyRadarActiveInTeam())
			{
				bool flag = false;
				_teamRadarObservable.Dispatch(ref flag);
			}
		}

		private bool IsAnyRadarActiveInTeam()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<RadarModuleEntityView> val = entityViewsDB.QueryEntityViews<RadarModuleEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				RadarModuleEntityView radarModuleEntityView = val.get_Item(i);
				if (!radarModuleEntityView.ownerComponent.isEnemy && radarModuleEntityView.radarComponent.isRadarActive.get_value())
				{
					return true;
				}
			}
			return false;
		}
	}
}
