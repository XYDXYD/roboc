using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal class MachineDestroyedConfirmedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private KillDependency _dependency;

		[Inject]
		internal DestructionManager destructionManager
		{
			private get;
			set;
		}

		public void Execute()
		{
			int killerPlayerId = _dependency.killerPlayerId;
			int playerId = _dependency.playerId;
			destructionManager.DestroyMachine(playerId, killerPlayerId);
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (KillDependency)dependency;
			return this;
		}
	}
}
