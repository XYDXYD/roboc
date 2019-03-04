using Achievements;
using Simulation.BattleArena.CapturePoint;
using Simulation.Hardware;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Simulation.Achievements
{
	internal class AchievementCapturePointsTrackerEngine : IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private CapturePointNotificationObserver _notificationObserver;

		private int _localMachineId = -1;

		[Inject]
		private IAchievementManager achievementManager
		{
			get;
			set;
		}

		[Inject]
		private PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		private PlayerMachinesContainer playerMachinesContainer
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe AchievementCapturePointsTrackerEngine(CapturePointNotificationObserver notificationObserver)
		{
			_notificationObserver = notificationObserver;
			_notificationObserver.AddAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_notificationObserver.RemoveAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void CheckLocalPlayerCapturedPoint(ref CapturePointNotificationDependency dependency)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			CaptureStateNode captureStateNode = default(CaptureStateNode);
			if (dependency.notification == CapturePointNotification.CaptureCompleted && playerTeamsContainer.IsMyTeam(dependency.attackingTeam) && entityViewsDB.TryQueryEntityView<CaptureStateNode>(dependency.id, ref captureStateNode))
			{
				if (_localMachineId == -1)
				{
					_localMachineId = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerTeamsContainer.localPlayerId);
				}
				MachineRigidbodyNode machineRigidbodyNode = default(MachineRigidbodyNode);
				if (entityViewsDB.TryQueryEntityView<MachineRigidbodyNode>(_localMachineId, ref machineRigidbodyNode) && GameUtility.IsPlayerInRange(machineRigidbodyNode.rigidbodyComponent.rb.get_worldCenterOfMass(), captureStateNode.rootComponent.root.get_transform().get_position(), captureStateNode.rangeComponent.squareRadius))
				{
					achievementManager.CapturedPoint();
				}
			}
		}

		public void Ready()
		{
		}
	}
}
