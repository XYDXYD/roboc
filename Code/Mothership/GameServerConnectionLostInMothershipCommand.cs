using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Mothership
{
	internal class GameServerConnectionLostInMothershipCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal LobbyPresenter lobbyPresenter
		{
			private get;
			set;
		}

		public void Execute()
		{
			lobbyPresenter.GameServerConnectionLost();
		}
	}
}
