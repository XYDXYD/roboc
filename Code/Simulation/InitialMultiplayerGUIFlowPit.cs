namespace Simulation
{
	internal sealed class InitialMultiplayerGUIFlowPit : InitialMultiplayerGUIFlow
	{
		public override void CreateLobbyStartScreen()
		{
			base.gameObjectFactory.Build("BattleStart_PIT");
		}

		protected override void CustomiseUIStyle()
		{
			CustomiseBattleStatsPresenterCommand customiseBattleStatsPresenterCommand = base.commandFactory.Build<CustomiseBattleStatsPresenterCommand>();
			customiseBattleStatsPresenterCommand.Inject(GameModeType.Pit);
			customiseBattleStatsPresenterCommand.Execute();
		}
	}
}
