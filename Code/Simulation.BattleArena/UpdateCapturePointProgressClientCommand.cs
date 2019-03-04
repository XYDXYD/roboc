using Simulation.BattleArena.CapturePoint;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.BattleArena
{
	internal class UpdateCapturePointProgressClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private TeamBaseStateDependency _dependency;

		[Inject]
		internal CapturePointProgressObservable observable
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as TeamBaseStateDependency);
			return this;
		}

		public void Execute()
		{
			observable.Dispatch(ref _dependency);
		}
	}
}
