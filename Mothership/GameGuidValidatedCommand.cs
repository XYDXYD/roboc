using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using Utility;

namespace Mothership
{
	internal class GameGuidValidatedCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal LobbyPresenter lobbyPresenter
		{
			private get;
			set;
		}

		public void Execute()
		{
			Console.Log("Game GUID validated");
			lobbyPresenter.GameGuidValidated();
		}
	}
}
