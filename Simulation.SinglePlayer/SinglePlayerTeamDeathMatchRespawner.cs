using Svelto.IoC;

namespace Simulation.SinglePlayer
{
	internal class SinglePlayerTeamDeathMatchRespawner : SinglePlayerRespawner
	{
		private GetFreeRespawnPointServerMockCommand _command;

		[Inject]
		public PlayerSpawnPointObservable playerSpawnPointsObservable
		{
			private get;
			set;
		}

		public override void GetRespawnPoint()
		{
			playerSpawnPointsObservable.Dispatch();
		}

		protected override void BuildRespawnCommand()
		{
			_command = base.commandFactory.Build<GetFreeRespawnPointServerMockCommand>();
		}

		protected override void ExecuteRespawnCommand(PlayerIdDependency dependency)
		{
			_command.Inject(dependency);
			_command.Execute();
		}
	}
}
