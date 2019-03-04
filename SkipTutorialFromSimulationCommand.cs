using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

internal sealed class SkipTutorialFromSimulationCommand : ICommand
{
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

	public void Execute()
	{
		loadingIconPresenter.NotifyLoading("TutorialSimulationFlow");
		CreateAndExecuteTask();
	}

	private void CreateAndExecuteTask()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		SerialTaskCollection val = new SerialTaskCollection();
		val.Add(SwitchToPreviousRobotFromTutorialRobot());
		val.Add(ExitTutorialRequest());
		val.Add(ForceReloadCurrentMachineRequest());
		val.Add(ExecuteSwitchToMothershipCommand());
		TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator((IEnumerator)val)
			.Start((Action<PausableTaskException>)OnFail, (Action)null);
	}

	private void OnFail(PausableTaskException e)
	{
		loadingIconPresenter.NotifyLoadingDone("TutorialSimulationFlow");
		ServiceBehaviour serviceBehaviour = (((Exception)e).InnerException as ServiceException)._serviceBehaviour;
		GenericErrorData error = new GenericErrorData(serviceBehaviour.errorTitle, serviceBehaviour.errorBody, serviceBehaviour.mainText, serviceBehaviour.alternativeText, delegate
		{
			loadingIconPresenter.NotifyLoading("TutorialSimulationFlow");
			CreateAndExecuteTask();
		}, serviceBehaviour.Alternative);
		ErrorWindow.ShowErrorWindow(error);
	}

	private IEnumerator ExecuteSwitchToMothershipCommand()
	{
		loadingIconPresenter.NotifyLoadingDone("TutorialSimulationFlow");
		worldSwitching.SwitchToMothershipFromTutorialSimulation();
		yield return null;
	}

	private IEnumerator ExitTutorialRequest()
	{
		IUpdateTutorialStatusRequest updateTutorialStatusRequest = serviceFactory.Create<IUpdateTutorialStatusRequest>();
		updateTutorialStatusRequest.Inject(new UpdateTutorialStatusData(inProgress_: false, skipped_: true, completed_: false));
		yield return new TaskService(updateTutorialStatusRequest);
	}

	private IEnumerator ForceReloadCurrentMachineRequest()
	{
		ILoadMachineRequest request = serviceFactory.Create<ILoadMachineRequest>();
		request.ForceClearCache();
		TaskService<LoadMachineResult> requestTask = request.AsTask();
		yield return requestTask;
		if (!requestTask.succeeded)
		{
			throw new ServiceException(requestTask.behaviour);
		}
	}

	private IEnumerator SwitchToPreviousRobotFromTutorialRobot()
	{
		ISwitchToPreviousRobotIfNecessaryRequest selectTutorialRobotRequest = serviceFactory.Create<ISwitchToPreviousRobotIfNecessaryRequest>();
		yield return new TaskService(selectTutorialRobotRequest);
	}
}
