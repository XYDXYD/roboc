namespace Simulation
{
	internal sealed class InitialMultiplayerGUIFlowNormal : InitialMultiplayerGUIFlow
	{
		protected override void CustomiseUIStyle()
		{
			CustomiseBattleStatsPresenterCommand customiseBattleStatsPresenterCommand = base.commandFactory.Build<CustomiseBattleStatsPresenterCommand>();
			customiseBattleStatsPresenterCommand.Inject(GameModeType.Normal);
			customiseBattleStatsPresenterCommand.Execute();
		}
	}
}
