using Svelto.Command;
using Svelto.IoC;

namespace Simulation.Pit
{
	internal class KillStreakLostCommand : IInjectableCommand<int>, ICommand
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

		public void Execute()
		{
			IBattleEventStreamManagerPit battleEventStreamManagerPit = battleEventStreamManager as IBattleEventStreamManagerPit;
			battleEventStreamManagerPit.StreakLost(_playerId);
			hudPlayerIDManager.StreakUpdate(_playerId, 0u);
		}

		public ICommand Inject(int playerId)
		{
			_playerId = playerId;
			return this;
		}
	}
}
