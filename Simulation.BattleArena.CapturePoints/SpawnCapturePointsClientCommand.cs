using Simulation.BattleArena.CapturePoint;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using UnityEngine;

namespace Simulation.BattleArena.CapturePoints
{
	internal sealed class SpawnCapturePointsClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GetCapturePointsDependency _dependency;

		[Inject]
		public MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public IMinimapPresenter minimapPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal CapturePointNotificationObservable notificationObservable
		{
			private get;
			set;
		}

		[Inject]
		private CapturePointProgressObservable progressObservable
		{
			get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as GetCapturePointsDependency);
			return this;
		}

		public void Execute()
		{
			RegisterCapturePoint(CapturePointId.one);
			RegisterCapturePoint(CapturePointId.two);
			RegisterCapturePoint(CapturePointId.three);
		}

		private void RegisterCapturePoint(CapturePointId position)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			GetCapturePointsDependency.Point point = _dependency.points[(int)position];
			int team = point.team;
			float progress = point.progress;
			GameObject machineRoot = machineRootContainer.GetMachineRoot(TargetType.CapturePoint, (int)position);
			machineRoot.get_transform().set_position(point.position);
			machineRoot.get_transform().set_rotation(point.rotation);
			VisualTeam visualTeam = VisualTeam.None;
			if (team != -1)
			{
				visualTeam = ((!playerTeamsContainer.IsMyTeam(team)) ? VisualTeam.EnemyTeam : VisualTeam.MyTeam);
			}
			machineRoot.SetActive(true);
			minimapPresenter.RegisterCapturePointPosition(point.position, (int)position);
			if (team != -1)
			{
				minimapPresenter.SetCapturePointOwner(visualTeam == VisualTeam.MyTeam, (int)position);
			}
			CapturePointNotificationDependency capturePointNotificationDependency = new CapturePointNotificationDependency();
			capturePointNotificationDependency.SetParameters(CapturePointNotification.Spawn, (int)position, point.position, team, -1);
			notificationObservable.Dispatch(ref capturePointNotificationDependency);
			TeamBaseStateDependency teamBaseStateDependency = new TeamBaseStateDependency((int)position, progress, point.maxProgress);
			progressObservable.Dispatch(ref teamBaseStateDependency);
		}
	}
}
