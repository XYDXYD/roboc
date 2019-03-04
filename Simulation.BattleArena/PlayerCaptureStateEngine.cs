using Simulation.BattleArena.CapturePoint;
using Simulation.BattleArena.GUI;
using Simulation.Hardware;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Simulation.BattleArena
{
	internal sealed class PlayerCaptureStateEngine : SingleEntityViewEngine<MachineRigidbodyNode>, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private CapturePointNotificationObserver _notificationObserver;

		private int _localMachineId = -1;

		private ITaskRoutine _task;

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerCapureStatePresenter presenter
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe PlayerCaptureStateEngine(CapturePointNotificationObserver _notificationObserver)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			this._notificationObserver = _notificationObserver;
			_notificationObserver.AddAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator((IEnumerator)new LoopActionEnumerator((Action)Update));
		}

		public void Ready()
		{
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_notificationObserver.RemoveAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(MachineRigidbodyNode entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				_localMachineId = entityView.get_ID();
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(MachineRigidbodyNode entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				_localMachineId = -1;
				_task.Stop();
			}
		}

		private void Update()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<CaptureStateNode> val = entityViewsDB.QueryEntityViews<CaptureStateNode>();
			MachineRigidbodyNode machineRigidbodyNode = default(MachineRigidbodyNode);
			if (entityViewsDB.TryQueryEntityView<MachineRigidbodyNode>(_localMachineId, ref machineRigidbodyNode))
			{
				for (int i = 0; i < val.get_Count(); i++)
				{
					CaptureStateNode captureStateNode = val.get_Item(i);
					bool flag = GameUtility.IsPlayerInRange(machineRigidbodyNode.rigidbodyComponent.rb.get_worldCenterOfMass(), captureStateNode.rootComponent.root.get_transform().get_position(), captureStateNode.rangeComponent.squareRadius);
					if (captureStateNode.stateComponent.state != 0 && flag && entityViewsDB.QueryEntityView<MachineInvisibilityNode>(_localMachineId).machineVisibilityComponent.isVisible)
					{
						presenter.ShowView(show: true, captureStateNode.stateComponent.state, captureStateNode.get_ID());
						return;
					}
				}
			}
			presenter.ShowView(show: false, CaptureState.none, -1);
		}

		private void HandleOnCapturePointNotificationReceived(ref CapturePointNotificationDependency parameter)
		{
			CaptureStateNode captureStateNode = null;
			entityViewsDB.TryQueryEntityView<CaptureStateNode>(parameter.id, ref captureStateNode);
			if (captureStateNode == null)
			{
				if (parameter.notification != CapturePointNotification.Spawn)
				{
					Console.LogError("HandleOnCapturePointNotificationReceived " + parameter.notification + " but node isn't registered yet");
				}
			}
			else if (parameter.notification == CapturePointNotification.CaptureStarted)
			{
				captureStateNode.stateComponent.state = (playerTeamsContainer.IsMyTeam(parameter.attackingTeam) ? CaptureState.capturing : CaptureState.none);
			}
			else if (parameter.notification == CapturePointNotification.CaptureStoppedNoAttackers || parameter.notification == CapturePointNotification.CaptureUnlocked)
			{
				captureStateNode.stateComponent.state = CaptureState.none;
			}
			else if (parameter.notification == CapturePointNotification.CaptureStoppedByDefenders || parameter.notification == CapturePointNotification.CaptureLocked)
			{
				captureStateNode.stateComponent.state = ((!playerTeamsContainer.IsMyTeam(parameter.defendingTeam)) ? CaptureState.contested : CaptureState.contesting);
			}
			else if (parameter.notification == CapturePointNotification.CaptureCompleted)
			{
				captureStateNode.visualTeamComponent.visualTeam = ((!playerTeamsContainer.IsMyTeam(parameter.attackingTeam)) ? VisualTeam.EnemyTeam : VisualTeam.MyTeam);
				captureStateNode.stateComponent.state = CaptureState.none;
			}
		}
	}
}
