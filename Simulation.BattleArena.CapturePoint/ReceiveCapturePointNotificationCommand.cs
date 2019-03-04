using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.BattleArena.CapturePoint
{
	internal sealed class ReceiveCapturePointNotificationCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private CapturePointNotificationDependency _dependency;

		[Inject]
		internal PlayerTeamsContainer teamContainer
		{
			private get;
			set;
		}

		[Inject]
		internal IMinimapPresenter minimapPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal CapturePointNotificationObservable observable
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as CapturePointNotificationDependency);
			return this;
		}

		public void Execute()
		{
			if (_dependency.notification == CapturePointNotification.CaptureCompleted)
			{
				int id = _dependency.id;
				bool isMyTeam = teamContainer.IsMyTeam(_dependency.attackingTeam);
				teamContainer.ChangePlayerTeam(TargetType.CapturePoint, id, _dependency.attackingTeam);
				minimapPresenter.SetCapturePointOwner(isMyTeam, id);
			}
			observable.Dispatch(ref _dependency);
		}
	}
}
