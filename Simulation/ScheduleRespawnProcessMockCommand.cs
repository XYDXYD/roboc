using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class ScheduleRespawnProcessMockCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private KillDependency _dependency;

		[Inject]
		internal MachineSpawnDispatcher dispatcher
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (KillDependency)dependency;
			return this;
		}

		public void Execute()
		{
			dispatcher.ScheduleRespawn(_dependency.playerId, 8);
		}
	}
}
