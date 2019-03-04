using Simulation.GUI;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class KillConfirmedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private KillDependency _dependency;

		[Inject]
		internal IBattleEventStreamManager battleEventStreamManager
		{
			private get;
			set;
		}

		[Inject]
		internal KillNotificationController killNotificationController
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
			battleEventStreamManager.PlayerWasKilledBy(_dependency.playerId, _dependency.killerPlayerId);
			killNotificationController.OnPlayerKilled(_dependency.killerPlayerId, _dependency.playerId);
		}
	}
}
