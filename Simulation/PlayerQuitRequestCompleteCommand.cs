using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal class PlayerQuitRequestCompleteCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal PlayerQuitRequestCompleteObservable _observable
		{
			get;
			set;
		}

		public void Execute()
		{
			_observable.Dispatch();
		}
	}
}
