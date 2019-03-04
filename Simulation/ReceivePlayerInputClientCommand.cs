using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class ReceivePlayerInputClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private MultiPlayerInputChangedDependency _dependency;

		[Inject]
		public NetworkInputRecievedManager inputRecievedManager
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as MultiPlayerInputChangedDependency);
			return this;
		}

		public void Execute()
		{
			inputRecievedManager.InputReceived(_dependency);
		}
	}
}
