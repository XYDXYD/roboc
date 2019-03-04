using Simulation;
using Simulation.Pit;

internal class MainSimulationPitMode : MainSimulationMultiplayer
{
	protected override void SetUpContainerForGameMode()
	{
		base.container.Bind<IPauseMenuController>().AsInstance<PauseMenuControllerPitMode>(new PauseMenuControllerPitMode(GetBattleLeftObserverable()));
		base.container.BindSelf<PitLeaderGraphicalEffect>();
		base.container.BindSelf<PitLeaderObserver>();
		base.container.BindSelf<PitModeHudPresenter>();
		base.container.Bind<IBattleEventStreamManager>().AsSingle<BattleEventStreamManagerPit>();
		base.container.BindSelf<BattleStatsPresenter>();
		base.container.Bind<ISpectatorModeActivator>().AsSingle<SpectatorModeActivatorPit>();
		base.container.Bind<IInitialSimulationGUIFlow>().AsSingle<InitialMultiplayerGUIFlowPit>();
		base.container.Bind<IMinimapPresenter>().AsSingle<MinimapPresenter>();
		base.container.Bind<ChatPresenter>().AsSingle<ChatPresenterPit>();
		base.container.Bind<IMusicManager>().AsSingle<PitModeMusicManager>();
		base.container.BindSelf<PitModeMusicManager>();
	}

	protected override void SetupNetworkContainerForGameMode()
	{
		base.container.Bind<BonusManager>().AsSingle<BonusManagerPit>();
	}

	protected override void SetUpEntitySystemForGameMode()
	{
	}

	protected override void BuildGameObjectsForGameMode()
	{
	}

	protected override void BuildClassesForContextGameMode()
	{
		base.container.Build<PitLeaderGraphicalEffect>();
	}
}
