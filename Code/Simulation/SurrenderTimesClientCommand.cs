using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SurrenderTimesClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SurrenderTimesDependency _dependency;

		[Inject]
		internal SurrenderControllerClient surrenderControllerClient
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as SurrenderTimesDependency);
			return this;
		}

		public void Execute()
		{
			surrenderControllerClient.SetSurrenderTimes(_dependency.playerCooldownSeconds, _dependency.teamCooldownSeconds, _dependency.surrenderTimeoutSeconds, _dependency.initialSurrenderTimeoutSeconds);
		}
	}
}
