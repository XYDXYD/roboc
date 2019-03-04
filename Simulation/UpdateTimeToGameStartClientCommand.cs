using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class UpdateTimeToGameStartClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GameTimeDependency _dependency;

		[Inject]
		public LobbyGameStartPresenter lobbyGameStartPresenter
		{
			private get;
			set;
		}

		[Inject]
		public IInitialSimulationGUIFlow initialSimulationGuiFlow
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		public WorldSwitching worldSwitch
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as GameTimeDependency);
			return this;
		}

		public void Execute()
		{
			worldSwitch.StartPlanet();
			initialSimulationGuiFlow.OnStartCountdown();
			lobbyGameStartPresenter.SetTimer(_dependency.time);
		}
	}
}
