namespace Simulation.SinglePlayer
{
	internal class SinglePlayerTestModeAndTutorialRespawner : SinglePlayerRespawner
	{
		private GetFreeRespawnPointServerMockCommandTestModeTutorial _command;

		protected override void BuildRespawnCommand()
		{
			_command = base.commandFactory.Build<GetFreeRespawnPointServerMockCommandTestModeTutorial>();
		}

		protected override void ExecuteRespawnCommand(PlayerIdDependency dependency)
		{
			_command.Inject(dependency);
			_command.Execute();
		}
	}
}
