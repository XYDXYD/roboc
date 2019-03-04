using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Pit
{
	internal class UpdateKillStreakCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private int _playerId;

		private uint _streak;

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
			battleEventStreamManagerPit.StreakUpdate(_playerId, _streak);
			hudPlayerIDManager.StreakUpdate(_playerId, _streak);
		}

		public IDispatchableCommand Inject(object dep)
		{
			UpdateKillStreamDependency updateKillStreamDependency = dep as UpdateKillStreamDependency;
			_playerId = updateKillStreamDependency.playerId;
			_streak = updateKillStreamDependency.streak;
			return this;
		}
	}
}
