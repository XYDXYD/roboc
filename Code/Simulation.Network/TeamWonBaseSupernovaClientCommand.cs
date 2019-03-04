using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Network
{
	internal sealed class TeamWonBaseSupernovaClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GameWonDependency _dependency;

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal SupernovaPlayer supernovaPlayer
		{
			private get;
			set;
		}

		[Inject]
		internal GameStateClient gameStateClient
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerSim guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal InputController inputController
		{
			private get;
			set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as GameWonDependency);
			return this;
		}

		public void Execute()
		{
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Won, _dependency.gameEndReason);
			gameEndedObserver.GameEnded(won: true);
			guiInputController.SetShortCutMode(ShortCutMode.OnlyEsc);
			inputController.Enabled = false;
			supernovaPlayer.PlaySupernova(_dependency.winningTeam, PlayGui);
		}

		private void PlayGui()
		{
			ICommand val = commandFactory.Build<TriggerGameWonClientCommand>().Inject(_dependency);
			val.Execute();
		}
	}
}
