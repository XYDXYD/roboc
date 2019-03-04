using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Pit
{
	internal sealed class StartPitModeClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GameTimeDependency _dependency;

		[Inject]
		public MultiplayerGameTimerClient multiplayerGameTimer
		{
			private get;
			set;
		}

		[Inject]
		public LobbyGameStartPresenter lobbyGameStartPresenter
		{
			private get;
			set;
		}

		[Inject]
		public GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public AllowMovementObservable allowMovementObservable
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
			multiplayerGameTimer.SetCurrentTime(_dependency.time);
			lobbyGameStartPresenter.OnGameStart();
			gameStartDispatcher.OnGameStart();
			bool flag = true;
			allowMovementObservable.Dispatch(ref flag);
		}
	}
}
