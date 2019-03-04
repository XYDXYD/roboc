using InputMask;
using Mothership;
using Svelto.ServiceLayer;
using WebServices;

internal sealed class MainLevelTutorial : MainLevel
{
	public override void BindContextSpecificItems(InvalidPlacementObservable invalidPlacementObservable)
	{
		container.BindSelf<InitialMothershipGUIFlowTutorial>();
		container.Bind<IServiceRequestFactory>().AsSingle<WebStorageRequestFactoryTutorial>();
		container.Bind<IPauseMenuController>().AsSingle<PauseMenuControllerMothershipTutorial>();
		container.Bind<ICubeLauncherPermission>().AsInstance<TutorialCubeLauncherPermission>(new TutorialCubeLauncherPermission(invalidPlacementObservable));
		container.Bind<IGhostCubeVisibilityChecker>().AsSingle<TutorialGhostCubeVisibilityChecker>();
		container.Bind<ICubeSelectVisibilityChecker>().AsSingle<CubeSelectVisibilityCheckerTutorial>();
		container.Bind<IInputActionMask>().AsSingle<InputActionMaskTutorial>();
		container.Bind<IDummyTestModeScreenDisplay>().AsInstance<DummyTestModeScreenDisplayTutorial>(new DummyTestModeScreenDisplayTutorial());
		container.Bind<ICubeHolder>().AsSingle<TutorialCubeHolder>();
	}

	public override void BuildContextSpecificItems()
	{
		container.Build<InitialMothershipGUIFlowTutorial>();
	}

	public override ITopBarDisplay BuildContextSpecificTopBar()
	{
		TopBarDisplayTutorial topBarDisplayTutorial = new TopBarDisplayTutorial();
		container.Bind<ITopBarDisplay>().AsInstance<TopBarDisplayTutorial>(topBarDisplayTutorial);
		container.Inject<TopBarDisplayTutorial>(topBarDisplayTutorial);
		return topBarDisplayTutorial;
	}
}
