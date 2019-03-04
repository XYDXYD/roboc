using Achievements;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;

internal sealed class FinishedTutorialCommand : ICommand
{
	private bool _exitedTutorial;

	private bool _robotReloaded;

	private bool _robotSwitched;

	[Inject]
	internal WorldSwitching worldSwitching
	{
		private get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingIconPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal IAchievementManager achievementManager
	{
		private get;
		set;
	}

	public void Execute()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		achievementManager.CompletedTutorial();
		loadingIconPresenter.NotifyLoading("TutorialSimulationFlow");
		SerialTaskCollection val = new SerialTaskCollection();
		val.Add(SwitchToPreviousRobotFromTutorialRobot());
		val.Add(ExitTutorialRequest());
		val.Add(ForceReloadCurrentMachineRequest());
		val.Add(ExecuteSwitchToMothershipCommand());
		TaskRunner.get_Instance().Run((IEnumerator)val);
	}

	private IEnumerator ExecuteSwitchToMothershipCommand()
	{
		while (!_robotSwitched && !_exitedTutorial && !_robotReloaded)
		{
			yield return null;
		}
		loadingIconPresenter.NotifyLoadingDone("TutorialSimulationFlow");
		worldSwitching.SwitchToMothershipFromTutorialSimulation();
	}

	private IEnumerator ExitTutorialRequest()
	{
		IUpdateTutorialStatusRequest updateTutorialStatusRequest = serviceFactory.Create<IUpdateTutorialStatusRequest>();
		updateTutorialStatusRequest.Inject(new UpdateTutorialStatusData(inProgress_: false, skipped_: false, completed_: true));
		yield return new TaskService(updateTutorialStatusRequest);
		_exitedTutorial = true;
	}

	private IEnumerator ForceReloadCurrentMachineRequest()
	{
		ILoadMachineRequest currentMachineRequest = serviceFactory.Create<ILoadMachineRequest>();
		currentMachineRequest.ForceClearCache();
		yield return new TaskService<LoadMachineResult>(currentMachineRequest);
		_robotReloaded = true;
	}

	private IEnumerator SwitchToPreviousRobotFromTutorialRobot()
	{
		ISwitchToPreviousRobotIfNecessaryRequest selectTutorialRobotRequest = serviceFactory.Create<ISwitchToPreviousRobotIfNecessaryRequest>();
		yield return new TaskService(selectTutorialRobotRequest);
		_robotSwitched = true;
	}
}
