using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class UpdateCurrentGameTimeClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GameTimeDependency _dependency;

		[Inject]
		public MultiplayerGameTimerClient multiplayerGameTimer
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (GameTimeDependency)dependency;
			return this;
		}

		public void Execute()
		{
			multiplayerGameTimer.SetCurrentTime(_dependency.time);
		}
	}
}
