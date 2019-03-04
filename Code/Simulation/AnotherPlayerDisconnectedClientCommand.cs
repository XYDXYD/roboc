using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal class AnotherPlayerDisconnectedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private PlayerIdDependency _dependency;

		[Inject]
		internal MachineSpawnDispatcher _machineSpawnDispatcher
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (PlayerIdDependency)dependency;
			return this;
		}

		public void Execute()
		{
			_machineSpawnDispatcher.PlayerSpawnedOut(_dependency.owner);
		}
	}
}
