using Simulation.GUI;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class AssistConfirmedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private KillDependency _dependency;

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
			killNotificationController.OnKillAssistRewarded(_dependency.killerPlayerId, _dependency.playerId);
		}
	}
}
