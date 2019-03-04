using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SurrenderDeclinedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SurrenderDeclinedDependency _dependency;

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal SurrenderControllerClient surrenderControllerClient
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as SurrenderDeclinedDependency);
			return this;
		}

		public void Execute()
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, _dependency._surrenderingPlayerId))
			{
				surrenderControllerClient.SurrenderDeclined(_dependency._gameTimeElapsed, surrenderControllerClient.playerCooldownSeconds);
			}
			else
			{
				surrenderControllerClient.SurrenderDeclined(_dependency._gameTimeElapsed, surrenderControllerClient.teamCooldownSeconds);
			}
		}
	}
}
