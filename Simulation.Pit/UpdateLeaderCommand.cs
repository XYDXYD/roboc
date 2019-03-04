using Svelto.Command;
using Svelto.IoC;

namespace Simulation.Pit
{
	internal class UpdateLeaderCommand : IInjectableCommand<int>, ICommand
	{
		private int _playerId;

		[Inject]
		internal IBattleEventStreamManager battleEventStreamManager
		{
			private get;
			set;
		}

		[Inject]
		internal HUDPlayerIDManager hudPlayerIDManager
		{
			private get;
			set;
		}

		public ICommand Inject(int playerId)
		{
			_playerId = playerId;
			return this;
		}

		public void Execute()
		{
			IBattleEventStreamManagerPit battleEventStreamManagerPit = battleEventStreamManager as IBattleEventStreamManagerPit;
			battleEventStreamManagerPit.NewLeader(_playerId);
			hudPlayerIDManager.LeaderUpdate(_playerId);
		}
	}
}
