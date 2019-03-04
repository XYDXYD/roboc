using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class ScheduleRespawnProcessCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private RespawnTimeDependency _dependency;

		[Inject]
		internal MachineSpawnDispatcher dispatcher
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as RespawnTimeDependency);
			return this;
		}

		public void Execute()
		{
			dispatcher.ScheduleRespawn(_dependency.owner, _dependency.waitingTime);
		}
	}
}
