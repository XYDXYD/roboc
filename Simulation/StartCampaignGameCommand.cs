using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class StartCampaignGameCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GameStartDependency _dependency = new GameStartDependency(isReconnecting_: false);

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

		public void Execute()
		{
			lobbyGameStartPresenter.OnGameStart();
			gameStartDispatcher.OnGameStart(_dependency.isReconnecting);
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (GameStartDependency)dependency;
			return this;
		}
	}
}
