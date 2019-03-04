using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal class LeaveMatchmakingQueueCommand : ICommand
	{
		[Inject]
		internal LobbyPresenter LobbyPresenter
		{
			private get;
			set;
		}

		public void Execute()
		{
			LobbyPresenter.LeaveMatchmakingQueue();
		}
	}
}
